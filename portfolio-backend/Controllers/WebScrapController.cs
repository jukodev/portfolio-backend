using Microsoft.AspNetCore.Mvc;
using portfolio_backend.DTOs;
using portfolio_backend.Services;

namespace portfolio_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapController (WebScraper scraper) : ControllerBase 
    {
        [HttpGet("Test")]
        public async Task<WebScrapedStockDto> Get()
        {
            var result = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            return result;
        }

        [HttpGet("Stock")]
        public async Task<WebScrapedStockDto> GetStock(string url)
        {
            var result = await scraper.ScrapStockData(url);
            return result;
        }
    }
}
