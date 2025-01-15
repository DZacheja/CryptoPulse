using Moq;
using Moq.Protected;
using MyCryptocurrency.BianceApi.DTOs;
using MyCryptocurrency.Infrastructure.Services;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Tests.BianceAPI;
public class BinanceApiClientTests
{
	private readonly Mock<ISecureStorageService> _mockSecureStorage;
	private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
	private readonly HttpClient _mockHttpClient;
	private readonly BinanceApiClient _binanceApiClient;

	public BinanceApiClientTests()
	{
		_mockSecureStorage = new Mock<ISecureStorageService>();
		_mockSecureStorage.Setup(x => x.GetApiKeyAsync()).ReturnsAsync("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyAsync()).ReturnsAsync("testApiSecret");
		_mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		_mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object)
		{
			BaseAddress = new Uri("https://api.binance.com")
		};

		// Initialize the BinanceApiClient with the mocked dependencies
		_binanceApiClient = new BinanceApiClient(_mockSecureStorage.Object);
	}

	[Fact]
	public async Task GetSymbolCurrentPrice_ReturnsValidPrice()
	{
		// Arrange
		var symbol = "BTCUSDT";
		var mockResponse = new PairPriceTickerDto { Symbol = symbol, Price = 50000.00m };
		var jsonResponse = JsonConvert.SerializeObject(mockResponse);

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
		var result = await _binanceApiClient.GetSymbolCurrentPrice(symbol, new CancellationTokenSource());

		// Assert
		Assert.NotNull(result);
		Assert.Equal(symbol, result.Symbol);
		Assert.Equal(50000.00m, result.Price);
	}

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
		await Assert.ThrowsAsync<Exception>(() => _binanceApiClient.GetAccountTradeLis("BTCUSDT"));
	}

	[Fact]
	public async Task GetAccountTradeLis_ReturnsValidTradeList()
	{
		// Arrange
		var projectDir = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
		var testDataPath = Path.Combine(projectDir, "BianceAPI", "OutputTestContent", "GetAccountTradeLisHttpTestResult.json");
		var jsonResponse = await File.ReadAllTextAsync(testDataPath);

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
		var Data = await _binanceApiClient.GetAccountTradeLis("XRPUSDT");
		var jsonData = JsonConvert.DeserializeObject<List<AccountTradeListDto>>(jsonResponse);
		// Act & Assert
		Assert.Equal(jsonData.Count, Data.Count);
		Assert.Equal(jsonData[0].Id, Data[0].Id);
	}

	[Fact]
	public async Task GetHistoricalDataAsync_ReturnsValidData()
	{
		// Arrange
		var symbol = "BTCUSDT";
		var interval = "1h";
		var limit = 10;
		var mockResponse = new List<List<object>>
		{
			new List<object> { 1627484400000, "40000.00", "41000.00", "39000.00", "40500.00", "1000", 1627488000000, "100000", 10, "500", "50000" }
		};
		var jsonResponse = JsonConvert.SerializeObject(mockResponse);

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
		var result = await _binanceApiClient.GetHistoricalDataAsync(symbol, interval, limit);

		// Assert
		Assert.NotNull(result);
		Assert.Single(result);
		Assert.Equal(40500.00M, result[0].ClosePrice);
	}
}

