using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using MyCryptocurrency.Infrastructure.Services.Interfaces;

namespace MyCryptocurrency.ViewModels;
public partial class KeyInputViewModel: ObservableObject
{
	private readonly ISecureStorageService _storageService;
	[ObservableProperty] private string _apiKey;
	[ObservableProperty] private string _privateKey;
	private readonly IBinanceApiClient _binanceApiClient;


	public KeyInputViewModel(ISecureStorageService secureStorageService, IBinanceApiClient binanceApiClient)
	{
		_storageService = secureStorageService;
		_binanceApiClient = binanceApiClient;
		GetKeys();
	}

	public async void GetKeys()
	{
		ApiKey = await _storageService.GetApiKeyAsync();
		PrivateKey = await _storageService.GetApiPrivateKeyAsync();
	}


	[RelayCommand]
	public async Task SaveApiKeyClickAsync()
	{
		if (string.IsNullOrWhiteSpace(ApiKey))
		{
			ShowBannerMessage("API Key cannot be empty.");
		}

		try
		{
			await _storageService.SaveApiKeyAsync(ApiKey);
			await _binanceApiClient.SetNewKeyApiValue(ApiKey);
			ShowBannerMessage("ApiKey saved successfully!");
		}
		catch (Exception ex)
		{
			ShowBannerMessage($"Error saving keys: {ex.Message}");
		}
	}

	[RelayCommand]
	public async Task SavePrivateKeyClickAsync()
	{
		if (string.IsNullOrWhiteSpace(PrivateKey))
		{
			ShowBannerMessage("Private Key cannot be empty.");
		}

		try
		{
			await _storageService.SaveApiPrivateKeyAsync(PrivateKey);
			await _binanceApiClient.SetNewPrivateKeyValue(PrivateKey);
			ShowBannerMessage("PrivateKey saved successfully!");
		}
		catch (Exception ex)
		{

			ShowBannerMessage($"Error saving keys: {ex.Message}");
		}
	}

	[RelayCommand]
	public async Task BackToMainViewwClickAsync()
	{
		try
		{
			await Shell.Current.GoToAsync("..");
		}

		catch (Exception e)
		{
			Console.WriteLine(e);
			ShowBannerMessage($"Error: {e.Message}");
		}
	}

	private async void ShowBannerMessage(string message)
	{
		await Application.Current.MainPage.DisplayAlert("Rezultat", message, "Ok");
	}
}
