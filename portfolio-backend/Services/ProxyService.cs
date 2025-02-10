using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
using portfolio_backend.DTOs;

namespace portfolio_backend.Services;

public class ProxyService
{
    private List<string> _proxies = new List<string>();
    private readonly string ProxyListUrl = "https://proxy.webshare.io/api/v2/proxy/list/?mode=direct&page=1&page_size=10";
    private readonly HttpClient httpClient;
    private readonly ILogger<ProxyService> logger;

    public ProxyService(HttpClient httpClient, IConfiguration configuration, ILogger<ProxyService> logger)
    {
        logger.LogInformation("Initializing ProxyService {}", configuration["webshare-authtoken"]);
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {configuration["webshare-authtoken"]}");
        this.logger = logger;
        InitializeProxies();
    }

    public void InitializeProxies()
    {
        _proxies.Clear();


        httpClient.GetAsync(ProxyListUrl).ContinueWith(task =>
        {
            var response = task.Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var proxies = JsonConvert.DeserializeObject<ProxyResponseDto>(json);
            _proxies = proxies!.Results.Select(p => $"http://{p.ProxyAddress}:{p.Port}").ToList();
            logger.LogInformation($"Initialized {_proxies.Count} proxies");
        }).Exception?.Handle(e =>
        {
            logger.LogError("Failed to initialize proxies: " + e.Message);
            return true;
        });
    }

    public string GetProxy()
    {
        var random = new Random();
        var index = random.Next(_proxies.Count);
        return _proxies[index];
    }
}
