using MyCryptocurrency.BianceApi.DTOs;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using MyCryptocurrency.Infrastructure.Services.Interfaces;

public class BinanceApiClient : IBinanceApiClient
{
	private string _apiKey;
	private string _apiSecret;
	private HttpClient _httpClient;
	private readonly long _recvWindow = 5000;
	private readonly ISecureStorageService _secureStorage;

	public BinanceApiClient(ISecureStorageService secureStorage)
	{
		_secureStorage = secureStorage;
	}

	// Generate HMAC SHA256 signature
	private string CreateSignature(string queryString)
	{
		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}

	// Fetch transaction history for a given pair
	public async Task<List<AccountTradeListDto>> GetAccountTradeLis(string symbol)
	{
		CheckKeys();
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var starttime = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeMilliseconds();
		var queryString = $"symbol={symbol}&timestamp={timestamp}&startTime={starttime}&recvWindow={_recvWindow}";
		var signature = CreateSignature(queryString);
		var url = $"/api/v3/myTrades?{queryString}&signature={signature}";

		var response = await _httpClient.GetAsync(url);
		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}

		var json = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<List<AccountTradeListDto>>(json);
	}

	private void CheckKeys()
	{
		if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
			GetAuthorizationKeysFromSecureStorage();
	}

	public async Task SetNewKeyApiValue(string apiKey)
	{
		_apiKey = apiKey;
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}

	public async Task SetNewPrivateKeyValue(string privateKey)
	{
		_apiSecret = privateKey;
	}

	public async void GetAuthorizationKeysFromSecureStorage()
	{
		_apiKey = await _secureStorage.GetApiKeyAsync();
		_apiSecret = await _secureStorage.GetApiPrivateKeyAsync();
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}
}
