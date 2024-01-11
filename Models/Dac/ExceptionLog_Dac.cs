using System.Data.SqlClient;
using System.Diagnostics;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ExceptionLog_Dac
    {
		readonly DBHelper _dbHelper = new();
		readonly ExceptionLog_Sql _query = new();

        /// <summary>
        /// 오류 로그
        /// </summary>
        /// <param name="ex"></param>
        public void Insert_ExceptionLog(Exception ex)
        {
			StackTrace st = new(ex, true);
			StackFrame frame = st.GetFrame(0)!;

			ExceptionLog_Dto info = new()
			{
				PAGEFILE_NAME = frame != null ? frame.GetFileName() : string.Empty,
				EXCEPTION_TYPE = ex.GetType().ToString(),
				EXCEPTION_METHOD = frame != null ? frame.GetMethod()!.Name : string.Empty,
				EXCEPTION_LINE = frame != null ? frame.GetFileLineNumber() : 0,
				EXCEPTION_STACKTRACE = ex.StackTrace,
				EXCEPTION_MSG = ex.Message
			};

			List<SqlParameter> param = new()
			{
				new SqlParameter("@PAGEFILE_NAME", info.PAGEFILE_NAME ?? string.Empty),
				new SqlParameter("@EXCEPTION_TYPE", info.EXCEPTION_TYPE ?? string.Empty),
				new SqlParameter("@EXCEPTION_METHOD", info.EXCEPTION_METHOD ?? string.Empty),
				new SqlParameter("@EXCEPTION_LINE", info.EXCEPTION_LINE.HasValue ? info.EXCEPTION_LINE : 0),
				new SqlParameter("@EXCEPTION_STACKTRACE", info.EXCEPTION_STACKTRACE ?? string.Empty),
				new SqlParameter("@EXCEPTION_MSG", info.EXCEPTION_MSG ?? string.Empty)
			};

			_dbHelper.RunNonQueryCmd(_query.Insert_ExceptionLog_Sql(), param.ToArray());          
        }
    }
}