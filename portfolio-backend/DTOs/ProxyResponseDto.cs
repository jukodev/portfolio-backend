namespace portfolio_backend.DTOs;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ProxyResponseDto
{
    [JsonProperty("shown_records")]
    public int ShownRecords { get; set; }

    [JsonProperty("total_records")]
    public int TotalRecords { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("skip")]
    public int Skip { get; set; }

    [JsonProperty("nextpage")]
    public bool NextPage { get; set; }

    [JsonProperty("proxies")]
    public List<ProxyDto> Proxies { get; set; }
}

public class ProxyDto
{
    [JsonProperty("alive")]
    public bool Alive { get; set; }

    [JsonProperty("alive_since")]
    public double AliveSince { get; set; }

    [JsonProperty("anonymity")]
    public string Anonymity { get; set; }

    [JsonProperty("average_timeout")]
    public double AverageTimeout { get; set; }

    [JsonProperty("first_seen")]
    public double FirstSeen { get; set; }

    [JsonProperty("ip_data")]
    public IpDataDto IpData { get; set; }

    [JsonProperty("ip_data_last_update")]
    public double IpDataLastUpdate { get; set; }

    [JsonProperty("last_seen")]
    public double LastSeen { get; set; }

    [JsonProperty("port")]
    public int Port { get; set; }

    [JsonProperty("protocol")]
    public string Protocol { get; set; }

    [JsonProperty("proxy")]
    public string Proxy { get; set; }

    [JsonProperty("ssl")]
    public bool Ssl { get; set; }

    [JsonProperty("timeout")]
    public double Timeout { get; set; }

    [JsonProperty("times_alive")]
    public int TimesAlive { get; set; }

    [JsonProperty("times_dead")]
    public int TimesDead { get; set; }

    [JsonProperty("uptime")]
    public double Uptime { get; set; }

    [JsonProperty("ip")]
    public string Ip { get; set; }
}

public class IpDataDto
{
    [JsonProperty("as")]
    public string As { get; set; }

    [JsonProperty("asname")]
    public string AsName { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("continent")]
    public string Continent { get; set; }

    [JsonProperty("continentCode")]
    public string ContinentCode { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }

    [JsonProperty("district")]
    public string District { get; set; }

    [JsonProperty("hosting")]
    public bool Hosting { get; set; }

    [JsonProperty("isp")]
    public string Isp { get; set; }

    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }

    [JsonProperty("mobile")]
    public bool Mobile { get; set; }

    [JsonProperty("org")]
    public string Organization { get; set; }

    [JsonProperty("proxy")]
    public bool Proxy { get; set; }

    [JsonProperty("regionName")]
    public string RegionName { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("timezone")]
    public string TimeZone { get; set; }

    [JsonProperty("zip")]
    public string Zip { get; set; }
}
