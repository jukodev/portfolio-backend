using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portfolio_backend.Database;
using portfolio_backend.DTOs;
using portfolio_backend.Services;
using portfolio_backend.Utils;

namespace portfolio_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapController(WebScraper scraper) : ControllerBase
    {
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

        [HttpGet("Misc")]
        public async Task<ActionResult<string>> GetMisc(string url)
        {
            try
            {
                var result = await scraper.GetHtml(url);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest("Failed to scrape website: " + e.Message);
            }
        }

        [HttpGet("Proxy")]
        public ActionResult<Dictionary<string, int>> GetProxy()
        {
            var ipCounts = WebScraper.FailedProxys
            .GroupBy(ip => ip)
            .ToDictionary(group => group.Key, group => group.Count());
            return ipCounts;
        }
    }
}