using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class OrderController : BaseController
	{
		[HttpPost]
		[Route("Order/Payment")]
		public async Task<JsonResult> PaypalPaymentAsync()
		{
			if (!_isLogin || !CheckId()) throw new Exception("domain error");

			int productIdx = Convert.ToInt32(Request.Form["productIdx"]);
			int quantity = 1; //Convert.ToInt32(Request.Form["quantity"]);
			string shippingAddress = Request.Form["shippingAddress"].ToString();
			string encodedShippingAddress = HttpUtility.UrlEncode(shippingAddress);
			bool status = CheckDomain();
			string result = string.Empty;
			string movePage = status ? string.Empty : "/Error";
			string loginInfo = GetCookie(_loginCookieKey);

			Member_Dto memberDto = new()
			{
				MEMBER_IDX = _memberIdx,
			};

			memberDto = new Member_Dac().Select_MemberInfo(memberDto);
			status = !string.IsNullOrEmpty(memberDto.COIN_ADDRESS);

			if (status)
			{
				// 제품정보 가져오기
				Product_Dto productDto = new()
				{
					PRODUCT_IDX = productIdx,
				};

				productDto = new Product_Dac().Select_ProductInfo(productDto);

				int nftTotalCount = (int)productDto.NFT_TOTAL_COUNT!;
				status = nftTotalCount > productDto.NFT_MINTING_COUNT;

				// NFT 잔여 수량 체크
				if (status)
				{
					// 민팅 설정 정보
					Setting_Dto settingInfo = new Setting_Dac().Select_MintingSettingInfo();

					string contractAddress = productDto.CONTRACT_ADDRESS!;

					// 민팅 가능 여부 체크
					string errorCode = await new SendProcess().CheckMinting(settingInfo, contractAddress, nftTotalCount);
					status = string.IsNullOrEmpty(errorCode);

					if (status)
					{
						productDto.QUANTITY = quantity;
						productDto.SHIPPING_ADDRES = shippingAddress;
						productDto.MEMBER_NAME = string.IsNullOrEmpty(loginInfo) ? string.Empty : loginInfo;

						string completeUrl = "https://" + _domain + "/Order/" + productIdx + "?paymentType=0002" + "&shippingAddress=" + encodedShippingAddress;
						string cancelUrl = "https://" + _domain + "/Product/Buy/" + productIdx + "?isCancel=true";

						var payment = new PaypalPayment().CreatePayment(productDto, completeUrl, cancelUrl);
						var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

						movePage = approvalUrl!;
					}
					else
					{
						result = errorCode.Equals("1002") || errorCode.Equals("1003") ? "SOLD OUT" : string.Format(_defaultErrorMessage, errorCode);
					}
				}
				else
				{
					result = "SOLD OUT";
				}
			}
			else
			{
				result = "지갑 등록 후 이용해 주세요.";
			}

			return Json(new { status, result, movePage });
		}

		[HttpPost]
		[Route("Order/Checked")]
		public async Task<JsonResult> CheckedAccountAsync()
		{
			if (!_isLogin || !CheckId()) throw new Exception("domain error");

			bool status = true;
			string result = string.Empty;
			string movePage = status ? string.Empty : "/Error";
			int productIdx = Convert.ToInt32(Request.Form["productIdx"]);
			//int quantity = 1; // Convert.ToInt32(Request.Form["quantity"]);
			string bankName = Request.Form["bankName"].ToString();
			string accountOwner = Request.Form["accountOwner"].ToString();
			string accountNumber = Request.Form["accountNumber"].ToString();

			//status = (quantity <= 10 && quantity > 0);

			if (status)
			{
				Member_Dto memberDto = new()
				{
					MEMBER_IDX = _memberIdx,
				};

				memberDto = new Member_Dac().Select_MemberInfo(memberDto);
				status = !string.IsNullOrEmpty(memberDto.COIN_ADDRESS);

				if (status)
				{
					Product_Dto productDto = new()
					{
						PRODUCT_IDX = productIdx
					};

					productDto = new Product_Dac().Select_ProductInfo(productDto);

					int nftTotalCount = (int)productDto.NFT_TOTAL_COUNT!;
					status = nftTotalCount > productDto.NFT_MINTING_COUNT;

					// NFT 잔여 수량 체크
					if (status)
					{
						// 민팅 설정 정보
						Setting_Dto settingInfo = new Setting_Dac().Select_MintingSettingInfo();

						string contractAddress = productDto.CONTRACT_ADDRESS!;

						// 민팅 가능 여부 체크
						string errorCode = await new SendProcess().CheckMinting(settingInfo, contractAddress, nftTotalCount);
						status = string.IsNullOrEmpty(errorCode);

						if (status)
						{
							string currentBankName = productDto.BANK_NAME!;
							string currentAccountOwner = productDto.ACCOUNT_OWNER!;
							string currentAccountNumber = productDto.ACCOUNT_NUMBER!;
							status = currentBankName.Equals(bankName) && currentAccountOwner.Equals(accountOwner) && currentAccountNumber.Equals(accountNumber);
							if (!status)
							{
								result = "입금 계좌 정보가 변경되었습니다.\r\n변경된 정보를 확인해 주세요.|";
							}
							result += currentBankName + "|" + currentAccountOwner + "|" + currentAccountNumber;
						}
						else
						{
							result = errorCode.Equals("1002") || errorCode.Equals("1003") ? "SOLD OUT" : string.Format(_defaultErrorMessage, errorCode);
						}
					}
				}
				else
				{
					result = "지갑 등록 후 이용해 주세요.";
				}
			}
			//else
			//{
			//	result = "수량은 1~10개만 가능합니다.";
			//}

			return Json(new { status, result, movePage });
		}

		[Route("Order/{productIdx}")]
		public async Task<IActionResult> OrderAsync(int productIdx, int quantity, string paymentType, string shippingAddress, string depositName = "",
			string paymentId = "", string token = "", string payerId = "")
		{
			if (!_isLogin || !CheckId()) throw new Exception("domain error");

			bool status;
			string errorCode = string.Empty;
			string orderNumber = DateTime.Now.ToString("yyMMddHHmmssffffff");
			bool isPaypalOrderComplete = false;     // 페이팔 결제 완료 여부
			bool isMintingComplete = false;			// 민팅 완료 여부
			SendProcess sendProcess = new();
			Product_Dac productDac = new();
			Paypal_Dac paypalDac = new();
			Paypal_Dto paypalDto;
			PaypalPayment paypalPayment = new();

			bool isPaypal = paymentType.Equals("0002");
			shippingAddress = isPaypal ? HttpUtility.UrlDecode(shippingAddress) : shippingAddress;
			depositName = isPaypal ? HttpUtility.UrlDecode(depositName) : depositName;
			quantity = 1;

			try 
			{
				Product_Dto productDto = new()
				{
					PRODUCT_IDX = productIdx,
				};

				productDto = productDac.Select_ProductInfo(productDto); // ★Dac★ 판매정보 조회
				status = productDto != null;

				if (status)
				{
					Member_Dac memberDac = new();
					Member_Dto memberDto = new()
					{
						MEMBER_IDX = _memberIdx,
					};

					memberDto = memberDac.Select_MemberInfo(memberDto);
					string coinAddress = memberDto.COIN_ADDRESS!;
					status = !string.IsNullOrEmpty(coinAddress);

					if (status)
					{
						Order_Dto orderDto = new()
						{
							ORDER_NUMBER = orderNumber,
							PRODUCT_IDX = productIdx,
							MEMBER_IDX = _memberIdx,
							QUANTITY = quantity,
							ORDER_AMOUNT = productDto!.AMOUNT * quantity,
							SHIPPING_ADDRESS = Function.SetXSSStringConvert(shippingAddress),
							IS_DEFAULT_ADDRESS = true,
							ORDER_TYPE_CODE = paymentType,
							DEPOSIT_ACCOUNT_IDX = isPaypal ? 0 : productDto.DEPOSIT_ACCOUNT_IDX,
							DEPOSIT_NAME = Function.SetXSSStringConvert(depositName),
							BANK_CODE = isPaypal ? string.Empty : productDto.BANK_CODE,
							ACCOUNT_OWNER = isPaypal ? string.Empty : productDto.ACCOUNT_OWNER,
							ACCOUNT_NUMBER = isPaypal ? string.Empty : productDto.ACCOUNT_NUMBER
						};

						if (isPaypal) // 페이팔인 경우
						{
							orderDto.PAYMENT_ID = paymentId;
							paypalDto = paypalPayment.GetPaymentInfo(paymentId); // 결제 정보 가져오기
							paypalDto.STATE_CODE = "0001";
							status = paypalDac.Insert_PaypalPayment(paypalDto); // 결제정보 등록 (등록이 아니라 수정으로 변경되어야 함)

							if (status)
							{
								status = paypalPayment.PaymentComplete(paymentId, payerId); // 결제 완료
								isPaypalOrderComplete = status;

								if (status)
								{
									paypalDto = paypalPayment.GetPaymentInfo(paymentId); // 결제 정보 가져오기
									paypalDto.STATE_CODE = "0002";

									status = paypalDac.Update_PaypalPayment(paypalDto); // 결제정보 수정 (등록이 아니라 수정으로 변경되어야 함)
									if (!status)
									{
										errorCode = "0004";
									}
								}
								else
								{
									errorCode = "0003";
								}
							}
							else
							{
								errorCode = "0002";
							}
						}

						string mintingAddress = string.Empty;
						string txid = string.Empty;
						int tokenId = 0;
						long nonce = 0;

						// 주문 정보 등록 및 NFT 민팅
						if (status)
						{
							Order_Dac orderDac = new();

							// 페이팔의 경우에만 발행한다. 결제가 완료되었기 때문(무통장은 관리자 승인시 발행)
							if (isPaypal)
							{
								int nftTotalCount = (int)productDto.NFT_TOTAL_COUNT!;
								status = nftTotalCount > productDto.NFT_MINTING_COUNT;

								// NFT 잔여 수량 체크
								if (status)
								{
									// 민팅 설정 정보
									Setting_Dto settingInfo = new Setting_Dac().Select_MintingSettingInfo();

									string contractAddress = productDto.CONTRACT_ADDRESS!;

									// 민팅 가능 여부 체크
									errorCode = await sendProcess.CheckMinting(settingInfo, contractAddress, nftTotalCount);

									status = string.IsNullOrEmpty(errorCode);

									if (status)
									{
										List<int> existingTokenIds = orderDac.Select_MintingNftList(productIdx);
										tokenId = Function.GetRandomTokenId(nftTotalCount, existingTokenIds);

										if (tokenId > 0)
										{
											mintingAddress = coinAddress;

											// NFT 민팅
											string[] mintingResult = await sendProcess.Minting(settingInfo, contractAddress, mintingAddress, tokenId);
											status = Convert.ToBoolean(mintingResult[0]);
											isMintingComplete = status;

											if (status)
											{
												// 민팅 개수 증가
												productDac.Update_MintingCount(productDto);
												
												txid = mintingResult[1];
												nonce = Convert.ToInt64(mintingResult[2]);
											}
											else
											{
												errorCode = "0006";
											}
										}
										else
										{
											errorCode = "0007";
										}
									}
								}
								else
								{
									errorCode = "0005";
								}
							}

							string orderStateCode = string.Empty;

							// 주문 정보 등록(실패시 취소로 변경)
							if (status)
							{
								orderStateCode = isPaypal ? "0002" : "0001";
							}
							else
							{
								orderStateCode = "0003";
							}

							orderDto.STATE_CODE = orderStateCode;
							orderDto.COIN_ADDRESS = mintingAddress;
							orderDto.TXID = txid;
							orderDto.TOKEN_ID = tokenId;
							orderDto.NONCE = nonce;
							orderDac.Insert_Order(orderDto); // 주문정보 생성 주문정보 테이블에 주문정보 INSERT(주문 대기 상태) -> 관리자에서 승인 해줘야 주문완료 상태로 변경
						}
					}
					else
					{
						errorCode = "0010";
					}
				}
				else
				{
					errorCode = "0001";
				}
			}
			catch (Exception ex) 
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				errorCode = "9999";
			}

			bool isError = !string.IsNullOrEmpty(errorCode);

			// 오류
			if (isError)
			{
				// 페이팔 결제가 완료되었으나 오류가 발생하면 결제 취소 처리, 그러나 민팅이 완료되면 취소하지 않는다.
				if (isPaypalOrderComplete && !isMintingComplete)
				{
					status = paypalPayment.CancelPayment(paymentId); // 페이팔 결제 취소

					if (status)
					{
						paypalDto = new()
						{
							PAYMENT_ID = paymentId,
							PAYMENT_STATE = "completed"
						};
						status = paypalDac.Update_CancelPayment(paypalDto);

						if (!status)
						{
							errorCode = "0009";
						}
					}
					else
					{
						errorCode = "0008";
					}
				}

				string[] sendHpNumbers = _isDebug ? new string[] { "01044224664" } : new string[] { "01044224664", "01091212729", "01033777025" };

				for (int i = 0; i < sendHpNumbers.Length; i++)
				{
					await sendProcess.SendHp(sendHpNumbers[i], string.Format("[{0}] 결제 오류 코드 => {1}", _serviceName, errorCode));
				}
			}

			return isError ? Redirect("/Order/Fail/" + productIdx + "/" + errorCode) : Redirect("/Order/Complete/" + orderNumber + "?paymentType=" + paymentType);
		}

		[HttpGet]
		[Route("Order/Complete/{orderNumber}")]
		public IActionResult Complete(string orderNumber, string paymentType)
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			Order_Dto orderDto = new()
			{ 
				ORDER_NUMBER = orderNumber,
				ORDER_TYPE_CODE = paymentType
			};

			orderDto = new Order_Dac().Select_OrderInfo(orderDto);

			return View(orderDto);
		}

		[HttpGet]
		[Route("Order/Fail/{productIdx}/{errorCode}")]
		public IActionResult Fail(int productIdx, string errorCode)
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			ViewData["productIdx"] = productIdx;
			ViewData["errorCode"] = errorCode;

			return View();
		}

		[HttpPost]
		[Route("Order/ListData")]
		public JsonResult ListData()
		{
			List<Order_Dto> list = null!;
			bool status = _isLogin && CheckId() && CheckDomain();

			if (status)
			{
				int page = Convert.ToInt32(Request.Form["page"]);
				string startDay = Request.Form["searchStartDay"]!;
				string endDay = Request.Form["searchEndDay"]!;
				startDay ??= DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
				endDay ??= DateTime.Now.ToString("yyyy-MM-dd");
				string stateCode = Request.Form["searchStateCode"]!;
				string productName = Request.Form["searchProductName"]!;

				Order_Dto orderDto = new()
				{
					PAGE = page,
					PAGE_SIZE = _pageSize,
					MEMBER_IDX = _memberIdx,
					START_DAY = startDay,
					END_DAY = DateTime.Parse(endDay).AddDays(1).ToString("yyyy-MM-dd")
				};
				if (!string.IsNullOrEmpty(stateCode)) orderDto.STATE_CODE = stateCode;
				if (!string.IsNullOrEmpty(productName)) orderDto.PRODUCT_NAME = "%" + Function.SetXSSStringConvert(productName) + "%";

				list = new Order_Dac().Select_OrderList(orderDto);
			}

			return Json(list);
		}
	}
}