using PayPal.Api;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class PaypalPayment
    {
		protected readonly static IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
		protected readonly static string _paypalKey = _config.GetValue<string>("Paypal:Key")!;
		protected readonly static string _paypalSecret = _config.GetValue<string>("Paypal:Secret")!;
		protected readonly static string _paypalMode = _config.GetValue<string>("Paypal:Mode")!;
		protected readonly static string _paypalMerchantId = _config.GetValue<string>("Paypal:MerchantId")!;
		readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 페이팔 컨텍스트 생성
		/// </summary>
		/// <returns></returns>
		public APIContext GetApiContext()
		{
			var config = new Dictionary<string, string>
			{
				{ "mode", _paypalMode},
				{ "clientId", _paypalKey},
				{ "clientSecret", _paypalSecret},
			};

			var accessToken = new OAuthTokenCredential(config).GetAccessToken();
			return new APIContext(accessToken) { Config = config };
		}

		/// <summary>
		/// 페이팔 결제 진행
		/// </summary>
		/// <param name="product"></param>
		/// <param name="completeUrl"></param>
		/// <param name="cancelUrl"></param>
		/// <returns></returns>
		public Payment CreatePayment(Product_Dto product, string completeUrl, string cancelUrl)
		{
			decimal usdAmount = (decimal)product.USD_AMOUNT!;
			var apiContext = GetApiContext();
			var itemList = new ItemList
			{
				items = new List<Item>
				{
					new Item
					{
						name= product.PRODUCT_NAME,
						currency = "USD",
						price = usdAmount.ToString("F"),
						quantity = "1",//product.QUANTITY.ToString(),
						sku = Guid.NewGuid().ToString()
					}
				}
			};

			var totalAmount = usdAmount.ToString("F");
			var payment = new Payment
			{
				intent = "sale",
				payer = new Payer 
				{
					payment_method = "paypal",
					payer_info = new PayerInfo
					{ 
						shipping_address = new ShippingAddress
						{ 
							recipient_name= product.MEMBER_NAME,
							line1 = Function.SetXSSStringConvert(product.SHIPPING_ADDRES!),
						}
					}
				},
				
				transactions = new List<Transaction>
				{
					new Transaction
					{
						description = "Payment description",
						invoice_number = Guid.NewGuid().ToString(),
						amount = new Amount { currency = "USD", total = totalAmount },
						item_list = itemList
					}
				},
				redirect_urls = new RedirectUrls { return_url = completeUrl, cancel_url = cancelUrl }
			};

			return payment.Create(apiContext);
		}

		/// <summary>
		/// 결제 청구
		/// </summary>
		/// <param name="payerId"></param>
		/// <returns></returns>
		public bool PaymentComplete(string paymentId, string payerId)
		{
			bool result = true;

			try
			{
				var apiContext = GetApiContext();
				var payment = Payment.Get(apiContext, paymentId);
				var paymentExecution = new PaymentExecution() { payer_id = payerId };
				var excutedPayment = payment.Execute(apiContext, paymentExecution);
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				result = false;
			}

			return result!;
		}

		/// <summary>
		/// 결제 정보 가져오기
		/// </summary>
		/// <param name="paymentId"></param>
		/// <returns></returns>
		public Paypal_Dto GetPaymentInfo(string paymentId)
		{
			var apiContext = GetApiContext();
			var payment = Payment.Get(apiContext, paymentId);
			var transaction = payment.transactions.FirstOrDefault(); // 트랜잭션 아이디
			var transactionId = transaction!.related_resources.FirstOrDefault()?.sale.id;
			var amount = transaction.amount;
			
			var receiptId = transaction.related_resources.FirstOrDefault()?.sale.receipt_id;
			var fee = transaction!.related_resources.FirstOrDefault()?.sale.transaction_fee; // 수수료
			var paymentMethod = payment.payer.payment_method; // 결제 유형
			var paymentState = payment.state; // 결제 상태

			Paypal_Dto paypalDto = new()
			{
				PAYMENT_ID = paymentId,
				TOKEN = payment.token,
				TRANSACTION_ID = transactionId == null ? "" : transactionId,
				AMOUNT = Convert.ToDecimal(amount.total),
				CURRENCY = amount.currency,
				PAYMENT_TYPE = paymentMethod,
				PAYMENT_STATE = paymentState,
				FEE_CURRENT = fee == null ? "" : fee!.currency,
				FEE_AMOUNT = fee == null ? 0 : Convert.ToDecimal(fee!.value),
				MERCHANT_ID = _paypalMerchantId,
			};

			return paypalDto;
		}

		/// <summary>
		/// 페이팔 결제 취소
		/// </summary>
		/// <param name="paymentId"></param>
		/// <returns></returns>
		public bool CancelPayment(string paymentId)
		{
			var apiContext = GetApiContext();

			try
			{
				// 특정 결제 정보 가져오기
				var payment = Payment.Get(apiContext, paymentId);

				// Null or index out of range check
				if (payment.transactions == null || !payment.transactions.Any())
				{
					return false;
				}

				var transaction = payment.transactions.FirstOrDefault();
				if (transaction?.related_resources == null || !transaction.related_resources.Any())
				{
					return false;
				}

				// 결제 상태를 "voided"로 설정
				if (payment.state.ToLower() != "approved") // This may need to be refined based on business requirements
				{
					return false;
				}

				var saleId = transaction.related_resources.FirstOrDefault()?.sale?.id;
				if (string.IsNullOrEmpty(saleId))
				{
					return false;
				}

				var sale = Sale.Get(apiContext, saleId);
				var refund = new Refund()
				{
					amount = new Amount()
					{
						currency = sale.amount.currency,
						total = sale.amount.total
					}
				};

				// 실제로 취소 수행
				var response = sale.Refund(apiContext, refund)!;

				if (response.state.ToLower() == "completed")
				{
					return true;
				}
				// Optional: Log the response for debugging purposes
				// _exceptionLogDac.Insert_ExceptionLog(response);

				return false;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				return false;
			}
		}
	}
}