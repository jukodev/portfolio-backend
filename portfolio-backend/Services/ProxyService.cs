using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
using portfolio_backend.DTOs;

namespace portfolio_backend.Services;

public class ProxyService
{
    private List<string> _proxies  = [];

    private const string ProxyListUrl =
        "https://api.proxyscrape.com/v4/free-proxy-list/get?request=display_proxies&proxy_format=protocolipport&format=json&timeout=500&ssl=no&&protocol=http&anonymity=all";
    
    
    public async Task ValidateProxies()
    {
        Console.WriteLine(_proxies.Count + " proxies found");
        try
        {
            List<string> invalidProxies = [];
        
            await Parallel.ForEachAsync(_proxies, async (proxyUrl, _) =>
            {
                var proxy = new WebProxy(proxyUrl);
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true,
                };
                using var httpClient = new HttpClient(httpClientHandler);
                httpClient.Timeout = TimeSpan.FromMilliseconds(500);
                if (await IsUrlUnReachableAsync(httpClient))
                {
                    lock (invalidProxies)
                    {
                        invalidProxies.Add(proxyUrl);
                    }
                }
                Console.WriteLine(proxyUrl);
            });
            _proxies = _proxies.Except(invalidProxies).ToList();
            Console.WriteLine(_proxies.Count + " proxies valid");
            if (_proxies.Count < 30)
            {
                await InitializeProxies();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to validate proxies\n" + e.Message);
        }
    }
    
    public async Task InitializeProxies()
    {
        _proxies.Clear();
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(ProxyListUrl);
        var json = await response.Content.ReadAsStringAsync();
        var proxies = JsonConvert.DeserializeObject<ProxyResponseDto>(json);
        _proxies = proxies!.Proxies.Select(p => p.Proxy).ToList();
        Console.WriteLine(_proxies.Count + " proxies found");
        await ValidateProxies();
    }
    
    public string GetProxy()
    {
        var random = new Random();
        var index = random.Next(_proxies.Count);
        return _proxies[index];
    }
    
    private static async Task<bool> IsUrlUnReachableAsync(HttpClient client)
    {
        try
        {
            using var response = await client.GetAsync("https://www.boerse.de/realtime-kurse/Visa-Aktie/US92826C8394");
            var html = await response.Content.ReadAsStringAsync();
            var index = html.IndexOf("<!-- KURSE -->", StringComparison.Ordinal);
            Console.WriteLine(html.Substring(index, index +20));
            return !response.IsSuccessStatusCode || !html.Contains("<!-- KURSE -->");
        }
        catch
        {
            return true;
        }
    }
}