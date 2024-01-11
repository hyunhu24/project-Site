using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Notice_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Notice_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 메인 공지사항 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Notice_Dto Select_MainNoticeInfo(Notice_Dto request)
		{
			Notice_Dto? response = null;

			try
			{
				List<SqlParameter> param = new();
				if (!string.IsNullOrEmpty(request.DATE)) param.Add(new SqlParameter("@DATE", request.DATE));

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_MainNoticeInfo_Sql(), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new Notice_Dto
						{
							NOTICE_IDX = Convert.ToInt32(reader["NOTICE_IDX"]),
							TITLE = reader["TITLE"].ToString(),
							CONTENTS = reader["CONTENTS"].ToString(),
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
		/// 공지사항 목록 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public List<Notice_Dto> Select_NoticeList(Notice_Dto request)
		{
			List<Notice_Dto>? list = null;

			try
            {
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PAGE", request.PAGE),
					new SqlParameter("@PAGE_SIZE", request.PAGE_SIZE)
				};

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_NoticeList_Sql(), param.ToArray());

                if (reader.HasRows)
                {
					list = new();
					while (reader.Read())
                    {
						Notice_Dto info = new()
                        {
							NOTICE_IDX = Convert.ToInt32(reader["NOTICE_IDX"]),
							TITLE = reader["TITLE"].ToString(),
							INS_DATE = reader["INS_DATE"].ToString(),
							TOTAL_COUNT = Convert.ToInt32(reader["TOTAL_COUNT"])
						};

                        list.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
				throw;
			}

            return list!;
        }

		/// <summary>
		/// 공지사항 상세 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Notice_Dto Select_NoticeInfo(Notice_Dto request)
        {
			Notice_Dto? response = null;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@NOTICE_IDX", request.NOTICE_IDX)
				};

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_NoticeInfo_Sql(), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new Notice_Dto
						{
							TITLE = reader["TITLE"].ToString(),
							CONTENTS = reader["CONTENTS"].ToString(),
							INS_DATE = reader["INS_DATE"].ToString()
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
	}
}