namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class ExceptionLog_Dto
    {
        public string? PAGEFILE_NAME { get; set; }
        public string? EXCEPTION_TYPE { get; set; }
        public string? EXCEPTION_METHOD { get; set; }
        public int? EXCEPTION_LINE { get; set; }
        public string? EXCEPTION_STACKTRACE { get; set; }
        public string? EXCEPTION_MSG { get; set; }
    }
}