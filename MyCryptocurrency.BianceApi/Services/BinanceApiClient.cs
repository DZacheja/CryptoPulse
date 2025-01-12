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
	private HttpClient _httpClientNoAuth;
	private readonly long _recvWindow = 50000;
	private readonly ISecureStorageService _secureStorage;

	public BinanceApiClient(ISecureStorageService secureStorage)
	{
		_secureStorage = secureStorage;
		_httpClientNoAuth = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
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
		await CheckKeys();
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


	public async Task<PairPriceTickerDto> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token)
	{
		var queryString = $"symbol={symbol}";
		var url = $"/api/v3/ticker/price?{queryString}";

		try
		{

			var response = await _httpClientNoAuth.GetAsync(url,token.Token);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
			}
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<PairPriceTickerDto>(json);
		}
		catch(TaskCanceledException)
		{
			return null;
		}
	}

	public async Task<PairAvgPriceDTo> GetSymbolAvgPrice(string symbol)
	{
		var queryString = $"symbol={symbol}";
		var url = $"/api/v3/avgPrice?{queryString}";

		var response = await _httpClientNoAuth.GetAsync(url);
		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}

		var json = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<PairAvgPriceDTo>(json);
	}

	public async Task<AccountTradeListDto> GetAccountTradeLastPairOperation(string symbol)
	{
		await CheckKeys();
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var queryString = $"symbol={symbol}&limit=1&timestamp={timestamp}&recvWindow={_recvWindow}";
		var signature = CreateSignature(queryString);
		var url = $"/api/v3/myTrades?{queryString}&signature={signature}";

		var response = await _httpClient.GetAsync(url);
		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<AccountTradeListDto>>(json).FirstOrDefault();
		}else
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}

	}
	private async Task CheckKeys()
	{
		if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
			await GetAuthorizationKeysFromSecureStorage();
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
	public async Task GetAuthorizationKeysFromSecureStorage()
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
