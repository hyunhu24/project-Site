namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
	public class Paypal_Dto
	{
		public long? ORDER_IDX { get; set; }
		public string? PAYMENT_ID { get; set; }
		public string? TOKEN { get; set; }
		public string? TRANSACTION_ID { get; set; }
		public decimal? AMOUNT { get; set; } 
		public string? CURRENCY { get; set; }
		public string? PAYMENT_TYPE { get; set; }
		public string? PAYMENT_STATE { get; set; }
		public string? STATE_CODE { get; set; }
		public string? FEE_CURRENT { get; set; }
		public decimal FEE_AMOUNT { get; set; }
		public string? MERCHANT_ID { get; set; }
	}
}