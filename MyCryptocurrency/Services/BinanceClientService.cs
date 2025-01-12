using AutoMapper;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;

namespace MyCryptocurrency.Services;
public class BinanceClientService: IBinanceClientService
{
	private readonly IBinanceApiClient _binanceApiClient;
	private readonly IMapper _mapper;
	public BinanceClientService(IMapper mapper, IBinanceApiClient bianceApiService)
	{
		_mapper = mapper;
		_binanceApiClient = bianceApiService;
	}

	public async Task<List<AccountTradeList>> GetAccountTradeList(string pair)
	{
		var res = await _binanceApiClient.GetAccountTradeLis(pair);
		return _mapper.Map<List<AccountTradeList>>(res);
	}
}
