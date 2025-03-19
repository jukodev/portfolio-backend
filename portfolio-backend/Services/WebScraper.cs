using System.Globalization;
using portfolio_backend.DTOs;
using portfolio_backend.Utils;
using PuppeteerSharp;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace portfolio_backend.Services;

public partial class WebScraper(ProxyService proxyService, ILogger<WebScraper> logger)
{

    public async Task<WebScrapedStockDto> ScrapStockData(string url)
    {
        var html = await GetHtml(url);

        if (html.Contains("Sorry, you have been blocked"))
        {
            logger.LogWarning("Cloudflare Block detected for URL: {Url}", url);
            throw new Exception("Cloudflare Block");
        }

        var indiceFirst = html.IndexOf("<!-- KURSE -->", StringComparison.Ordinal);
        var indiceLast = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html[indiceFirst..indiceLast];

        var lastPrice =
            ExtractFloatAfterTag(htmlKurse, "itemprop=\"price\" content=", CultureInfo.InvariantCulture);

        var lastPerformance = ExtractFloatAfterTag(htmlKurse, "data-push-attribute=\"perfInstAbs",
            CultureInfo.InvariantCulture);

        var lastPerformancePercent = ExtractFloatAfterTag(htmlKurse, "data-push-attribute=\"perfInstRel",
            CultureInfo.InvariantCulture);

        var priceCurrency = ExtractStringAfterTag(htmlKurse, "itemprop=\"priceCurrency\" >", "</span>");

        var time = ExtractStringAfterTag(htmlKurse, "data-push-attribute=\"timestamp\">", "</span>");

        var dateRaw = ExtractStringAfterTag(htmlKurse, "data-push-attribute=\"date\">", "</span>");
        var shortDd = dateRaw[..2];
        var shortMm = dateRaw.Substring(3, 2);
        var shortYy = dateRaw.Substring(6, 2);
        var lastTime = "20" + shortYy + "-" + shortMm + "-" + shortDd + " " + time;

        var lastBid = ExtractFloatAfterTag(html, "data-push-attribute=\"bid\"", CultureInfo.InvariantCulture);

        var lastAsk = ExtractFloatAfterTag(html, "data-push-attribute=\"ask\"", CultureInfo.InvariantCulture);

        var yesterdayClose =
            ExtractFloatAfterTag(html, "data-push-attribute=\"close\"", CultureInfo.InvariantCulture);

        var todayHigh = ExtractFloatAfterTag(html, "data-push-attribute=\"high\"", CultureInfo.InvariantCulture);

        var todayLow = ExtractFloatAfterTag(html, "data-push-attribute=\"low\"", CultureInfo.InvariantCulture);

        return new WebScrapedStockDto
        {
            Price = lastPrice,
            Performance = lastPerformance,
            PerformancePercentage = lastPerformancePercent,
            Currency = priceCurrency,
            Time = lastTime,
            Bid = lastBid,
            Ask = lastAsk,
            YesterdayClose = yesterdayClose,
            TodayHigh = todayHigh,
            TodayLow = todayLow
        };
    }

    public async Task<WebScrapedGoldDto> ScrapGoldData(string url)
    {
        var html = await GetHtml(url);

        var start = html.IndexOf("<!-- KURSE EURO -->", StringComparison.Ordinal);
        var end = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html[start..end];

        var lastPrice = ExtractFloatAfterTag(htmlKurse, "<span", CultureInfo.InvariantCulture);

        var afterPrice = AdvancePastSpan(htmlKurse, "<span", "</span>");
        var currency = ExtractValueBetweenTags(afterPrice, "<span", "</span>");
        var afterCurrency = AdvancePastSpan(afterPrice, "<span", "</span>");

        var lastPerformance = ExtractFloatAfterTag(afterCurrency, "<span", CultureInfo.InvariantCulture);
        var afterPerf = AdvancePastSpan(afterCurrency, "<span", "</span>");

        var timeLabel = "Zeit ";
        var timeIndex = afterPerf.IndexOf(timeLabel, StringComparison.Ordinal);
        var timeEnd = afterPerf.IndexOf("</div>", timeIndex, StringComparison.Ordinal);
        var time = afterPerf[(timeIndex + timeLabel.Length)..timeEnd].Trim();
        var afterTime = afterPerf[(timeEnd + 6)..];

        var lastPerformancePercent = ExtractFloatAfterTag(afterTime, "<span", CultureInfo.InvariantCulture);
        var afterPerfPercent = AdvancePastSpan(afterTime, "<span", "</span>");

        const string dateLabel = "Datum&nbsp;";
        var dateIndex = afterPerfPercent.IndexOf(dateLabel, StringComparison.Ordinal);
        var dateRaw = afterPerfPercent.Substring(dateIndex + dateLabel.Length, 8);

        var day = dateRaw[..2];
        var month = dateRaw.Substring(3, 2);
        var year = dateRaw.Substring(6, 2);

        var lastTime = $"20{year}-{month}-{day} {time}";

        var dto = new WebScrapedGoldDto()
        {
            Price = lastPrice,
            Performance = lastPerformance,
            PerformancePercentage = lastPerformancePercent,
            Currency = currency,
            Time = lastTime
        };

        return dto;
    }

    private static string AdvancePastSpan(string source, string startTag, string endTag)
    {
        var startIndex = source.IndexOf(startTag, StringComparison.Ordinal);
        if (startIndex < 0) return source;
        var endIndex = source.IndexOf(endTag, startIndex, StringComparison.Ordinal);
        return endIndex < 0 ? source : source[(endIndex + endTag.Length)..];
    }


    private static float ExtractFloatAfterTag(string source, string startTag, IFormatProvider culture)
    {
        var val = ExtractValueBetweenTags(source, startTag, "</span>");
        val = val.Replace(".", "").Replace(',', '.');
        return float.TryParse(val, NumberStyles.Any, culture, out var result) ? result : 0.0f;
    }

    private static string ExtractStringAfterTag(string source, string startTag, string endTag)
    {
        return ExtractValueBetweenTags(source, startTag, endTag);
    }

    private static string ExtractValueBetweenTags(string source, string startTag, string endTag)
    {
        var startIndex = source.IndexOf(startTag, StringComparison.Ordinal);
        if (startIndex == -1) return string.Empty;
        startIndex = source.IndexOf('>', startIndex) + 1;
        var endIndex = source.IndexOf(endTag, startIndex, StringComparison.Ordinal);
        return endIndex == -1 ? string.Empty : source[startIndex..endIndex].Trim();
    }

    public async Task<string> GetHtml(string url)
    {
        var proxy = proxyService.GetProxy();
        try
        {
            await new BrowserFetcher().DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            { Headless = true, DefaultViewport = null, Args = ["--no-sandbox", $"--proxy-server={proxy}"] });
            await using var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync(GenerateRandomUserAgent());
            await page.SetCookieAsync(GenerateRandomCookie());

            await page.SetRequestInterceptionAsync(true);

            var blockedDomains = new[]
            {
                        "doubleclick.net", "googlesyndication.com", "adservice.google.com",
                        "ads.yahoo.com", "ads.bing.com", "adroll.com", "outbrain.com",
                        "taboola.com", "amazon-adsystem.com", "criteo.com", "facebook.com/ad",
                        "googleadservices.com", "ads",
                    };

            page.Request += async (sender, e) =>
            {
                var url = e.Request.Url.ToLower();
                if (e.Request.ResourceType == ResourceType.Image || e.Request.ResourceType == ResourceType.Font ||
                    e.Request.ResourceType == ResourceType.Media || e.Request.ResourceType == ResourceType.StyleSheet ||
                    blockedDomains.Any(d => url.Contains(d)))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };

            if (DotEnv.Get("environment").Equals("dev")) // prod is authed via ip
            {
                await page.AuthenticateAsync(new Credentials { Username = DotEnv.Get("proxy-user"), Password = DotEnv.Get("proxy-pw") });
            }   


            await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
            await page.WaitForSelectorAsync("body");

            return await page.GetContentAsync();
        }
        catch (NavigationException e)
        {
            logger.LogError(e, "Navigation error while fetching HTML from URL: {Url} with proxy {Proxy}", url, proxy);
            throw new Exception("Navigation error: " + e.Message);
        }
        catch (TimeoutException e)
        {
            logger.LogError(e, "Timeout error while fetching HTML from URL: {Url} with proxy {Proxy}", url, proxy);
            throw new Exception("Timeout error: " + e.Message);
        }
        catch (PuppeteerException e)
        {
            logger.LogError(e, "Puppeteer error while fetching HTML from URL: {Url} with proxy {Proxy}", url, proxy);
            throw new Exception("Puppeteer error: " + e.Message);
        }
        catch (Exception e)
        {   
            logger.LogError(e, "General error while fetching HTML from URL: {Url} with proxy {Proxy}", url, proxy);
            throw new Exception("Error while fetching html: " + e.Message);
        }
    }

    private static string GenerateRandomUserAgent()
    {
        var osOptions = new[]
        {
                    "Windows NT 10.0; Win64; x64",
                    "Windows NT 10.0; WOW64",
                    "Macintosh; Intel Mac OS X 10_15_7",
                    "X11; Linux x86_64",
                    "X11; Ubuntu; Linux x86_64"
                };

        var browserOptions = new[]
        {
                    "Chrome/110.0.5481.177",
                    "Chrome/112.0.5615.121",
                    "Chrome/114.0.5735.198",
                    "Firefox/110.0",
                    "Firefox/114.0",
                    "Safari/537.36"
                };

        var os = osOptions[new Random().Next(osOptions.Length)];
        var browser = browserOptions[new Random().Next(browserOptions.Length)];

        return $"Mozilla/5.0 ({os}) AppleWebKit/537.36 (KHTML, like Gecko) {browser} Safari/537.36";
    }

    private static CookieParam GenerateRandomCookie()
    {
        var random = new Random();
        string[] names = ["session_id", "user_token", "tracking_id", "preferences", "auth"];
        string[] values = [Guid.NewGuid().ToString(), "xyz123", "track_" + random.Next(100000, 999999), "darkmode=true", "secure_cookie"
        ];

        return new CookieParam
        {
            Name = names[random.Next(names.Length)],
            Value = values[random.Next(values.Length)],
            Domain = "boerse.de",
            Path = "/",
            HttpOnly = random.Next(0, 2) == 1,
            Secure = random.Next(0, 2) == 1,
            Expires = (DateTime.UtcNow.AddDays(random.Next(1, 30)) - new DateTime(1970, 1, 1)).TotalSeconds
        };
    }
}