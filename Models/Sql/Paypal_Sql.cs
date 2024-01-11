namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
	public class Paypal_Sql
	{
		/// <summary>
		/// 주문 정보 등록 (사용자 승인 상태)
		/// </summary>
		/// <returns></returns>
		public string Insert_PaypalPayment_Sql()
		{
			string query = @"
				INSERT INTO PAYPAL_PAYMENT_INFO
				(
					PAYMENT_ID,
					TRANSACTION_ID,
					AMOUNT,
					CURRENCY,
					PAYMENT_TYPE,
					PAYMENT_STATE,
					FEE_CURRENT,
					FEE_AMOUNT,
					MERCHANT_ID,
					STATE_CODE
				)
				VALUES
				(
					@PAYMENT_ID,
					@TRANSACTION_ID,
					@AMOUNT,
					@CURRENCY,
					@PAYMENT_TYPE,
					@PAYMENT_STATE,
					@FEE_CURRENT,
					@FEE_AMOUNT,
					@MERCHANT_ID,
					@STATE_CODE
				)
			";

			return query;
		}

		/// <summary>
		/// 주문 정보 수정 (결제 청구 상태)
		/// </summary>
		/// <returns></returns>
		public string Update_PaypalPayment_Sql()
		{
			string query = @"
				UPDATE PAYPAL_PAYMENT_INFO
				SET
					UPD_DATE = GETDATE(),
					TRANSACTION_ID = @TRANSACTION_ID,
					CURRENCY = @CURRENCY,
					PAYMENT_STATE = @PAYMENT_STATE,
					FEE_CURRENT = @FEE_CURRENT,
					FEE_AMOUNT = @FEE_AMOUNT,
					STATE_CODE = @STATE_CODE
				WHERE 
					PAYMENT_ID = @PAYMENT_ID
			";

			return query;
		}

		/// <summary>
		/// 페이팔 결제 취소
		/// </summary>
		/// <returns></returns>
		public string Update_CancelPayment_Sql()
		{
			string query = @"
				UPDATE PAYPAL_PAYMENT_INFO
				SET
					UPD_DATE = GETDATE(),
					PAYMENT_STATE = @PAYMENT_STATE,
					STATE_CODE = '0003'
				WHERE
					PAYMENT_ID = @PAYMENT_ID
			";

			return query;
		}
	}
}