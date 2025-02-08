namespace portfolio_backend.DTOs;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ProxyResponseDto
{
    [JsonProperty("count")]
    public required int Count { get; set; }

    [JsonProperty("next")]
    public string? Next { get; set; }

    [JsonProperty("previous")]
    public string? Previous { get; set; }

    [JsonProperty("results")]
    public required List<ProxyData> Results { get; set; }
}

public class ProxyData
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("username")]
    public required string Username { get; set; }

    [JsonProperty("password")]
    public required string Password { get; set; }

    [JsonProperty("proxy_address")]
    public required string ProxyAddress { get; set; }

    [JsonProperty("port")]
    public required int Port { get; set; }

    [JsonProperty("valid")]
    public required bool Valid { get; set; }

    [JsonProperty("last_verification")]
    public required DateTime LastVerification { get; set; }

    [JsonProperty("country_code")]
    public required string CountryCode { get; set; }

    [JsonProperty("city_name")]
    public required string CityName { get; set; }

    [JsonProperty("created_at")]
    public required DateTime CreatedAt { get; set; }
}
