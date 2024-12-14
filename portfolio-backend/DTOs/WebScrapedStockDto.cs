namespace portfolio_backend.DTOs;

public class WebScrapedStockDto
{
    public required float Price { get; set; }
    public required float Performance { get; set; }
    public required float PerformancePercentage { get; set; }
    public required string Currency { get; set; }
    public required string Time { get; set; }
    public required float Bid { get; set; }
    public required float Ask { get; set; }
    public required float YesterdayClose { get; set; }
    public required float TodayHigh { get; set; }
    public required float TodayLow { get; set; }
}