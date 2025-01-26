using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using portfolio_backend.DTOs;
using PuppeteerSharp;

namespace portfolio_backend.Services;

public partial class WebScraper()
{
    public async Task<WebScrapedStockDto> ScrapStockData(string url)
    {
        var html = await GetHtml(url);

        var indiceFirst = html.IndexOf("<!-- KURSE -->", StringComparison.Ordinal);
        var indiceLast = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html.Substring(indiceFirst, indiceLast - indiceFirst);

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

    private static async Task<string> GetHtml(string url)
    {
        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            { Headless = true, DefaultViewport = null, Args = new[] { "--no-sandbox" } });
        await using var page = await browser.NewPageAsync();
        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        await page.SetCookieAsync(new CookieParam
        {
            Name = "example_cookie",
            Value = "example_value",
            Domain = "boerse.de"
        });

        await page.GoToAsync(url);
        return await page.GetContentAsync();
    }
}