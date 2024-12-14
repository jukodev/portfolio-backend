using System.Globalization;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using portfolio_backend.DTOs;

namespace portfolio_backend.Services;

public partial class WebScraper (HttpClient client)
{
    public async Task<WebScrapedStockDto> ScrapStockData(string url)
    {
        var html = await GetHtml(url);
        
        var indiceFirst = html.IndexOf("<!-- KURSE -->", StringComparison.Ordinal);
        var indiceLast = html.IndexOf("<!-- ENDE KURSE -->", StringComparison.Ordinal);
        var htmlKurse = html.Substring(indiceFirst, indiceLast - indiceFirst);

        var lastPrice = ExtractFloatAfterTag(htmlKurse, "itemprop=\"price\" content=", CultureInfo.InvariantCulture);

        var lastPerformance = ExtractFloatAfterTag(htmlKurse, "data-push-attribute=\"perfInstAbs", CultureInfo.InvariantCulture);

        var lastPerformancePercent = ExtractFloatAfterTag(htmlKurse, "data-push-attribute=\"perfInstRel", CultureInfo.InvariantCulture);

        var priceCurrency = ExtractStringAfterTag(htmlKurse, "itemprop=\"priceCurrency\" >", "</span>");

        var time = ExtractStringAfterTag(htmlKurse, "data-push-attribute=\"timestamp\">", "</span>");

        var dateRaw = ExtractStringAfterTag(htmlKurse, "data-push-attribute=\"date\">", "</span>");
        var shortDd = dateRaw[..2];
        var shortMm = dateRaw.Substring(3, 2);
        var shortYy = dateRaw.Substring(6, 2);
        var lastTime = "20" + shortYy + "-" + shortMm + "-" + shortDd + " " + time;

        var lastBid = ExtractFloatAfterTag(html, "data-push-attribute=\"bid\"", CultureInfo.InvariantCulture);

        var lastAsk = ExtractFloatAfterTag(html, "data-push-attribute=\"ask\"", CultureInfo.InvariantCulture);

        var yesterdayClose = ExtractFloatAfterTag(html, "data-push-attribute=\"close\"", CultureInfo.InvariantCulture);

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
    
    private static float ExtractFloatAfterTag(string source, string startTag, IFormatProvider culture)
    {
        var val = ExtractValueBetweenTags(source, startTag, "</span>");
        val = val.Replace(',', '.');
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
    
    private async Task<string> GetHtml(string url)
    {
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Portfolio", "1.0"));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(compatible; MSIE 6.0; Windows NT 5.2)"));
        var response = await client.GetAsync(url);
        var text = await response.Content.ReadAsStringAsync();
        return text;
    }
}