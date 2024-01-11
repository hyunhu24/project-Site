namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class AuthLog_Dto
	{
		public int? MEMBER_IDX { get; set; }
		public string? PURPOSE_CODE { get; set; }
		public string? EMAIL_ADDRESS { get; set; }
		public string? AUTH_CODE { get; set; }
		public string? AUTH_SEND_KEY { get; set; }
		public string? TITLE { get; set; }
		public string? CONTENTS { get; set; }
		public string? EXP_DATE { get; set; }
		public long? LOG_IDX { get; set; }
		public string? AUTH_COMPLETE_KEY { get; set; }
	}
}