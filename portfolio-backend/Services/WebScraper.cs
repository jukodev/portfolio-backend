using System.Globalization;
using portfolio_backend.DTOs;
using PuppeteerSharp;

namespace portfolio_backend.Services;

public partial class WebScraper
{
    public async Task<WebScrapedStockDto> ScrapStockData(string url, string proxy)
    {
        var html = await GetHtml(url, proxy);

       // Console.WriteLine(rawHtml);

        var indiceFirst = html.IndexOf("<!-- KURSE -->", StringComparison.Ordinal);
        var indiceLast = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html.Substring(indiceFirst, indiceLast - indiceFirst);

        Console.WriteLine("hi");

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

    public async Task<WebScrapedGoldDto> ScrapGoldData(string url, string proxy)
    {
        var html = await GetHtml(url, proxy);

        var start = html.IndexOf("<!-- KURSE EURO -->", StringComparison.Ordinal);
        var end = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html.Substring(start, end - start);

        var lastPrice = ExtractFloatAfterTag(htmlKurse, "<span", CultureInfo.InvariantCulture);

        var afterPrice = AdvancePastSpan(htmlKurse, "<span", "</span>");
        var currency = ExtractValueBetweenTags(afterPrice, "<span", "</span>");
        var afterCurrency = AdvancePastSpan(afterPrice, "<span", "</span>");

        var lastPerformance = ExtractFloatAfterTag(afterCurrency, "<span", CultureInfo.InvariantCulture);
        var afterPerf = AdvancePastSpan(afterCurrency, "<span", "</span>");

        var timeLabel = "Zeit ";
        var timeIndex = afterPerf.IndexOf(timeLabel, StringComparison.Ordinal);
        var timeEnd = afterPerf.IndexOf("</div>", timeIndex, StringComparison.Ordinal);
        var time = afterPerf.Substring(timeIndex + timeLabel.Length, timeEnd - (timeIndex + timeLabel.Length)).Trim();
        var afterTime = afterPerf[(timeEnd + 6)..];

        var lastPerformancePercent = ExtractFloatAfterTag(afterTime, "<span", CultureInfo.InvariantCulture);
        var afterPerfPercent = AdvancePastSpan(afterTime, "<span", "</span>");

        const string dateLabel = "Datum&nbsp;";
        var dateIndex = afterPerfPercent.IndexOf(dateLabel, StringComparison.Ordinal);
        var dateRaw = afterPerfPercent.Substring(dateIndex + dateLabel.Length, 8); 

        var day = dateRaw[..2];
        var month = dateRaw.Substring(3, 2) ;
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
        return endIndex < 0 ? source : source.Substring(endIndex + endTag.Length);
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
        return endIndex == -1 ? string.Empty : source.Substring(startIndex, endIndex - startIndex).Trim();
    }

    private async Task<string> GetHtml(string url, string proxy)
    {
        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        { Headless = true, DefaultViewport = null, Args = new[] { "--no-sandbox", $"--proxy-server={proxy}" } });
        await using var page = await browser.NewPageAsync();
        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        await page.SetCookieAsync(new CookieParam
        {
            Name = "example_cookie",
            Value = "example_value",
            Domain = "boerse.de"
        });

        await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
        await page.WaitForSelectorAsync("body");

        return await page.GetContentAsync();
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

    private static CookieParam GenerateRandomCookie(string domain)
    {
        var random = new Random();
        string[] names = ["session_id", "user_token", "tracking_id", "preferences", "auth"];
        string[] values = [Guid.NewGuid().ToString(), "xyz123", "track_" + random.Next(100000, 999999), "darkmode=true", "secure_cookie"
        ];

        return new CookieParam
        {
            Name = names[random.Next(names.Length)],
            Value = values[random.Next(values.Length)],
            Domain = domain,
            Path = "/",
            HttpOnly = random.Next(0, 2) == 1,
            Secure = random.Next(0, 2) == 1,
            Expires = (DateTime.UtcNow.AddDays(random.Next(1, 30)) - new DateTime(1970, 1, 1)).TotalSeconds
        };
    }
}