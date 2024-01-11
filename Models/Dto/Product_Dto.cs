namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Product_Dto
    {
        public int? PRODUCT_IDX { get; set; }
        public int? COMPANY_IDX { get; set; }
        public string? PRODUCT_NAME { get; set; }
        public string? MAIN_DESCRIPTION { get; set; }
        public string? DESCRIPTION { get; set; }
		public string? IMAGE_KEY { get; set; }
		public string? IMAGE_URL { get; set; }
		public string? THUMBNAIL_IMAGE_KEY { get; set; }
        public string? THUMBNAIL_IMAGE_URL { get; set; }
		public string? BACKGROUND_IMAGE_KEY { get; set; }
		public string? BACKGROUND_IMAGE_URL { get; set; }
		public string? DETAIL_IMAGE_KEY { get; set; }
		public string? DETAIL_IMAGE_URL { get; set; }
		public int? AMOUNT { get; set; }
        public int? QUANTITY { get; set; }
		public int? DEPOSIT_ACCOUNT_IDX { get; set; }
		public string? BANK_CODE { get; set; }
		public string? BANK_NAME { get; set; }
		public string? ACCOUNT_OWNER { get; set; }
		public string? ACCOUNT_NUMBER { get; set; }
        public bool? IS_SHOW { get; set; }
        public int? TURN { get; set; }
        public string? INS_DATE { get; set; }
        public string? UPD_DATE { get; set; }
        public string? SHIPPING_ADDRES { get; set; }
		public string? MEMBER_NAME { get; set; }
		public string? CONTRACT_ADDRESS { get; set; }
		public int? NFT_TOTAL_COUNT { get; set; }
        public int? NFT_MINTING_COUNT { get; set; }
        public int? NFT_REMAINS_COUNT { get; set; }
		public decimal? USD_AMOUNT { get; set; }
	}
}