using MyCryptocurrency.BianceApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.BianceApi.Services.Interfaces;
public interface IBinanceApiClient
{
	public Task<List<AccountTradeListDto>> GetAccountTradeLis(string symbol);
	public Task<AccountTradeListDto> GetAccountTradeLastPairOperation(string symbol);
	public Task SetNewKeyApiValue(string apiKey);
	public Task SetNewPrivateKeyValue(string privateKey);
	public Task<PairPriceTickerDto> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token);
	public Task<PairAvgPriceDTo> GetSymbolAvgPrice(string symbol);
}
