using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Helpers;
using CryptoPulse.Infrastructure.Services.Interfaces;
using Microsoft.Maui.Platform;

namespace CryptoPulse.ViewModels;
public partial class KeyInputViewModel: ObservableObject
{
	[ObservableProperty] public partial bool ActivityIndicatorIsRunning { get; set; } = true;

	private readonly ISecureStorageService _storageService;
	[ObservableProperty] public partial string ApiKey {  get; set; } = string.Empty;
	[ObservableProperty] public partial string PrivateKey { get; set; } = string.Empty;
	[ObservableProperty] public partial string BackgroundImage { get; set; } = "Resources\\Splash\\mysplash.svg";

	private readonly IBinanceApiClient _binanceApiClient;


	public KeyInputViewModel(ISecureStorageService secureStorageService, IBinanceApiClient binanceApiClient)
	{
		_storageService = secureStorageService;
		_binanceApiClient = binanceApiClient;
	}

	public void Initialize()
	{
		ActivityIndicatorIsRunning = true;
		bool validKeys = false;
		Task.Run(async () =>
		{
			ApiKey = await _storageService.GetApiKeyAsync() ?? string.Empty;
			PrivateKey = await _storageService.GetApiPrivateKeyAsync() ?? string.Empty;
			validKeys =  await _binanceApiClient.ChceckUserKeysValidationAsync(ApiKey, PrivateKey, true);
			
		}).Wait();

		if (validKeys)
		{
			ActivityIndicatorIsRunning = false;
			Application.Current!.Windows[0].Page = new AppShell();
		}
		ActivityIndicatorIsRunning = false;
		BackgroundImage = "";

	}

	[RelayCommand]
	public async Task SaveKeys()
	{
		if (string.IsNullOrWhiteSpace(ApiKey) || string.IsNullOrWhiteSpace(PrivateKey))
		{
			await Application.Current!.Windows[0].Page!.DisplayAlert("Alert", "API Key cannot be empty.", "OK");
			return;
		}

		bool validKeys = await _binanceApiClient.ChceckUserKeysValidationAsync(ApiKey, PrivateKey);
		if (!validKeys)
		{
			await Application.Current!.Windows[0].Page!.DisplayAlert("Alert", "The provided api key or private key is not correct!", "OK");
			return;
		}

		try
		{
			await _storageService.SaveApiKeyAndPriveteKeyAsync(ApiKey, PrivateKey);
			await SnackbarHelper.ShowSnackbarAsync("ApiKey saved successfully!");

			Application.Current!.Windows[0].Page = new AppShell();
		}
		catch (Exception ex)
		{
			await SnackbarHelper.ShowSnackbarAsync($"Error saving keys: {ex.Message}");
		}
	}
}
