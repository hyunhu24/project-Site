using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Faq_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Faq_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// FAQ 목록 조회
		/// </summary>
		/// <returns></returns>
		public List<Faq_Dto> Select_FaqList()
		{
			List<Faq_Dto>? list = new();

			try
            {
                using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_FaqList_Sql());

                if (reader.HasRows)
                {	
					while (reader.Read())
                    {
						Faq_Dto info = new()
                        {
							QUESTION_CONTENTS = reader["QUESTION_CONTENTS"].ToString(),
							ANSWER_CONTENTS = reader["ANSWER_CONTENTS"].ToString(),
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
	}
}