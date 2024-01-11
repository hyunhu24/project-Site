namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Setting_Sql
	{
		/// <summary>
		/// 민팅 설정 정보 조회
		/// </summary>
		/// <returns></returns>
		public string Select_MintingSettingInfo_Sql()
		{
			string query = @"
				SELECT TOP 1
                    MOTHER_PUBLIC_KEY,
	                MOTHER_PRIVATE_KEY,
                    INFURA_URL,
					NONCE
                FROM 
	                MINTING_SETTING_INFO (NOLOCK)
                ORDER BY
                    SETTING_IDX
                DESC
			";

			return query;
		}

        /// <summary>
        /// 민팅 거래 순번 정보 변경
        /// </summary>
        /// <returns></returns>
        public string Update_MintingNonceInfo_Sql()
        {
            string query = @"
				UPDATE 
	                MINTING_SETTING_INFO 
                SET
                    NONCE = @NONCE,
                    UPD_DATE = GETDATE()
			";

            return query;
        }
    }
}