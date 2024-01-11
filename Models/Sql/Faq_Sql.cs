namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Faq_Sql
	{
		/// <summary>
		/// FAQ 목록 조회
		/// </summary>
		/// <returns></returns>
		public string Select_FaqList_Sql()
		{
			string query = @"
				SELECT
					REPLACE(QUESTION_CONTENTS, CHAR(10), '<br />') AS QUESTION_CONTENTS,
					REPLACE(ANSWER_CONTENTS, CHAR(10), '<br />') AS ANSWER_CONTENTS
				FROM 
					FAQ_INFO (NOLOCK)
				WHERE
						IS_DEL = 0
					AND IS_SHOW = 1
				ORDER BY
					TURN
				ASC,
					FAQ_IDX
				DESC
			";
				
			return query;
		}
	}
}