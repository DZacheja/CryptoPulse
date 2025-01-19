using CryptoPulse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Services.Interfaces;
public interface IBinanceClientService
{
	public Task<List<AccountTrade>> GetAccountTradeList(string pair);
	public Task<PairPriceTicker> GetSymbolCurrentPriceAsync(string symbol);
	public Task<AccountTrade> GetAccountTradeLastPairOperationAsync(string symbol);
	public Task<PairAvgPrice> GetSymbolAvgPriceAsync(string symbol);
	public Task<List<KlineData>> GetHistoricalDataAsync(string symbol, string interval, int limit);
}
