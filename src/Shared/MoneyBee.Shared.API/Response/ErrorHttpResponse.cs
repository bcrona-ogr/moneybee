namespace MoneyBee.Shared.API.Response
{
    public  class ErrorHttpResponse
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int StatusCode { get; set; }
        public string CorrelationId { get; set; }
        public string Detail { get; set; }
    }
}