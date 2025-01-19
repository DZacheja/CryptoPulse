using AutoMapper;
using CryptoPulse.Database.DTO;
using CryptoPulse.Database.Service.Interfaces;
using CryptoPulse.Models;
using CryptoPulse.Services.Interfaces;

namespace CryptoPulse.Services;
public class DatabaseService : IDatabaseService
{
	private readonly IDatabaseClientService _databaseClient;
	private readonly IBinanceClientService _bianceClient;
	private readonly IMapper _mapper;

	public DatabaseService(IDatabaseClientService databaseClientService, IMapper mapper, IBinanceClientService binanceClient)
	{
		_databaseClient = databaseClientService;
		_bianceClient = binanceClient;
		_mapper = mapper;
	}
	public async Task<int> AddPairAsync(CryptocurrencyPair pair)
	{
		return await _databaseClient.SavePairAsync(_mapper.Map<CryptocurrencyPairDto>(pair));
	}

	public async Task<int> DeletePairAsync(CryptocurrencyPair pair)
	{
		return await _databaseClient.DeletePairAsync(_mapper.Map<CryptocurrencyPairDto>(pair));
	}

	public async Task<List<CryptocurrencyPair>> GetPairsAsync()
	{
		var pairs = await _databaseClient.GetPairsAsync();
		var mappedPairs = _mapper.Map<List<CryptocurrencyPair>>(pairs);
		foreach (var pair in mappedPairs)
		{
			pair.LastOperation = await _bianceClient.GetAccountTradeLastPairOperationAsync(pair.Symbol);
			if(pair.LastOperation != null)
			{
				pair.LastOperationPair = pair.LastOperation.IsBuyer ? $"{pair.CurrencyName2} ► {pair.CurrencyName1}" : $"{pair.CurrencyName1} ► {pair.CurrencyName2}";
				pair.FoundLastOperation = true;
			}
			else
			{
				pair.LastOperation = new AccountTrade() { IsBuyer = false, Symbol = "NoTades", Price = 0 };
				pair.FoundLastOperation = false;
			}
		}
		return mappedPairs;
	}
}
