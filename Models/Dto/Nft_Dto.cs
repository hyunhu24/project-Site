namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Nft_Dto
    {
        public string? NFT_PREFIX_NAME { get; set; }
        public string? NFT_DESCRIPTION { get; set; }
		public string? IMAGE_KEY { get; set; }
        public string? IMAGE_URL { get; set; }
		public string? EXTERNAL_LINK { get; set; }
		public int? TOKEN_ID { get; set; }
        public bool? IS_NFT_VIDEO { get; set; }
	}
}