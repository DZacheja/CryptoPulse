using CryptoPulse.BianceApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.BianceApi.Services.Interfaces;
public interface IBinanceApiClient
{
	public Task InitializeAsync();
	public Task<List<AccountTradeListDto>> GetAccountTradeLisAsync(string symbol);
	public Task<AccountTradeListDto> GetAccountTradeLastPairOperationAsync(string symbol);
	public Task<PairPriceTickerDto> GetSymbolCurrentPriceAsync(string symbol);
	public Task<PairAvgPriceDTo> GetSymbolAvgPriceAsync(string symbol);
	public Task<List<KlineDataDto>> GetHistoricalDataAsync(string symbol, string interval, int limit);
	public Task<bool> ChceckUserKeysValidationAsync(string apiKey, string privateKey, bool applayNewKeys = false);
	public bool GetKeysValidationInfo();
}
