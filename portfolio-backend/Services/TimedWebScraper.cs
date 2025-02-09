namespace portfolio_backend.Services;

public class TimedWebScraper (WebScraper scraper, ProxyService proxyService) : BackgroundService
{
    private Timer? _scrapeTimer, _proxyTimer;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Timed Web Scraper");
        _scrapeTimer = new Timer(_ => Task.Run(() => ScrapeWebsites(null)), null, TimeSpan.FromDays(1), TimeSpan.FromDays(1));
        _proxyTimer = new Timer(_ => Task.Run(() => ValidateProxies(null)), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));

        
        return Task.CompletedTask;
    }
    
    private async Task ScrapeWebsites(object? _)
    {
        Console.WriteLine("whut");
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
    
    private void ValidateProxies(object? _)
    {
        return;
        Console.WriteLine("Validating proxies");
        proxyService.InitializeProxies();
    }
    
    public override void Dispose()
    {
        _scrapeTimer?.Change(Timeout.Infinite, 0);
        _scrapeTimer?.Dispose();
        _proxyTimer?.Change(Timeout.Infinite, 0);
        _proxyTimer?.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}