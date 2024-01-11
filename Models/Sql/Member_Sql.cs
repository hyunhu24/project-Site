namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Member_Sql
    {
		/// <summary>
		/// 아이디 중복 체크
		/// </summary>
		/// <returns></returns>
		public string Select_MemberIdDuplicateCheck_Sql()
		{
			string query = @"
				SELECT
					COUNT(*)
				FROM
					MEMBER_INFO (NOLOCK)
				WHERE
					MEMBER_ID = @MEMBER_ID
			";

			return query;
		}

		/// <summary>
		/// 회원 계정 체크
		/// </summary>
		/// <returns></returns>
		public string Select_MemberCheck_Sql()
        {
            string query = @"
				SELECT
					COUNT(*)
				FROM
					MEMBER_INFO (NOLOCK)
				WHERE
					    IS_DEL = 0
                    AND STATE_CODE = '0001'
					AND MEMBER_IDX = @MEMBER_IDX
			";

            return query;
        }

        /// <summary>
        /// 회원 정보 저장
        /// </summary>
        /// <returns></returns>
        public string Insert_MemberInfo_Sql()
        {
            string query = @"
                INSERT INTO MEMBER_INFO 
				(
					MEMBER_KEY,
					MEMBER_ID,
					MEMBER_PASSWORD,
					MEMBER_NAME,
					HP_NUMBER,
					SHIPPING_ADDRESS,
					COIN_ADDRESS
				)
				VALUES
				(
					@MEMBER_KEY,
					@MEMBER_ID,
					@MEMBER_PASSWORD,
					@MEMBER_NAME,
					@HP_NUMBER,
					@SHIPPING_ADDRESS,
					@COIN_ADDRESS
				);

				INSERT INTO MEMBER_HISTORY_SUM_INFO
				(
					MEMBER_IDX
				)
				VALUES
				(
					SCOPE_IDENTITY()
				)
            ";

            return query;
        }

        /// <summary>
        /// 회원 정보 조회
        /// </summary>
        /// <returns></returns>
        public string Select_MemberInfo_Sql(Member_Dto request)
        {
            string addQuery = @"";
            if (!string.IsNullOrEmpty(request.MEMBER_KEY)) addQuery += @" AND MEMBER_KEY = @MEMBER_KEY ";
            if (request.MEMBER_IDX.HasValue) addQuery += @" AND MEMBER_IDX = @MEMBER_IDX ";
            if (!string.IsNullOrEmpty(request.MEMBER_ID)) addQuery += @" AND MEMBER_ID = @MEMBER_ID ";
            if (!string.IsNullOrEmpty(request.MEMBER_PASSWORD)) addQuery += @" AND MEMBER_PASSWORD = @MEMBER_PASSWORD ";

            string query = @"
                SELECT
					MEMBER_IDX,
					MEMBER_ID,
                    MEMBER_KEY,
                    MEMBER_PASSWORD,
                    MEMBER_NAME,
                    HP_NUMBER,
                    SHIPPING_ADDRESS,
                    COIN_ADDRESS
				FROM
					MEMBER_INFO (NOLOCK)
				WHERE
                        IS_DEL = 0
					AND	STATE_CODE = '0001'
            ";
            query += addQuery;

            return query;
        }

		/// <summary>
		/// 회원 정보 변경
		/// </summary>
		/// <returns></returns>
		public string Update_MemberInfo_Sql(Member_Dto request)
		{
			string addQuery = @"";
			if (!string.IsNullOrEmpty(request.MEMBER_PASSWORD)) addQuery += @" , MEMBER_PASSWORD = @MEMBER_PASSWORD ";
			if (!string.IsNullOrEmpty(request.HP_NUMBER)) addQuery += @" , HP_NUMBER = @HP_NUMBER ";
			if (request.SHIPPING_ADDRESS != null) addQuery += @" , SHIPPING_ADDRESS = @SHIPPING_ADDRESS ";
			if (request.COIN_ADDRESS != null) addQuery += @" , COIN_ADDRESS = @COIN_ADDRESS ";

			string query = @"
                UPDATE
					MEMBER_INFO
				SET
					UPD_DATE = GETDATE()
			";
			query += addQuery;
			query += @"
				WHERE
						IS_DEL = 0
					AND	STATE_CODE = '0001'
            ";
			query += !string.IsNullOrEmpty(request.MEMBER_ID) ? @" AND MEMBER_ID = @MEMBER_ID " : @" AND MEMBER_IDX = @MEMBER_IDX ";

			return query;
		}

		/// <summary>
		/// 회원 탈퇴
		/// </summary>
		/// <returns></returns>
		public string Delete_MemberInfo_Sql()
		{
			string query = @"
                UPDATE
					MEMBER_INFO
				SET
					STATE_CODE = '0003',
					UPD_DATE = GETDATE(),
					IS_DEL = 1,
					PREV_MEMBER_ID = MEMBER_ID,
					MEMBER_ID = REPLACE(NEWID(), '-', '')
				WHERE
						IS_DEL = 0
					AND MEMBER_IDX = @MEMBER_IDX
            ";

			return query;
		}
	}
}