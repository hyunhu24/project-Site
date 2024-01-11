namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Order_Dto
    {
		public long? ORDER_IDX { get; set; }
		public string? ORDER_NUMBER { get; set; }
		public string? PAYMENT_ID { get; set; }
		public int? AMOUNT { get; set; }
		public int? ORDER_AMOUNT { get; set; }
		public int? QUANTITY { get; set; }
		public string? SHIPPING_ADDRESS { get; set; }
		public string? STATE_CODE { get; set; }
		public string? STATE_CODE_NAME { get; set; }
		public string? ORDER_TYPE_CODE { get; set; }
		public string? ORDER_TYPE_CODE_NAME { get; set; }
		public int? MEMBER_IDX { get; set; }
		public int? DEPOSIT_ACCOUNT_IDX { get; set; }
		public bool? IS_DEFAULT_ADDRESS { get; set; }
		public string? DEPOSIT_NAME { get; set; }
		public string? INS_DATE { get; set; }
		public string? MEMBER_KEY { get; set; }
		public string? MEMBER_NAME { get; set; }
		public string? MEMBER_ID { get; set; }
		public string? HP_NUMBER { get; set; }
		public long? PRODUCT_IDX { get; set; }
		public string? PRODUCT_NAME { get; set; }
		public string? THUMBNAIL_IMAGE_KEY { get; set; }
		public string? THUMBNAIL_IMAGE_URL { get; set; }
		public int? PRODUCT_AMOUNT { get; set; }
		public string? BANK_CODE { get; set; }
		public string? BANK_NAME { get; set; }
		public string? ACCOUNT_OWNER { get; set; }
		public string? ACCOUNT_NUMBER { get; set; }
		public string? START_DAY { get; set; }
		public string? END_DAY { get; set; }
		public int? PAGE { get; set; }
		public int? PAGE_SIZE { get; set; }
		public int? TOTAL_COUNT { get; set; }
		public string? COIN_ADDRESS {  get; set; }
		public string? TXID {  get; set; }
		public int? TOKEN_ID { get; set; }
		public long? NONCE { get; set; }
	}
}