namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ExceptionLog_Sql
    {
        /// <summary>
        /// 오류 로그 저장
        /// </summary>
        /// <returns></returns>
        public string Insert_ExceptionLog_Sql()
        {
            string query = @"
                INSERT INTO EXCEPTION_LOG
				(
					PAGEFILE_NAME
					, EXCEPTION_TYPE
					, EXCEPTION_METHOD
					, EXCEPTION_LINE
					, EXCEPTION_STACKTRACE
					, EXCEPTION_MSG
				)
				VALUES
				(
					@PAGEFILE_NAME
					, @EXCEPTION_TYPE
					, @EXCEPTION_METHOD
					, @EXCEPTION_LINE
					, @EXCEPTION_STACKTRACE
					, @EXCEPTION_MSG
				)
            ";
            return query;
        }
    }
}