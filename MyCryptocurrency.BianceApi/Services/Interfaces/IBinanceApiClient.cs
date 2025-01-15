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
	public void SetNewKeyApiValue(string apiKey);
	public void SetNewPrivateKeyValue(string privateKey);
	public Task<PairPriceTickerDto> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token);
	public Task<PairAvgPriceDTo> GetSymbolAvgPrice(string symbol);
	public Task<List<KlineDataDto>> GetHistoricalDataAsync(string symbol, string interval, int limit);
}
