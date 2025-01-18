using CryptoPulse.Database.DTO;

namespace CryptoPulse.Database.Service.Interfaces;
public interface IDatabaseClientService
{
	public Task<List<CryptocurrencyPairDto>> GetPairsAsync();

	public Task<int> SavePairAsync(CryptocurrencyPairDto pair);

	public Task<int> DeletePairAsync(CryptocurrencyPairDto pair);
	public Task InitializeAsync();
}
