namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Nft_Sql
    {
		/// <summary>
		/// NFT 메타데이터용 정보 조회
		/// </summary>
		/// <returns></returns>
		public string Select_NftMetadataInfo_Sql()
		{
			string query = @"
				SELECT
					PRODUCT.NFT_PREFIX_NAME,
					PRODUCT.NFT_DESCRIPTION,
					NFT.IMAGE_KEY,
					ISNULL(PRODUCT.EXTERNAL_LINK, '') AS EXTERNAL_LINK,
					PRODUCT.IS_NFT_VIDEO
				FROM
					NFT_INFO AS NFT (NOLOCK)
				INNER JOIN
					PRODUCT_INFO AS PRODUCT (NOLOCK)
						ON	PRODUCT.PRODUCT_IDX = NFT.PRODUCT_IDX
						AND	PRODUCT.CONTRACT_ADDRESS = @CONTRACT_ADDRESS
				WHERE 
						NFT.IS_DEL = 0
					AND NFT.TOKEN_ID = @TOKEN_ID
			";

			return query;
		}
	}
}