namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ApiLog_Sql
	{
        /// <summary>
        /// API 로그 저장
        /// </summary>
        /// <returns></returns>
        public string Insert_ApiLog_Sql()
        {
            string query = @"
                INSERT INTO API_LOG
				(
					TYPE_CODE,
					API_URL,
					REQUEST_DATA,
					REQUEST_DATE,
					RESPONSE_DATA,
					RESPONSE_DATE,
					REMOTE_IP_ADDRESS
				)
				VALUES
				(
					@TYPE_CODE,
					@API_URL,
					@REQUEST_DATA,
					@REQUEST_DATE,
					@RESPONSE_DATA,
					@RESPONSE_DATE,
					@REMOTE_IP_ADDRESS
				);

				SELECT SCOPE_IDENTITY()
            ";
            return query;
        }

		/// <summary>
		/// API 로그 변경
		/// </summary>
		/// <returns></returns>
		public string Update_ApiLog_Sql()
		{
			string query = @"
				UPDATE
					API_LOG
				SET
					RESPONSE_DATA = @RESPONSE_DATA,
					RESPONSE_DATE = @RESPONSE_DATE
				WHERE
					LOG_IDX = @LOG_IDX
            ";
			return query;
		}
	}
}