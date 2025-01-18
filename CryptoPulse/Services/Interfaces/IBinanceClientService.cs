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
	public Task<PairPriceTicker> GetSymbolCurrentPrice(string symbol, CancellationTokenSource token);
	public Task<AccountTrade> GetAccountTradeLastPairOperation(string symbol);
	public Task<PairAvgPrice> GetSymbolAvgPrice(string symbol);
	public Task<List<KlineData>> GetHistoricalDataAsync(string symbol, string interval, int limit);
}
