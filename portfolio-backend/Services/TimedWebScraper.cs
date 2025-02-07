namespace portfolio_backend.Services;

public class TimedWebScraper (WebScraper scraper, ProxyService proxyService) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    private Timer? _scrapeTimer, _proxyTimer;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Timed Web Scraper");
        _scrapeTimer = new Timer(_ => Task.Run(() => ScrapeWebsites(null)), null, TimeSpan.Zero, _interval);
        _proxyTimer = new Timer(_ => Task.Run(() => ValidateProxies(null)), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        
        return Task.CompletedTask;
    }
    
    private async Task ScrapeWebsites(object? state)
    {
        return;
        try
        {
            //var data = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            //Console.WriteLine(data);
            // TODO save data to database
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to scrape website: " + e.Message);
        }
    }
    
    private async Task ValidateProxies(object? state)
    {
        Console.WriteLine("Validating proxies");
        await proxyService.ValidateProxies();
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