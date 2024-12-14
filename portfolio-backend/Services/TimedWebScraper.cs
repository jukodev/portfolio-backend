namespace portfolio_backend.Services;

public class TimedWebScraper (WebScraper scraper) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    private Timer? _timer;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(ScrapeWebsites, null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }
    
    private async void ScrapeWebsites(object? state)
    {
        try
        {
            var data = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            Console.WriteLine(data);
            // TODO save data to database
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to scrape website: " + e.Message);
        }
    }
    
    public override void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}