using Microsoft.AspNetCore.Mvc;
using portfolio_backend.DTOs;
using portfolio_backend.Services;

namespace portfolio_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapController(WebScraper scraper) : ControllerBase
    {
        [HttpGet("Test")]
        public async Task<WebScrapedStockDto> Get()
        {
            var result = await scraper.ScrapStockData("https://www.boerse.de/realtime-kurse/Apple-Aktie/US0378331005");
            return result;
        }

        [HttpGet("Stock")]
        public async Task<ActionResult<WebScrapedStockDto>> GetStock(string url)
        {
            try
            {
                var result = await scraper.ScrapStockData(url);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest("Failed to scrape website: " + e.Message);
            }
        }
        
        [HttpGet("Gold")]
        public async Task<ActionResult<WebScrapedGoldDto>> GetGold(string url)
        {
            try
            {
                var result = await scraper.ScrapGoldData(url);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest("Failed to scrape website: " + e.Message);
            }
        }
    }
}