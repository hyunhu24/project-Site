using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ApiLog_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly ApiLog_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// API 로그 저장
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public void Insert_ApiLog(ApiLog_Dto request)
		{
			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@TYPE_CODE", request.TYPE_CODE),
					new SqlParameter("@API_URL", request.API_URL),
					new SqlParameter("@REQUEST_DATA", request.REQUEST_DATA),
					new SqlParameter("@REQUEST_DATE", request.REQUEST_DATE),
					new SqlParameter("@RESPONSE_DATA", request.RESPONSE_DATA),
					new SqlParameter("@RESPONSE_DATE", request.RESPONSE_DATE),
					new SqlParameter("@REMOTE_IP_ADDRESS", request.REMOTE_IP_ADDRESS ?? string.Empty)
				};

				_dbHelper.RunNonQueryCmd(_query.Insert_ApiLog_Sql(), param.ToArray());
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}
		}
	}
}