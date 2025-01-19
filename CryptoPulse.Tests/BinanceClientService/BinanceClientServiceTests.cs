using AutoMapper;
using CryptoPulse.BianceApi.DTOs;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Infrastructure.Services.Interfaces;
using CryptoPulse.Mapping;
using CryptoPulse.Models;
using CryptoPulse.Services;
using Moq;
using Newtonsoft.Json;
using System.Globalization;

namespace CryptoPulse.Tests.BianceApiClient;
public class BinanceClientServiceTests: BaseTest
{
	private readonly Mock<ISecureStorageService> _mockSecureStorage;
	private readonly Mock<IBinanceApiClient> _mockBinanceApiClient;
	private readonly BinanceClientService _binanceClientService;
	private readonly IMapper _mapper;
	public BinanceClientServiceTests()
	{
		_mockBinanceApiClient = new Mock<IBinanceApiClient>();
		_mockSecureStorage = new Mock<ISecureStorageService>();
		_mockSecureStorage.Setup(x => x.GetApiKeyAsync()).ReturnsAsync("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyAsync()).ReturnsAsync("testApiSecret");
		_mockSecureStorage.Setup(x => x.GetApiKeyCashed()).Returns("testApiKey");
		_mockSecureStorage.Setup(x => x.GetApiPrivateKeyCashed()).Returns("testApiSecret");

		var config = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile(new MappingProfiles());
		});
		_mapper = new Mapper(config);

		_binanceClientService = new BinanceClientService(_mapper, _mockBinanceApiClient.Object);
	}

	#region GetAccountTradeList
	[Fact]
	public async Task GetAccountTradeList_ReturnValidTradeList()
	{
		// Arrange
		string symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetAccountTradeLisHttpTestResult.json");

		var jsonData = JsonConvert.DeserializeObject<List<AccountTradeListDto>>(jsonResponse);
		_mockBinanceApiClient.Setup(x => x.GetAccountTradeLisAsync(symbol)).ReturnsAsync(jsonData!);
		Assert.NotNull(jsonData);

		// Act
		List<AccountTrade> result = await _binanceClientService.GetAccountTradeList(symbol);
		var firstJson = jsonData.First();
		var firstResult = result.First();

		Assert.NotNull(result);
		Assert.Equal(firstJson.Qty, firstResult.Qty);
		Assert.Equal(firstJson.Commission, firstJson.Commission);
		var mappedTime = DateTimeOffset.FromUnixTimeMilliseconds(firstJson.Time).DateTime.ToLocalTime();
		Assert.Equal(mappedTime, firstResult.Time);
	}
	#endregion
	#region GetSymbolCurrentPrice
	[Fact]
	public async Task GetSymbolCurrentPrice_ReturnValidCurrentPrice()
	{
		// Arrange
		string symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetSymbolCurrentPrice");

		var jsonData = JsonConvert.DeserializeObject<PairPriceTickerDto>(jsonResponse);
		_mockBinanceApiClient.Setup(x => x.GetSymbolCurrentPriceAsync(symbol)).ReturnsAsync(jsonData!);
		Assert.NotNull(jsonData);

		// Act
		PairPriceTicker result = await _binanceClientService.GetSymbolCurrentPriceAsync(symbol);

		Assert.NotNull(result);
		Assert.Equal(result.Price, jsonData.Price);
		Assert.Equal(result.Symbol, jsonData.Symbol);

	}
	#endregion

	#region GetAccountTradeLastPairOperationAsync
	[Fact]
	public async Task GetAccountTradeLastPairOperationAsync_ReturnValidAccountTrade()
	{
		// Arrange
		string symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetAccountTradeLastPairOperationTestResult");

		var jsonData = JsonConvert.DeserializeObject<List<AccountTradeListDto>>(jsonResponse);
		var jsonDataFirstElement = jsonData!.First();
		_mockBinanceApiClient.Setup(x => x.GetAccountTradeLastPairOperationAsync(symbol)).ReturnsAsync(jsonDataFirstElement);
		Assert.NotNull(jsonData);

		// Act
		AccountTrade result = await _binanceClientService.GetAccountTradeLastPairOperationAsync(symbol);

		Assert.NotNull(result);
		Assert.Equal(result.Price, jsonDataFirstElement.Price);
		Assert.Equal(result.Symbol, jsonDataFirstElement.Symbol);

	}
	#endregion

	#region GetSymbolAvgPriceAsync
	[Fact]
	public async Task GetSymbolAvgPriceAsync_ReturnValidPairAvgPrice()
	{
		// Arrange
		string symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetSymbolAvgPrice");

		var jsonData = JsonConvert.DeserializeObject<PairAvgPriceDTo>(jsonResponse);
		_mockBinanceApiClient.Setup(x => x.GetSymbolAvgPriceAsync(symbol)).ReturnsAsync(jsonData!);
		Assert.NotNull(jsonData);

		// Act
		PairAvgPrice result = await _binanceClientService.GetSymbolAvgPriceAsync(symbol);

		Assert.NotNull(result);
		Assert.Equal(result.Price, jsonData.Price);
		Assert.Equal(result.Mins, jsonData.Mins);
		Assert.Equal(result.CloseTime, jsonData.CloseTime);

	}
	#endregion

	#region GetHistoricalDataAsync
	[Fact]
	public async Task GetHistoricalDataAsync_ReturnValidPairAvgPrice()
	{
		// Arrange
		string symbol = "BTCUSDT";
		var jsonResponse = await GetBianceApiTestFileContent("GetHistoricalDataAsync");
		var jsonData = JsonConvert.DeserializeObject<List<List<object>>>(jsonResponse)!.Select(kline => new KlineDataDto
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

		_mockBinanceApiClient.Setup(x => x.GetHistoricalDataAsync(symbol, "1h", 168)).ReturnsAsync(jsonData!);
		Assert.NotNull(jsonData);

		// Act
		List<KlineData> result = await _binanceClientService.GetHistoricalDataAsync(symbol, "1h", 168);

		Assert.NotNull(result);
		KlineDataDto firstDto = jsonData.First();
		KlineData firstrMaped = result.First();

		var OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(firstDto.OpenTime).DateTime.ToLocalTime();
		var AvgTime = DateTimeOffset.FromUnixTimeMilliseconds((firstDto.OpenTime + firstDto.CloseTime) / 2).DateTime.ToLocalTime();
		var AvgTimeLng = (firstDto.OpenTime + firstDto.CloseTime) / 2;
		var AvgPrice = (firstDto.HighPrice + firstDto.LowPrice) / 2;
		Assert.Equal(OpenTime, firstrMaped.OpenTime);
		Assert.Equal(AvgTime, firstrMaped.AvgTime);
		Assert.Equal(AvgTimeLng, firstrMaped.AvgTimeLng);
		Assert.Equal(AvgPrice, firstrMaped.AvgPrice);
	}
	#endregion
}
