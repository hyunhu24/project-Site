using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Product_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Product_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

        /// <summary>
        /// 제품 목록 조회
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		public List<Product_Dto> Select_ProductList()
		{
			List<Product_Dto>? list = new();

			try
            {
                using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_ProductList_Sql());

                if (reader.HasRows)
                {					
					while (reader.Read())
                    {
						Product_Dto info = new()
                        {
                            PRODUCT_IDX = Convert.ToInt32(reader["PRODUCT_IDX"]),
                            COMPANY_IDX = Convert.ToInt32(reader["COMPANY_IDX"]),
                            PRODUCT_NAME = reader["PRODUCT_NAME"].ToString(),
                            MAIN_DESCRIPTION = reader["MAIN_DESCRIPTION"].ToString(),
                            THUMBNAIL_IMAGE_KEY = reader["THUMBNAIL_IMAGE_KEY"].ToString(),
							BACKGROUND_IMAGE_KEY = reader["BACKGROUND_IMAGE_KEY"].ToString(),
							AMOUNT = Convert.ToInt32(reader["AMOUNT"]),
                            TURN = Convert.ToInt32(reader["TURN"]),
                        };
						info.THUMBNAIL_IMAGE_URL = Function.SetFileUrl("product/thumbnail", info.THUMBNAIL_IMAGE_KEY!);
						info.BACKGROUND_IMAGE_URL = Function.SetImageUrl("product/background", info.BACKGROUND_IMAGE_KEY!);

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
        /// 제품 정보 조회
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Product_Dto Select_ProductInfo(Product_Dto request)
        {
			Product_Dto? response = null;

			try
			{
                List<SqlParameter> param = new()
                {
					new SqlParameter("@PRODUCT_IDX", request.PRODUCT_IDX)
				};

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_ProductInfo_Sql(), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new Product_Dto
                        {
                            PRODUCT_IDX = Convert.ToInt32(reader["PRODUCT_IDX"]),
                            COMPANY_IDX = Convert.ToInt32(reader["COMPANY_IDX"]),
                            PRODUCT_NAME = reader["PRODUCT_NAME"].ToString(),
                            DESCRIPTION = reader["DESCRIPTION"].ToString(),
							IMAGE_KEY = reader["IMAGE_KEY"].ToString(),
							THUMBNAIL_IMAGE_KEY = reader["THUMBNAIL_IMAGE_KEY"].ToString(),
							BACKGROUND_IMAGE_KEY = reader["BACKGROUND_IMAGE_KEY"].ToString(),
							DETAIL_IMAGE_KEY = reader["DETAIL_IMAGE_KEY"].ToString(),
							AMOUNT = Convert.ToInt32(reader["AMOUNT"]),
							USD_AMOUNT = Convert.ToDecimal(reader["USD_AMOUNT"]),
							DEPOSIT_ACCOUNT_IDX = Convert.ToInt32(reader["DEPOSIT_ACCOUNT_IDX"]),
                            BANK_CODE = reader["BANK_CODE"].ToString(),
                            CONTRACT_ADDRESS = reader["CONTRACT_ADDRESS"].ToString(),
							BANK_NAME = reader["BANK_NAME"].ToString(),
							ACCOUNT_OWNER = reader["ACCOUNT_OWNER"].ToString(),
							ACCOUNT_NUMBER = reader["ACCOUNT_NUMBER"].ToString(),
							NFT_TOTAL_COUNT = Convert.ToInt32(reader["NFT_TOTAL_COUNT"]),
							NFT_MINTING_COUNT = Convert.ToInt32(reader["NFT_MINTING_COUNT"])
                        };
						response.NFT_REMAINS_COUNT = response.NFT_TOTAL_COUNT - response.NFT_MINTING_COUNT;
                        response.IMAGE_URL = Function.SetImageUrl("product/info", response.IMAGE_KEY!, true);
						response.THUMBNAIL_IMAGE_URL = Function.SetImageUrl("product/thumbnail", response.THUMBNAIL_IMAGE_KEY!);
                        response.BACKGROUND_IMAGE_URL = Function.SetImageUrl("product/background", response.BACKGROUND_IMAGE_KEY!);
						response.DETAIL_IMAGE_URL = Function.SetImageUrl("product/detail", response.DETAIL_IMAGE_KEY!);
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
		/// 민팅 개수 증가
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public void Update_MintingCount(Product_Dto request)
		{
			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PRODUCT_IDX", request.PRODUCT_IDX)
				};

				_dbHelper.RunNonQueryCmd(_query.Update_MintingCount_Sql(), param.ToArray());
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}
		}
	}
}