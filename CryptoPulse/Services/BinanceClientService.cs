using AutoMapper;
using CryptoPulse.BianceApi.Services.Interfaces;
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

	public async Task<List<Balance>> GetAccountBalance()
	{
		var accountInfo = await _binanceApiClient.GetAccountInfo();
		return _mapper.Map<List<Balance>>(accountInfo);
	}

	public async Task<AccountTrade> GetAccountTradeLastPairOperationAsync(string symbol)
	{
		var res = await _binanceApiClient.GetAccountTradeLastPairOperationAsync(symbol);
		return _mapper.Map<AccountTrade>(res);
	}

	public async Task<List<AccountTrade>> GetAccountTradeList(string pair)
	{
		var res = await _binanceApiClient.GetAccountTradeLisAsync(pair);
		return _mapper.Map<List<AccountTrade>>(res);
	}

	public async Task<List<KlineData>> GetHistoricalDataAsync(string symbol, string interval, int limit)
	{
		var res = await _binanceApiClient.GetHistoricalDataAsync(symbol, interval ,limit);
		return _mapper.Map<List<KlineData>>(res);
	}

	public async Task<PairAvgPrice> GetSymbolAvgPriceAsync(string symbol)
	{
		var res = await _binanceApiClient.GetSymbolAvgPriceAsync(symbol);
		return _mapper.Map<PairAvgPrice>(res);
	}

	public async Task<PairPriceTicker> GetSymbolCurrentPriceAsync(string symbol)
	{
		var res = await _binanceApiClient.GetSymbolCurrentPriceAsync(symbol);
		return _mapper.Map<PairPriceTicker>(res);
	}
}
