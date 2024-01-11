using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Setting_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Setting_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 민팅 설정 정보 조회
		/// </summary>
		/// <returns></returns>
		public Setting_Dto Select_MintingSettingInfo()
        {
			Setting_Dto? response = null;

			try
			{
				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_MintingSettingInfo_Sql());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new Setting_Dto
						{
							MOTHER_PUBLIC_KEY = reader["MOTHER_PUBLIC_KEY"].ToString(),
							MOTHER_PRIVATE_KEY = Encrypter.Decrypt(reader["MOTHER_PRIVATE_KEY"].ToString()!),
							INFURA_URL = reader["INFURA_URL"].ToString(),
							NONCE = Convert.ToInt64(reader["NONCE"])
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
        /// 민팅 거래 순번 정보 변경
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void Update_MintingNonceInfo(Setting_Dto request)
        {
            try
            {
                List<SqlParameter> param = new()
                {
                    new SqlParameter("@NONCE", request.NONCE)
                };

                _dbHelper.RunNonQueryCmd(_query.Update_MintingNonceInfo_Sql(), param.ToArray());
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
            }
        }
    }
}