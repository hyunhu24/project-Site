using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Member_Dac
    {
		readonly DBHelper _dbHelper = new();
		readonly Member_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 아이디 중복 체크
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Select_MemberIdDuplicateCheck(Member_Dto request)
		{
			bool result = false;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@MEMBER_ID", request.MEMBER_ID)
				};

				result = Convert.ToInt32(_dbHelper.RunCmdScalar(_query.Select_MemberIdDuplicateCheck_Sql(), param.ToArray())) == 0;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}

		/// <summary>
		/// 계정 정상 여부 체크
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Select_MemberCheck(Member_Dto request)
        {
            bool result = false;

            try
            {
				List<SqlParameter> param = new();
				if (request.MEMBER_IDX.HasValue) param.Add(new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX));

				result = Convert.ToInt32(_dbHelper.RunCmdScalar(_query.Select_MemberCheck_Sql(), param.ToArray())) == 1;
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
            }

            return result;
        }

        /// <summary>
        /// 회원 정보 저장
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Insert_MemberInfo(Member_Dto request)
        {
            bool result = false;

            try
            {
                List<SqlParameter> param = new()
                {
                    new SqlParameter("@MEMBER_KEY", request.MEMBER_KEY),
                    new SqlParameter("@MEMBER_ID", request.MEMBER_ID),
                    new SqlParameter("@MEMBER_PASSWORD", request.MEMBER_PASSWORD),
                    new SqlParameter("@MEMBER_NAME", request.MEMBER_NAME),
                    new SqlParameter("@HP_NUMBER", request.HP_NUMBER),
                    new SqlParameter("@SHIPPING_ADDRESS", request.SHIPPING_ADDRESS ?? string.Empty),
                    new SqlParameter("@COIN_ADDRESS", request.COIN_ADDRESS ?? string.Empty)
                };

                result = _dbHelper.RunIntCmd(_query.Insert_MemberInfo_Sql(), param.ToArray()) == 2;
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
            }

            return result;
        }

        /// <summary>
		///	회원 정보 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Member_Dto Select_MemberInfo(Member_Dto request)
        {
            Member_Dto? response = null;

            try
            {
                List<SqlParameter> param = new();
                if (!string.IsNullOrEmpty(request.MEMBER_KEY)) param.Add(new SqlParameter("@MEMBER_KEY", request.MEMBER_KEY));
                if (request.MEMBER_IDX.HasValue) param.Add(new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX));
                if (!string.IsNullOrEmpty(request.MEMBER_ID)) param.Add(new SqlParameter("@MEMBER_ID", request.MEMBER_ID));
                if (!string.IsNullOrEmpty(request.MEMBER_PASSWORD)) param.Add(new SqlParameter("@MEMBER_PASSWORD", request.MEMBER_PASSWORD));

                using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_MemberInfo_Sql(request), param.ToArray());

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        response = new Member_Dto
                        {
                            MEMBER_IDX = Convert.ToInt32(reader["MEMBER_IDX"]),
                            MEMBER_ID = reader["MEMBER_ID"].ToString(),
                            MEMBER_KEY = reader["MEMBER_KEY"].ToString(),
							MEMBER_PASSWORD = reader["MEMBER_PASSWORD"].ToString(),
							MEMBER_NAME = reader["MEMBER_NAME"].ToString(),
							HP_NUMBER = reader["HP_NUMBER"].ToString(),
							SHIPPING_ADDRESS = reader["SHIPPING_ADDRESS"].ToString(),
							COIN_ADDRESS = reader["COIN_ADDRESS"].ToString(),
						};
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
            }

            return response!;
        }

		/// <summary>
		/// 회원 정보 변경
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Update_MemberInfo(Member_Dto request)
		{
			bool result = false;

			try
			{
				List<SqlParameter> param = new();
				if (!string.IsNullOrEmpty(request.MEMBER_PASSWORD)) param.Add(new SqlParameter("@MEMBER_PASSWORD", request.MEMBER_PASSWORD));
				if (!string.IsNullOrEmpty(request.HP_NUMBER)) param.Add(new SqlParameter("@HP_NUMBER", request.HP_NUMBER));
				if (request.SHIPPING_ADDRESS != null) param.Add(new SqlParameter("@SHIPPING_ADDRESS", request.SHIPPING_ADDRESS));
				if (request.COIN_ADDRESS != null) param.Add(new SqlParameter("@COIN_ADDRESS", request.COIN_ADDRESS));
				if (!string.IsNullOrEmpty(request.MEMBER_ID)) param.Add(new SqlParameter("@MEMBER_ID", request.MEMBER_ID));
                else param.Add(new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX));

				result = _dbHelper.RunIntCmd(_query.Update_MemberInfo_Sql(request), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}

		/// <summary>
		/// 회원 탈퇴
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public bool Delete_MemberInfo(Member_Dto request)
		{
			bool result = false;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX)
				};
				
				result = _dbHelper.RunIntCmd(_query.Delete_MemberInfo_Sql(), param.ToArray()) == 1;
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return result;
		}
	}
}