namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Product_Sql
	{
		/// <summary>
		///  제품 목록 조회(메인페이지)
		/// </summary>
		/// <returns></returns>
		public string Select_ProductList_Sql()
		{
			string query = @"
				SELECT
					PRODUCT_IDX,
					COMPANY_IDX,
					PRODUCT_NAME,
					REPLACE(MAIN_DESCRIPTION, CHAR(10), '<br />') AS MAIN_DESCRIPTION,
					BACKGROUND_IMAGE_KEY,
					THUMBNAIL_IMAGE_KEY,
					AMOUNT,
					TURN
				FROM 
					PRODUCT_INFO (NOLOCK)
				WHERE
						IS_DEL = 0
					AND IS_SHOW = 1
				ORDER BY 
					TURN 
				ASC
			";
				
			return query;
		}

		/// <summary>
		/// 제품 정보 조회
		/// </summary>
		/// <returns></returns>
		public string Select_ProductInfo_Sql()
		{
			string queyr = @"
				DECLARE @KRW_USD DECIMAL(20, 8)
				SET @KRW_USD = (
					SELECT
						KRW_USD
					FROM 
						EXCHANGE_RATE_INFO (NOLOCK)
				)

				SELECT
					PROD.PRODUCT_IDX,
					PROD.COMPANY_IDX,
					PROD.PRODUCT_NAME,
					REPLACE([DESCRIPTION], CHAR(10), '<br />') AS DESCRIPTION,
					PROD.IMAGE_KEY,
					PROD.THUMBNAIL_IMAGE_KEY,
					PROD.BACKGROUND_IMAGE_KEY,
					PROD.DETAIL_IMAGE_KEY,
					PROD.AMOUNT,
					ROUND(PROD.AMOUNT * @KRW_USD, 2) AS USD_AMOUNT,
					PROD.DEPOSIT_ACCOUNT_IDX,
					PROD.CONTRACT_ADDRESS,
					DEPO.BANK_CODE,
					CODE.MIDDLE_CODE_NAME AS BANK_NAME,
					DEPO.ACCOUNT_OWNER,
					DEPO.ACCOUNT_NUMBER,
					PROD.NFT_TOTAL_COUNT,
					PROD.NFT_MINTING_COUNT
				FROM 
					PRODUCT_INFO AS PROD (NOLOCK)
				INNER JOIN 
					DEPOSIT_ACCOUNT_INFO AS DEPO (NOLOCK)
						ON DEPO.IS_DEL = 0
						AND DEPO.COMPANY_IDX = PROD.COMPANY_IDX
				INNER JOIN 
					COMMON_CODE_INFO AS CODE (NOLOCK)
						ON CODE.MAIN_CODE = '0001'
						AND CODE.MIDDLE_CODE = DEPO.BANK_CODE
				WHERE
						PROD.IS_DEL = 0
					AND PROD.PRODUCT_IDX = @PRODUCT_IDX
			";	

			return queyr;
		}

		/// <summary>
		/// 민팅 개수 증가
		/// </summary>
		/// <returns></returns>
		public string Update_MintingCount_Sql()
		{
			string query = @"
				UPDATE
					PRODUCT_INFO
				SET
					NFT_MINTING_COUNT = NFT_MINTING_COUNT + 1,
					UPD_DATE = GETDATE()
				WHERE
					PRODUCT_IDX = @PRODUCT_IDX
			";

			return query;
		}
	}
}