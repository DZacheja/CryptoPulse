using MyCryptocurrency.BianceApi.DTOs;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

public class BinanceApiClient : IBinanceApiClient
{
	private string _apiKey { get; set; } = string.Empty;
	private string _apiSecret { get; set; } = string.Empty;
	private HttpClient _httpClient;
	private HttpClient _httpClientNoAuth;
	private readonly long _recvWindow = 50000;
	private readonly ISecureStorageService _secureStorage;

	public BinanceApiClient(ISecureStorageService secureStorage)
	{
		_secureStorage = secureStorage;
		_httpClient = new HttpClient();
		_httpClientNoAuth = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};

		InitializeAuthHttpClientAsync().GetAwaiter().GetResult();
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

		return JsonConvert.DeserializeObject<List<AccountTradeListDto>>(json) ?? throw new Exception("Response is empty");
	}


	public async Task<PairPriceTickerDto> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token)
	{
		var queryString = $"symbol={symbol}";
		var url = $"/api/v3/ticker/price?{queryString}";

		var response = await _httpClientNoAuth.GetAsync(url,token.Token);
		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}

		var json = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<PairPriceTickerDto>(json) ?? throw new Exception("Response is empty");
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
		return JsonConvert.DeserializeObject<PairAvgPriceDTo>(json) ?? throw new Exception("Response is empty");
	}

	public async Task<AccountTradeListDto> GetAccountTradeLastPairOperation(string symbol)
	{
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var queryString = $"symbol={symbol}&limit=1&timestamp={timestamp}&recvWindow={_recvWindow}";
		var signature = CreateSignature(queryString);
		var url = $"/api/v3/myTrades?{queryString}&signature={signature}";

		var response = await _httpClient.GetAsync(url);
		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<AccountTradeListDto>>(json)?.FirstOrDefault() ?? throw new Exception("Response is empty");
		}
		else
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}
	}

	public async Task<List<KlineDataDto>> GetHistoricalDataAsync(string symbol, string interval, int limit)
	{
		var response = await _httpClientNoAuth.GetAsync($"/api/v3/klines?symbol={symbol}&interval={interval}&limit={limit}");
		response.EnsureSuccessStatusCode();

		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			var test = JsonConvert.DeserializeObject<List<List<object>>>(json);
			var firstElemet = test!.FirstOrDefault();
			foreach (var value in firstElemet!)
			{
				Console.WriteLine($"Value: {value}");
			}
			try
			{
				return JsonConvert.DeserializeObject<List<List<object>>>(json)!.Select(kline => new KlineDataDto
				{
					OpenTime = (long)Convert.ToDouble(kline[0], CultureInfo.InvariantCulture),
					OpenPrice = Convert.ToDecimal(kline[1], CultureInfo.InvariantCulture),
					HighPrice = Convert.ToDecimal(kline[2], CultureInfo.InvariantCulture),
					LowPrice = Convert.ToDecimal(kline[3], CultureInfo.InvariantCulture),
					ClosePrice = Convert.ToDecimal(kline[4], CultureInfo.InvariantCulture),
					Volume = Convert.ToDouble(kline[5], CultureInfo.InvariantCulture),
					CloseTime = (long)Convert.ToDouble(kline[6], CultureInfo.InvariantCulture),
					QuoteAssetVolume = Convert.ToDouble(kline[7], CultureInfo.InvariantCulture),
					NumberOfTrades = (long)Convert.ToDouble(kline[8], CultureInfo.InvariantCulture),
					TakerBuyBaseAssetVolume = Convert.ToDecimal(kline[9], CultureInfo.InvariantCulture),
					TakerBuyQuoteAssetVolume = Convert.ToDecimal(kline[10], CultureInfo.InvariantCulture)
				}).ToList();
			}
			catch (Exception ex)
			{
				string messahe = ex.Message;
				throw new Exception($"API call failed: {response.StatusCode} - {ex.Message}");
			}
		}
		else
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}
	}

	#region Keys operation
	//private async Task CheckKeys()
	//{
	//	if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
	//		await GetAuthorizationKeysFromSecureStorage();
	//}

	public void SetNewKeyApiValue(string apiKey)
	{
		_apiKey = apiKey;
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}

	public void SetNewPrivateKeyValue(string privateKey)
	{
		_apiSecret = privateKey;
	}


	public async Task InitializeAuthHttpClientAsync()
	{
		_apiKey = await _secureStorage.GetApiKeyAsync() ?? string.Empty;
		_apiSecret = await _secureStorage.GetApiPrivateKeyAsync() ?? string.Empty;
		if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
			throw new Exception("Your authentication key's not found!");

		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}
	#endregion
}
