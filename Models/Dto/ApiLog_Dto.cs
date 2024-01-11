namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ApiLog_Dto
	{
        public long? LOG_IDX { get; set; }
		public string? TYPE_CODE { get; set; }
        public string? API_URL { get; set; }
        public string? REQUEST_DATA { get; set; }
        public string? REQUEST_DATE { get; set; }
        public string? RESPONSE_DATA { get; set; } = string.Empty;
        public string? RESPONSE_DATE { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
		public string? REMOTE_IP_ADDRESS { get; set; }
	}
}