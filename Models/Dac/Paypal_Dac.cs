using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Paypal_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Paypal_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 주문 정보 등록 (사용자 승인 상태)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Insert_PaypalPayment(Paypal_Dto request)
		{
			bool result = false;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PAYMENT_ID", request.PAYMENT_ID),
					new SqlParameter("@TRANSACTION_ID", request.TRANSACTION_ID),
					new SqlParameter("@AMOUNT", request.AMOUNT),
					new SqlParameter("@CURRENCY", request.CURRENCY),
					new SqlParameter("@PAYMENT_TYPE", request.PAYMENT_TYPE),
					new SqlParameter("@PAYMENT_STATE", request.PAYMENT_STATE),
					new SqlParameter("@FEE_CURRENT", request.FEE_CURRENT),
					new SqlParameter("@FEE_AMOUNT", request.FEE_AMOUNT),
					new SqlParameter("@MERCHANT_ID", request.MERCHANT_ID),
					new SqlParameter("@STATE_CODE", request.STATE_CODE),
				};

				result = _dbHelper.RunIntCmd(_query.Insert_PaypalPayment_Sql(), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				throw;
			}
			
			return result;
		}

		/// <summary>
		/// 주문 정보 수정 (결제 청구 상태)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Update_PaypalPayment(Paypal_Dto request)
		{
			bool result;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PAYMENT_ID", request.PAYMENT_ID),
					new SqlParameter("@TRANSACTION_ID", request.TRANSACTION_ID),
					new SqlParameter("@CURRENCY", request.CURRENCY),
					new SqlParameter("@PAYMENT_STATE", request.PAYMENT_STATE),
					new SqlParameter("@FEE_CURRENT", request.FEE_CURRENT),
					new SqlParameter("@FEE_AMOUNT", request.FEE_AMOUNT),
					new SqlParameter("@STATE_CODE", request.STATE_CODE),
				};

				result = _dbHelper.RunIntCmd(_query.Update_PaypalPayment_Sql(), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				throw;
			}

			return result;
		}

		/// <summary>
		/// 주문 취소 처리
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <returns></returns>
		public bool Update_CancelPayment(Paypal_Dto request)
		{
			bool result = true;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PAYMENT_ID", request.PAYMENT_ID),
					new SqlParameter("@PAYMENT_STATE", request.PAYMENT_STATE),
				};
				result = _dbHelper.RunIntCmd(_query.Update_CancelPayment_Sql(), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}
	}
}