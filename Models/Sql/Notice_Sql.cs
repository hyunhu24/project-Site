namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Notice_Sql
	{
		/// <summary>
		/// 메인 공지사항 조회
		/// </summary>
		/// <returns></returns>
		public string Select_MainNoticeInfo_Sql()
		{
			string query = @"
				SELECT
					TOP(1)
					NOTICE_IDX,
					TITLE,
					CONTENTS
				FROM
					MAIN_NOTICE_INFO (NOLOCK)
				WHERE 
						IS_DEL = 0
					AND IS_SHOW = 1
					AND START_DATE <= @DATE 
					AND END_DATE >= @DATE
				ORDER BY
					NOTICE_IDX 
				DESC
			";

			return query;
		}

		/// <summary>
		/// 공지사항 목록 조회
		/// </summary>
		/// <returns></returns>
		public string Select_NoticeList_Sql()
		{
			string query = @"
				WITH LIST AS
				(
					SELECT
						NOTICE_IDX,
						TITLE,
						INS_DATE,
						ROW_NUMBER() OVER(ORDER BY NOTICE_IDX DESC) AS ROW_NUM
					FROM
						NOTICE_INFO (NOLOCK)
					WHERE 
							IS_SHOW = 1
						AND IS_DEL = 0
				)
				SELECT
					*,
					(SELECT COUNT(*) FROM LIST WITH(NOLOCK)) AS TOTAL_COUNT
				FROM
					LIST WITH(NOLOCK)
				WHERE
						LIST.ROW_NUM BETWEEN (@PAGE - 1) * @PAGE_SIZE + 1 
					AND @PAGE * @PAGE_SIZE
				ORDER BY
					LIST.ROW_NUM
				ASC
			";
				
			return query;
		}

		/// <summary>
		/// 공지사항 상세 조회
		/// </summary>
		/// <returns></returns>
		public string Select_NoticeInfo_Sql()
		{
			string query = @"
				SELECT
					TITLE,
					REPLACE(CONTENTS, CHAR(10), '<br />') AS CONTENTS,
					INS_DATE
				FROM 
					NOTICE_INFO (NOLOCK)
				WHERE
						IS_SHOW = 1
					AND IS_DEL = 0
					AND NOTICE_IDX = @NOTICE_IDX
			";

			return query;
		}
	}
}