using System.Net;
using System.Text.Json;
using Newtonsoft.Json;
using portfolio_backend.DTOs;

namespace portfolio_backend.Services;

public class ProxyService//(HttpClient httpClient)
{
    private readonly List<string> _proxies  = [];
    
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

        /*
        httpClient.GetAsync(ProxyListUrl).ContinueWith(task =>
        {
            var response = task.Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var proxies = JsonConvert.DeserializeObject<ProxyResponseDto>(json);
            _proxies = proxies!.Results.Select(p => $"http://{p.ProxyAddress}{p.Port}").ToList();
            Console.WriteLine(_proxies.Count + " proxies found");
        });*/
    }

    public string GetProxy()
    {
        var random = new Random();
        var index = random.Next(_proxies.Count);
        return _proxies[index];
    }
}