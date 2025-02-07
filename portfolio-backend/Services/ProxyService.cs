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
        return;
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
                //await InitializeProxies();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to validate proxies\n" + e.Message);
        }
    }
    
    public void InitializeProxies()
    {
        _proxies.Clear();
        _proxies.Add("http://198.23.239.134:6540");
        _proxies.Add("http://207.244.217.165:6712");
        _proxies.Add("http://107.172.163.27:6543");
        _proxies.Add("http://64.137.42.112:5157");
        _proxies.Add("http://173.211.0.148:6641");
        _proxies.Add("http://161.123.152.115:6360");
        _proxies.Add("http://23.94.138.75:6349");
        _proxies.Add("http://154.36.110.199:6853");
        _proxies.Add("http://173.0.9.70:5653");
        _proxies.Add("http://173.0.9.209:5792");

        /*198.23.239.134:6540
207.244.217.165:6712
107.172.163.27:6543
64.137.42.112:5157
173.211.0.148:6641
161.123.152.115:6360
23.94.138.75:6349
154.36.110.199:6853
173.0.9.70:5653
173.0.9.209:5792

         * 
         * 
        _proxies.Clear();
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(ProxyListUrl);
        var json = await response.Content.ReadAsStringAsync();
        var proxies = JsonConvert.DeserializeObject<ProxyResponseDto>(json);
        _proxies = proxies!.Proxies.Select(p => p.Proxy).ToList();
        Console.WriteLine(_proxies.Count + " proxies found");
        await ValidateProxies();*/
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