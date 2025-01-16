using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using MyCryptocurrency.Helpers;
using MyCryptocurrency.Infrastructure.Services.Interfaces;

namespace MyCryptocurrency.ViewModels;
public partial class KeyInputViewModel: ObservableObject
{
	private readonly ISecureStorageService _storageService;
	[ObservableProperty] public partial string ApiKey {  get; set; } = string.Empty;
	[ObservableProperty] public partial string PrivateKey { get; set; } = string.Empty;

	private readonly IBinanceApiClient _binanceApiClient;


	public KeyInputViewModel(ISecureStorageService secureStorageService, IBinanceApiClient binanceApiClient)
	{
		_storageService = secureStorageService;
		_binanceApiClient = binanceApiClient;
		GetKeys();
	}

	public async void GetKeys()
	{
		ApiKey = await _storageService.GetApiKeyAsync()	?? string.Empty;
		PrivateKey = await _storageService.GetApiPrivateKeyAsync() ?? string.Empty;
	}


	[RelayCommand]
	public async Task SaveApiKeyClickAsync()
	{
		if (string.IsNullOrWhiteSpace(ApiKey))
		{
			await SnackbarHelper.ShowSnackbarAsync("API Key cannot be empty.");
		}

		try
		{
			await _storageService.SaveApiKeyAsync(ApiKey);
			_binanceApiClient.SetNewKeyApiValue(ApiKey);
			await SnackbarHelper.ShowSnackbarAsync("ApiKey saved successfully!");
		}
		catch (Exception ex)
		{
			await SnackbarHelper.ShowSnackbarAsync($"Error saving keys: {ex.Message}");
		}
	}

	[RelayCommand]
	public async Task SavePrivateKeyClickAsync()
	{
		if (string.IsNullOrWhiteSpace(PrivateKey))
		{
			await SnackbarHelper.ShowSnackbarAsync("Private Key cannot be empty.");
		}

		try
		{
			await _storageService.SaveApiPrivateKeyAsync(PrivateKey);
			_binanceApiClient.SetNewPrivateKeyValue(PrivateKey);
			await SnackbarHelper.ShowSnackbarAsync("PrivateKey saved successfully!");
		}
		catch (Exception ex)
		{

			await SnackbarHelper.ShowSnackbarAsync($"Error saving keys: {ex.Message}");
		}
	}
}
