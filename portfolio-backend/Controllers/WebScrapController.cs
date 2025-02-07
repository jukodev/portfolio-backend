using Microsoft.AspNetCore.Mvc;
using portfolio_backend.DTOs;
using portfolio_backend.Services;

namespace portfolio_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapController(WebScraper scraper, ProxyService proxyService) : ControllerBase
    {
        [HttpGet("Test")]
        public async Task<WebScrapedStockDto> Get()
        {
            await proxyService.InitializeProxies();
            //var result = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            return null;
        }

        [HttpGet("Stock")]
        public async Task<ActionResult<WebScrapedStockDto>> GetStock(string url, string proxy)
        {
            try
            {
                var result = await scraper.ScrapStockData(url, proxy);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest("Failed to scrape website: " + e.Message);
            }
        }
        
        [HttpGet("Gold")]
        public async Task<ActionResult<WebScrapedGoldDto>> GetGold(string url, string proxy)
        {
            try
            {
                var result = await scraper.ScrapGoldData(url, proxy);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest("Failed to scrape website: " + e.Message);
            }
        }
    }
}