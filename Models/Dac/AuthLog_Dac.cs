using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class AuthLog_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly AuthLog_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

        /// <summary>
        /// 인증 로그 저장
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		public bool Insert_AuthLog(AuthLog_Dto request)
        {
            bool result = false;

            try
            {
                List<SqlParameter> param = new()
                {
                    new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX),
                    new SqlParameter("@PURPOSE_CODE", request.PURPOSE_CODE),
                    new SqlParameter("@EMAIL_ADDRESS", request.EMAIL_ADDRESS),
                    new SqlParameter("@AUTH_CODE", request.AUTH_CODE),
                    new SqlParameter("@AUTH_SEND_KEY", request.AUTH_SEND_KEY),
					new SqlParameter("@TITLE", request.TITLE),
					new SqlParameter("@CONTENTS", request.CONTENTS),
					new SqlParameter("@EXP_DATE", request.EXP_DATE)
				};

                result = _dbHelper.RunIntCmd(_query.Insert_AuthLog_Sql(), param.ToArray()) == 1;
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
            }

            return result;
        }

		/// <summary>
		/// 인증 체크
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public long Select_AuthCheck(AuthLog_Dto request)
		{
			long result = 0;

			try
			{
				List<SqlParameter> param = new()
				{
                    new SqlParameter("@EMAIL_ADDRESS", request.EMAIL_ADDRESS),
                    new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX),
					new SqlParameter("@PURPOSE_CODE", request.PURPOSE_CODE),
					new SqlParameter("@AUTH_SEND_KEY", request.AUTH_SEND_KEY),
					new SqlParameter("@AUTH_CODE", request.AUTH_CODE)
				};

				result = Convert.ToInt64(_dbHelper.RunCmdScalar(_query.Select_AuthCheck_Sql(), param.ToArray()));
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}

		/// <summary>
		/// 인증 완료 처리
		/// </summary>
		/// <returns></returns>
		public bool Update_AuthLogComplete(AuthLog_Dto request)
		{
			bool result = false;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@LOG_IDX", request.LOG_IDX),
					new SqlParameter("@AUTH_COMPLETE_KEY", request.AUTH_COMPLETE_KEY)
				};

				result = _dbHelper.RunIntCmd(_query.Update_AuthLogComplete_Sql(), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}

		/// <summary>
		/// 인증 완료 번호 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public long Select_AuthCompleteIdx(AuthLog_Dto request)
		{
			long result = 0;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PURPOSE_CODE", request.PURPOSE_CODE),
                    new SqlParameter("@AUTH_SEND_KEY", request.AUTH_SEND_KEY),
                    new SqlParameter("@AUTH_COMPLETE_KEY", request.AUTH_COMPLETE_KEY),
					new SqlParameter("@EMAIL_ADDRESS", request.EMAIL_ADDRESS)
				};

				result = Convert.ToInt64(_dbHelper.RunCmdScalar(_query.Select_AuthCompleteIdx_Sql(), param.ToArray()));
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}

		/// <summary>
		/// 인증 완료 정보 삭제
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public void Delete_AuthLog(AuthLog_Dto request)
		{
			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@LOG_IDX", request.LOG_IDX)
				};

				_dbHelper.RunNonQueryCmd(_query.Delete_AuthLog_Sql(), param.ToArray());
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}
		}
	}
}