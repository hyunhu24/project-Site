namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class AuthLog_Sql
	{
        /// <summary>
        /// 인증 정보 저장
        /// </summary>
        /// <returns></returns>
        public string Insert_AuthLog_Sql()
        {
            string query = @"
				INSERT INTO AUTH_LOG
				(
					MEMBER_IDX,
					PURPOSE_CODE,
					EMAIL_ADDRESS,
					AUTH_CODE,
					AUTH_SEND_KEY,
					TITLE,
					CONTENTS,
					EXP_DATE
				)
				VALUES
				(
					@MEMBER_IDX,
					@PURPOSE_CODE,
					@EMAIL_ADDRESS,
					@AUTH_CODE,
					@AUTH_SEND_KEY,
					@TITLE,
					@CONTENTS,
					@EXP_DATE
				)
			";

            return query;
        }

		/// <summary>
		/// 인증 정보 체크
		/// </summary>
		/// <returns></returns>
		public string Select_AuthCheck_Sql()
		{
			string query = @"
				DECLARE @LOG_IDX BIGINT
				SET @LOG_IDX = 0

				SELECT
					@LOG_IDX = LOG_IDX
				FROM
					AUTH_LOG (NOLOCK)
				WHERE
						IS_AUTH_COMPLETE = 0
					AND EXP_DATE > GETDATE()
					AND MEMBER_IDX = @MEMBER_IDX
					AND EMAIL_ADDRESS = @EMAIL_ADDRESS
					AND AUTH_SEND_KEY = @AUTH_SEND_KEY
					AND AUTH_CODE = @AUTH_CODE
					AND PURPOSE_CODE = @PURPOSE_CODE

				IF @LOG_IDX > 0
					BEGIN
						DECLARE @AUTH_COUNT INT
						SET @AUTH_COUNT = 0

						SELECT
							@AUTH_COUNT = COUNT(*)
						FROM
							AUTH_LOG (NOLOCK)
						WHERE
								LOG_IDX > @LOG_IDX
							AND	IS_AUTH_COMPLETE = 0
							AND MEMBER_IDX = @MEMBER_IDX
							AND EMAIL_ADDRESS = @EMAIL_ADDRESS
							AND PURPOSE_CODE = @PURPOSE_CODE

						IF @AUTH_COUNT > 0
							BEGIN
								SET @LOG_IDX = 0
							END
					END

				SELECT @LOG_IDX
			";

			return query;
		}

		/// <summary>
		/// 인증 완료 처리
		/// </summary>
		/// <returns></returns>
		public string Update_AuthLogComplete_Sql()
		{
			string query = @"
                UPDATE
	                AUTH_LOG
                SET
					IS_AUTH_COMPLETE = 1,
	                AUTH_COMPLETE_DATE = GETDATE(),
					AUTH_COMPLETE_KEY = @AUTH_COMPLETE_KEY
                WHERE
					LOG_IDX = @LOG_IDX
            ";
			return query;
		}

		/// <summary>
		/// 인증 완료 번호 조회
		/// </summary>
		/// <returns></returns>
		public string Select_AuthCompleteIdx_Sql()
		{
			string query = @"
				DECLARE @LOG_IDX BIGINT

                SELECT
					@LOG_IDX = LOG_IDX
				FROM 
					AUTH_LOG (NOLOCK) 
				WHERE 
						AUTH_SEND_KEY = @AUTH_SEND_KEY
					AND AUTH_COMPLETE_KEY = @AUTH_COMPLETE_KEY
					AND EMAIL_ADDRESS = @EMAIL_ADDRESS
					AND PURPOSE_CODE = @PURPOSE_CODE
					AND IS_AUTH_COMPLETE = 1
					AND IS_DEL = 0

				SELECT ISNULL(@LOG_IDX, 0)
            ";
			return query;
		}

		/// <summary>
		/// 인증 완료 정보 삭제
		/// </summary>
		/// <returns></returns>
		public string Delete_AuthLog_Sql()
		{
			string query = @"
				UPDATE
					AUTH_LOG
				SET
					IS_DEL = 1
				WHERE
					LOG_IDX = @LOG_IDX
			";

			return query;
		}
	}
}