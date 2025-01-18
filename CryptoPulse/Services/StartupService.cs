using CryptoPulse.Infrastructure.Services.Interfaces;
using CryptoPulse.Database.Service.Interfaces;
using CryptoPulse.BianceApi.Services.Interfaces;
namespace CryptoPulse.Infrastructure.Services;
public class StartupService : IStartupService
{
	private readonly IDatabaseClientService _databaseClientService;
	private readonly IBinanceApiClient _bianceApiService;

	public StartupService(IDatabaseClientService databaseClientService, IBinanceApiClient binanceApiClient)
	{
		_databaseClientService = databaseClientService;
		_bianceApiService = binanceApiClient;
	}
	public async Task InitializeAsync()
	{
		await _databaseClientService.InitializeAsync();
		await _bianceApiService.InitializeAsync();
		AppSettings.IsValidKeys = _bianceApiService.GetKeysValidationInfo();
	}
}
