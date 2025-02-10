namespace portfolio_backend.Services;

public class TimedWebScraper (WebScraper scraper, ProxyService proxyService) : BackgroundService
{
    private Timer? _scrapeTimer;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _scrapeTimer = new Timer(_ => Task.Run(() => ScrapeWebsites(null)), null, TimeSpan.FromDays(1), TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }
    
    private async Task ScrapeWebsites(object? _)
    {
        try
        {
            var data = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            // TODO save data to database
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to scrape website: " + e.Message);
        }
    }
    
    
    public override void Dispose()
    {
        _scrapeTimer?.Change(Timeout.Infinite, 0);
        _scrapeTimer?.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}