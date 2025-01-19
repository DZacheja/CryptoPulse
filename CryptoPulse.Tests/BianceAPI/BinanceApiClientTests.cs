using CryptoPulse.BianceApi.Attributes;
using CryptoPulse.BianceApi.DTOs;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Infrastructure.Exceptions;
using CryptoPulse.Infrastructure.Services;
using CryptoPulse.Infrastructure.Services.Interfaces;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CryptoPulse.Tests.BianceAPI;
public class BinanceApiClientTests:BaseTest
{
	private readonly Mock<ISecureStorageService> _mockSecureStorage;
	private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
	private readonly HttpClient _mockHttpClient;
	private readonly BinanceApiClient _binanceApiClient;
	private readonly Mock<IBinanceApiClient> _mockBinanceApiClient;

	#region Constructor
	public BinanceApiClientTests()
	{
		_mockSecureStorage = new Mock<ISecureStorageService>();
		_mockSecureStorage.Setup(x => x.GetApiKeyAsync()).ReturnsAsync("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyAsync()).ReturnsAsync("testApiSecret");
		_mockSecureStorage.Setup(x => x.GetApiKeyCashed()).Returns("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyCashed()).Returns("testApiSecret");

		_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		_mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object)
		{
			BaseAddress = new Uri("https://api.binance.com")
		};

		// Initialize the BinanceApiClient with the mocked dependencies
		_binanceApiClient = new BinanceApiClient(_mockSecureStorage.Object);
		_binanceApiClient.GetType().GetField("_hasValidKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_binanceApiClient, true);

		_mockBinanceApiClient = new Mock<IBinanceApiClient>();
	}
	#endregion

	[Fact]
	public async Task GetDataWithRequiedAuthorization_ThrownNoAuthKeyExeption()
	{
		//Arrange
		var mockSecureStorage = new Mock<ISecureStorageService>();
		mockSecureStorage.Setup(x => x.GetApiKeyCashed()).Returns(string.Empty);
		mockSecureStorage.Setup(x => x.GetApiPrivateKeyCashed()).Returns(string.Empty);

		BinanceApiClient binanceApiClient = new BinanceApiClient(mockSecureStorage.Object);
		binanceApiClient.GetType().GetField("_hasValidKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_binanceApiClient, false);

		var methods = typeof(BinanceApiClient)
			.GetMethods()
			.Where(m => m.GetCustomAttributes(typeof(AuthKeyRequiredAttribute), false).Any());

		// Act & Assert
		foreach (var method in methods)
		{
			switch (method.Name)
			{
				case nameof(_binanceApiClient.GetAccountTradeLisAsync):
				{
					await Assert.ThrowsAsync<NoAuthKeyException>(() => binanceApiClient.GetAccountTradeLisAsync("BTCUSDT"));
				}
				break;
				case nameof(_binanceApiClient.GetAccountTradeLastPairOperationAsync):
				{
					await Assert.ThrowsAsync<NoAuthKeyException>(() => binanceApiClient.GetAccountTradeLastPairOperationAsync("BTCUSDT"));
				}
				break;
				case nameof(_binanceApiClient.GetAccountInfo):
				{
					await Assert.ThrowsAsync<NoAuthKeyException>(() => binanceApiClient.GetAccountInfo());
				}
				break;
				default:
				Assert.Fail($"Not all method's with AuthKeyRequired are tested!, missing Nethod: {method.Name}");
				break;
			}
		}
	}

	#region GetAccountInfo
	[Fact]
	public async Task GetAccountInfo_ReturnsAccountInfoDto()
	{
		// Arrange
		var jsonResponse = await GetBianceApiTestFileContent("GetAccountInfo");

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		var resonse = await _binanceApiClient.GetAccountInfo();
		var jsonData = JsonConvert.DeserializeObject<AccountInfoDto>(jsonResponse);

		// Act & Assert
		Assert.NotNull(resonse);
		Assert.Equivalent(jsonData, resonse);
	}
	#endregion

	#region GetAccountTradeLis
	[Fact]
	public async Task GetAccountTradeLis_ThrowsException_OnApiFailure()
	{
		// Arrange
		_mockSecureStorage.Setup(x => x.GetApiKeyAsync()).ReturnsAsync("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyAsync()).ReturnsAsync("testApiSecret");

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.BadRequest,
				Content = new StringContent("Bad Request")
			});

		_binanceApiClient.GetType().GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _binanceApiClient.GetAccountTradeLisAsync("BTCUSDT"));
	}

	[Fact]
	public async Task GetAccountTradeLis_ReturnsValidTradeList()
	{
		// Arrange
		var jsonResponse = await GetBianceApiTestFileContent("GetAccountTradeLisHttpTestResult");

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);
		var Data = await _binanceApiClient.GetAccountTradeLisAsync("XRPUSDT");
		var jsonData = JsonConvert.DeserializeObject<List<AccountTradeListDto>>(jsonResponse);
		// Act & Assert
		Assert.Equal(jsonData!.Count, Data.Count);
		Assert.Equal(jsonData[0].Id, Data[0].Id);
	}
	#endregion

	#region GetAccountTradeLastPairOperation
	[Fact]
	public async Task GetAccountTradeLastPairOperation_ReturnsLastTradeInfo()
	{
		// Arrange
		var jsonResponse = await GetBianceApiTestFileContent("GetAccountTradeLastPairOperationTestResult.json");

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		var Data = await _binanceApiClient.GetAccountTradeLastPairOperationAsync("XRPUSDT");

		var jsonData = JsonConvert.DeserializeObject<List<AccountTradeListDto>>(jsonResponse);
		AccountTradeListDto element = jsonData!.First();
		// Act & Assert
		Assert.Equivalent(element, Data);
	}
	#endregion

	#region GetSymbolCurrentPrice
	[Fact]
	public async Task GetSymbolCurrentPrice_ReturnsValidPrice()
	{
		// Arrange
		var symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetSymbolCurrentPrice");
		var mockResponse = JsonConvert.DeserializeObject<PairPriceTickerDto>(jsonResponse);

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClientNoAuth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		// Act
		var result = await _binanceApiClient.GetSymbolCurrentPriceAsync(symbol);

		// Assert
		Assert.NotNull(result);
		Assert.Equivalent(mockResponse, result);
	}

	#endregion

	#region GetSymbolAvgPrice
	[Fact]
	public async Task GetSymbolAvgPrice_ReturnValidPrice()
	{
		// Arrange
		var symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetSymbolAvgPrice");
		var mockResponse = JsonConvert.DeserializeObject<PairAvgPriceDTo>(jsonResponse);

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClientNoAuth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		// Act
		var result = await _binanceApiClient.GetSymbolAvgPriceAsync(symbol);

		// Assert
		Assert.NotNull(result);
		Assert.Equivalent(mockResponse, result);
	}
	#endregion

	#region GetHistoricalDataAsync
	[Fact]
	public async Task GetHistoricalDataAsync_ReturnsValidData()
	{
		// Arrange
		var symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetHistoricalDataAsync");

		var mockResponse = JsonConvert.DeserializeObject<List<List<object>>>(jsonResponse)!.Select(kline => new KlineDataDto
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

		_mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
			});

		_binanceApiClient.GetType().GetField("_httpClientNoAuth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_binanceApiClient, _mockHttpClient);

		// Act
		var result = await _binanceApiClient.GetHistoricalDataAsync("BTCUSDT","1h", 168);

		// Assert
		Assert.NotNull(result);
		Assert.Equivalent(mockResponse, result);
	}
	#endregion

	#region InitializeAsync
	[Fact]
	public async Task InitializeAsync_GetKeysValidationInfo_ReturnTrue()
	{
		await _binanceApiClient.InitializeAsync();
		Assert.True(_binanceApiClient.GetKeysValidationInfo());
	}
	#endregion
}

