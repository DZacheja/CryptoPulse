using AutoMapper;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Infrastructure.Services.Interfaces;
using CryptoPulse.Models;
using CryptoPulse.Services.Interfaces;

namespace CryptoPulse.Services;
public class BinanceClientService: IBinanceClientService
{
	private readonly IBinanceApiClient _binanceApiClient;
	private readonly IMapper _mapper;
	public BinanceClientService(IMapper mapper, IBinanceApiClient bianceApiService)
	{
		_mapper = mapper;
		_binanceApiClient = bianceApiService;
	}

	public async Task<AccountTrade> GetAccountTradeLastPairOperation(string symbol)
	{
		var res = await _binanceApiClient.GetAccountTradeLastPairOperation(symbol);
		return _mapper.Map<AccountTrade>(res);
	}

	public async Task<List<AccountTrade>> GetAccountTradeList(string pair)
	{
		var res = await _binanceApiClient.GetAccountTradeLis(pair);
		return _mapper.Map<List<AccountTrade>>(res);
	}

	public async Task<List<KlineData>> GetHistoricalDataAsync(string symbol, string interval, int limit)
	{
		var res = await _binanceApiClient.GetHistoricalDataAsync(symbol, interval ,limit);
		return _mapper.Map<List<KlineData>>(res);
	}

	public async Task<PairAvgPrice> GetSymbolAvgPrice(string symbol)
	{
		var res = await _binanceApiClient.GetSymbolAvgPrice(symbol);
		return _mapper.Map<PairAvgPrice>(res);
	}

	public async Task<PairPriceTicker> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token)
	{
		var res = await _binanceApiClient.GetSymbolCurrentPrice(symbol, token);
		return _mapper.Map<PairPriceTicker>(res);
	}
}
