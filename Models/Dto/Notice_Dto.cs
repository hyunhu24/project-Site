namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Notice_Dto
	{
		public int? PAGE {  get; set; }
		public int? PAGE_SIZE { get; set; }
		public int? TOTAL_COUNT {  get; set; }
		public int? NOTICE_IDX { get; set; }
        public string? TITLE { get; set; }
		public string? CONTENTS { get; set; }
		public string? INS_DATE { get; set; }
		public string? DATE { get; set; }
	}
}