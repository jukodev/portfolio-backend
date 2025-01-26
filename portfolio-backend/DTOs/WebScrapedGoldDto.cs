namespace portfolio_backend.DTOs;

public class WebScrapedGoldDto
{
    public required float Price { get; set; }
    public required float Performance { get; set; }
    public required float PerformancePercentage { get; set; }
    public required string Currency { get; set; }
    public required string Time { get; set; }
}