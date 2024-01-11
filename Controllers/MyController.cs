using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class MyController : BaseController
	{
		/// <summary>
		/// 내 정보
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			Member_Dto memberDto = new()
			{
				MEMBER_IDX = _memberIdx
			};

			return View(new Member_Dac().Select_MemberInfo(memberDto));
		}

		/// <summary>
		/// 주문 내역
		/// </summary>
		/// <returns></returns>
		[Route("My/OrderList")]
		public IActionResult OrderList()
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			Member_Dto memberDto = new()
			{
				MEMBER_IDX = _memberIdx
			};

			return View(new Member_Dac().Select_MemberInfo(memberDto));
		}

		/// <summary>
		/// 주문 상세
		/// </summary>
		/// <param name="orderNumber"></param>
		/// <param name="paymentType"></param>
		/// <returns></returns>
		[Route("My/Order/{orderNumber}/Detail")]
		public IActionResult OrderDetail(string orderNumber, string paymentType)
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			Order_Dto orderDto = new()
			{
				ORDER_NUMBER = orderNumber,
				ORDER_TYPE_CODE = paymentType
			};

			return View(new Order_Dac().Select_OrderInfo(orderDto));
		}

		/// <summary>
		/// 회원 탈퇴
		/// </summary>
		/// <returns></returns>
		public IActionResult ServiceWithdrawal()
		{
			if (!_isLogin || !CheckId()) return Redirect("/");

			return View();
		}

		/// <summary>
		///	비밀번호 체크
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("CheckCurrentPassword")]
		public JsonResult LoginProcess()
		{
			if (!_isLogin || !CheckId()) throw new Exception("domain error");

			bool status = true;
			string result = string.Empty;
			string? movePage = string.Empty;
			string memberPassword = Request.Form["memberPassword"]!;

			Dictionary<string, string> paramList = new()
			{
				{ "memberPassword", memberPassword }
			};

			foreach (KeyValuePair<string, string> item in paramList)
			{
				if (!Function.CheckValidation(item.Key, item.Value))
				{
					status = false;
					result = Function.GetMessage(item.Key, "0002");
					break;
				}
			}

			if (status)
			{
				Member_Dto memberDto = new()
				{
					MEMBER_IDX = _memberIdx
				};

				Member_Dto memberInfo = new Member_Dac().Select_MemberInfo(memberDto);

				if (!_isDebug)
				{
					memberPassword = Encrypter.Encrypt_MD5(memberPassword);
				}

				status = memberPassword.Equals(memberInfo.MEMBER_PASSWORD);

				if (!status)
				{
					result = "비밀번호를 확인해 주세요.";
				}
			}

			return Json(new { status, result, movePage });
		}

		/// <summary>
		///	회원 탈퇴 처리
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("ServiceWithdrawalProcess")]
		public JsonResult ServiceWithdrawalProcess()
		{
			if (!CheckDomain() || !_isLogin || !CheckId()) throw new Exception("check error");

			string result = string.Empty;
			bool status = true;
			string movePage = string.Empty;
			string memberPassword = Request.Form["memberPassword"]!;

			Dictionary<string, string> paramList = new()
			{
				{ "memberPassword", memberPassword }
			};

			foreach (KeyValuePair<string, string> item in paramList)
			{
				string key = item.Key;
				status = Function.CheckValidation(key, item.Value);

				if (!status)
				{
					result = Function.GetMessage(key, "0002");
					break;
				}
			}

			if (status)
			{
				Member_Dac memberDac = new();
				Member_Dto memberDto = new()
				{
					MEMBER_IDX = _memberIdx
				};

				Member_Dto memberInfo = memberDac.Select_MemberInfo(memberDto);
					
				if (!_isDebug)
				{
					memberPassword = Encrypter.Encrypt_MD5(memberPassword);
				}

				status = memberPassword.Equals(memberInfo.MEMBER_PASSWORD);

				if (status)
				{
					status = memberDac.Delete_MemberInfo(memberDto);

					if (status)
					{
						DeleteCookie(_loginCookieKey);
					}
					else
					{
						movePage = "/Error";
					}
				}
				else
				{
					result = "비밀번호를 확인해 주세요.";
				}
			}

			return Json(new { status, result, movePage });
		}

		/// <summary>
		/// 회원 탈퇴 완료
		/// </summary>
		/// <returns></returns>
		public IActionResult ServiceWithdrawalComplete()
		{
			if (_isLogin) return Redirect("/");

			return View();
		}

		/// <summary>
		///	회원 정보 변경
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("EditMemberProcess")]
		public JsonResult EditMemberProcess()
		{
			if (!_isLogin || !CheckId()) throw new Exception("domain error");
			string result = string.Empty;
			string? movePage = string.Empty;
			string memberEditTypeCode = Request.Form["memberEditTypeCode"]!;
			bool status = memberEditTypeCode.Equals("0001") || memberEditTypeCode.Equals("0002") || memberEditTypeCode.Equals("0003") || memberEditTypeCode.Equals("0004");

			if (status)
			{
				bool isPassword = memberEditTypeCode.Equals("0001");
				bool isHpNumber = memberEditTypeCode.Equals("0002");
				bool isShppingAddress = memberEditTypeCode.Equals("0003");
				bool isCoinAddress = memberEditTypeCode.Equals("0004");

				string memberPassword = isPassword ? Request.Form["memberPassword"]! : string.Empty;
				string newPassword = isPassword ? Request.Form["newPassword"]! : string.Empty;
				string hpNumber = isHpNumber ? Request.Form["hpNumber"]! : string.Empty;
				string shippingAddress = isShppingAddress ? Request.Form["shippingAddress"]! : string.Empty;
				string coinAddress = isCoinAddress ? Request.Form["coinAddress"]! : string.Empty;

				Dictionary<string, string> paramList = new();
				if (isPassword)
				{
					paramList.Add("memberPassword", memberPassword);
					paramList.Add("newPassword", newPassword);
				}
				else if (isHpNumber)
				{
					paramList.Add("hpNumber", hpNumber);
				}
				else if (isShppingAddress)
				{
					paramList.Add("shippingAddress", shippingAddress);
				}
				else if (isCoinAddress)
				{
					if (!string.IsNullOrEmpty(coinAddress))
					{
						paramList.Add("coinAddress", coinAddress);
					}
				}

				if (paramList.Count > 0)
				{
					foreach (KeyValuePair<string, string> item in paramList)
					{
						string key = item.Key;
						if (!Function.CheckValidation(key, item.Value))
						{
							status = false;
							result = Function.GetMessage(key, "0002");
							break;
						}
					}
				}

				if (status)
				{
					Member_Dac memberDac = new();
					Member_Dto memberDto = new()
					{
						MEMBER_IDX = _memberIdx
					};

					Member_Dto memberInfo = memberDac.Select_MemberInfo(memberDto);

					if (isPassword)
					{
						if (!_isDebug)
						{
							memberPassword = Encrypter.Encrypt_MD5(memberPassword);
						}
						status = memberPassword.Equals(memberInfo.MEMBER_PASSWORD);

						if (!status)
						{
							result = "현재 비밀번호를 확인해 주세요.";
						}
					}

					if (status)
					{
						if (isPassword)
						{
							if (!_isDebug)
							{
								newPassword = Encrypter.Encrypt_MD5(newPassword);
							}
							memberInfo.MEMBER_PASSWORD = newPassword;
							result = "MemberPassword|" + new string('＊', newPassword.Length);
						}
						else if (isHpNumber)
						{
							memberInfo.HP_NUMBER = hpNumber;
							result = "HpNumber|" + hpNumber;
						}
						else if (isShppingAddress)
						{
							memberInfo.SHIPPING_ADDRESS = Function.SetXSSStringConvert(shippingAddress);
							result = "ShippingAddress|" + shippingAddress;
						}
						else if (isCoinAddress)
						{
							memberInfo.COIN_ADDRESS = coinAddress;
							result = "CoinAddress|" + coinAddress;
						}

						status = memberDac.Update_MemberInfo(memberInfo);
						if (!status)
						{
							result = string.Format(_defaultErrorMessage, "0001");
						}
					}
				}
			}
			else
			{
				movePage = "/Error";
			}

			return Json(new { status, result, movePage });
		}
	}
}