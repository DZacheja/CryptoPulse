using CryptoPulse.BianceApi.Attributes;
using CryptoPulse.BianceApi.DTOs;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Infrastructure.Exceptions;
using CryptoPulse.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

public class BinanceApiClient : IBinanceApiClient
{
	private string _apiKey { get; set; } = string.Empty;
	private string _privateKey { get; set; } = string.Empty;
	private HttpClient _httpClient;
	private HttpClient _httpClientNoAuth;
	private readonly long _recvWindow = 50000;
	private readonly ISecureStorageService _secureStorage;
	private bool _hasValidKeys;

	public BinanceApiClient(ISecureStorageService secureStorage)
	{
		_secureStorage = secureStorage;
		_httpClient = new HttpClient();
		_httpClientNoAuth = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
	}

	public async Task InitializeAsync()
	{
		await InitializeAuthHttpClientAsync();
	}

	public void TestInitializeObject()
	{
		_apiKey = "TzyLPYqcZx7hqc6cCJSsjr69tPpSNmOLXIVLyPFTvWL4mR4UpTopDPuq5nkzvcTr";
		_privateKey = "VyfPsOsFLe0uHyGTLurCAqj8ACdMXNb7V0zpjyzzX5Iu8jufroqioxYVmL8IyiLZ";
		_hasValidKeys = true;
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};

		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}

	// Generate HMAC SHA256 signature
	private string CreateSignature(string queryString)
	{
		if (!_hasValidKeys)
			GetCashedKeys();

		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey));
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}
	#region Authorization required

	[AuthKeyRequired]
	public async Task<AccountInfoDto> GetAccountInfo()
	{
		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var queryString = $"timestamp={timestamp}&recvWindow={_recvWindow}&omitZeroBalances=true";
		var signature = CreateSignature(queryString);
		var url = $"/api/v3/account?{queryString}&signature={signature}";

		var response = await _httpClient.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			var json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<AccountInfoDto>(json);
		}
		else
		{
			throw new Exception($"API call failed: {response.StatusCode}");
		}
	}
	/// <summary>
	/// Fetch transaction history for a given pair
	/// </summary>
	/// <param name="symbol"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	[AuthKeyRequired]
	public async Task<List<AccountTradeListDto>> GetAccountTradeLisAsync(string symbol)
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

	[AuthKeyRequired]
	public async Task<AccountTradeListDto> GetAccountTradeLastPairOperationAsync(string symbol)
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
	#endregion

	#region No authorization methods
	public async Task<PairPriceTickerDto> GetSymbolCurrentPriceAsync(string symbol)
	{
		var queryString = $"symbol={symbol}";
		var url = $"/api/v3/ticker/price?{queryString}";

		var response = await _httpClientNoAuth.GetAsync(url);
		if (!response.IsSuccessStatusCode)
		{
			throw new Exception($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
		}

		var json = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<PairPriceTickerDto>(json) ?? throw new Exception("Response is empty");
	}

	public async Task<PairAvgPriceDTo> GetSymbolAvgPriceAsync(string symbol)
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
	#endregion

	#region Keys operation
	private void GetCashedKeys()
	{
		_apiKey = _secureStorage.GetApiKeyCashed() ?? string.Empty;
		_privateKey = _secureStorage.GetApiPrivateKeyCashed() ?? string.Empty;

		if (!(string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_privateKey)))
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://api.binance.com")
			};
			_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
			_hasValidKeys = true;
		}
		else
		{
			_hasValidKeys = false;
			throw new NoAuthKeyException();
		}
	}

	public async Task InitializeAuthHttpClientAsync()
	{
		if (!_hasValidKeys)
		{

			_apiKey = await _secureStorage.GetApiKeyAsync() ?? string.Empty;
			_privateKey = await _secureStorage.GetApiPrivateKeyAsync() ?? string.Empty;

			_httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://api.binance.com")
			};

			if (!(string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_privateKey)))
			{
				_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
				_hasValidKeys = true;
			}
			else
			{
				_hasValidKeys = false;
			}
		}
	}

	public async Task<bool> ChceckUserKeysValidationAsync(string apiKey, string privateKey, bool applayNewKeys = false)
	{

		HttpClient tempClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};
		tempClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);

		var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var queryString = $"timestamp={timestamp}&recvWindow={_recvWindow}";

		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
		var signature =  BitConverter.ToString(hash).Replace("-", "").ToLower();

		var url = $"/api/v3/account?{queryString}&signature={signature}";
		var response = await tempClient.GetAsync(url);
		var responseSuccess = response.IsSuccessStatusCode;

		if (applayNewKeys && responseSuccess)
		{
			SetNewKeyApiValueToHttpClient(apiKey, privateKey);
		}

		return response.IsSuccessStatusCode;
	}

	public bool GetKeysValidationInfo()
	{
		return _hasValidKeys;
	}

	public void SetNewKeyApiValueToHttpClient(string apiKey, string privateKey)
	{
		_apiKey = apiKey;
		_privateKey = privateKey;
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://api.binance.com")
		};

		_httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
	}
	#endregion
}
