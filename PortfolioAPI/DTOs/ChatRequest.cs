namespace PortfolioAPI.DTOs
{
    public class ChatRequest
    {
        // Angular sends { message: "..." }
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        // Angular expects { reply: "..." }
        public string Reply { get; set; } = string.Empty;
    }
}