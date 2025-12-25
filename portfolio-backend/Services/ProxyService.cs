using Newtonsoft.Json;
using portfolio_backend.DTOs;
using portfolio_backend.Utils;

namespace portfolio_backend.Services;

public class ProxyService(HttpClient httpClient, ILogger<ProxyService> logger) : IHostedService
{
    private List<string> _proxies = [];
    private readonly string ProxyListUrl = "https://proxy.webshare.io/api/v2/proxy/list/?mode=direct&page=1&page_size=100";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {DotEnv.Get("WEBSHARE_AUTHTOKEN")}");
        InitializeProxies();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async void InitializeProxies()
    {
        try
        {
            _proxies.Clear();
            var res = await httpClient.GetAsync(ProxyListUrl);
            var json = res.Content.ReadAsStringAsync().Result;
            var proxies = JsonConvert.DeserializeObject<ProxyResponseDto>(json);
            _proxies = proxies!.Results.Select(p => $"http://{p.ProxyAddress}:{p.Port}").ToList();
            logger.LogInformation("Initialized {} proxies", _proxies.Count);
        }
        catch (Exception e)
        {
            logger.LogError("Failed to initialize proxies: {}", e.Message);
            throw e;
        }
    }

    public string GetProxy()
    {
        if(_proxies.Count == 0)
        {
            throw new Exception("No proxies available");
        }
        var random = new Random();
        var index = random.Next(_proxies.Count);
        return _proxies[index];
    }

    
}
