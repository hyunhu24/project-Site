using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Nft_Dac
    {
        readonly DBHelper _dbHelper = new();
        readonly Nft_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// NFT 메타데이터용 정보 조회
		/// </summary>
		/// <param name="contractAddress"></param>
		/// <param name="tokenId"></param>
		/// <returns></returns>
		public Nft_Dto Select_NftMetadataInfo(string contractAddress, int tokenId)
		{
			Nft_Dto? response = null;

			try
			{
				List<SqlParameter> param = new()
				{ 
					new SqlParameter("@CONTRACT_ADDRESS", contractAddress),
					new SqlParameter("@TOKEN_ID", tokenId)
				};

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_NftMetadataInfo_Sql(), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new()
						{
							NFT_PREFIX_NAME = reader["NFT_PREFIX_NAME"].ToString(),
							NFT_DESCRIPTION = reader["NFT_DESCRIPTION"].ToString(),
							IMAGE_KEY = reader["IMAGE_KEY"].ToString(),
							EXTERNAL_LINK = reader["EXTERNAL_LINK"].ToString(),
							IS_NFT_VIDEO = Convert.ToBoolean(reader["IS_NFT_VIDEO"])
						};

						response.IMAGE_URL = Function.SetImageUrl(string.Format("nft/{0}", contractAddress), response.IMAGE_KEY!, (bool)response.IS_NFT_VIDEO);
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