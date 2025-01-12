using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using MyCryptocurrency.Views;
using System.Collections.ObjectModel;

namespace MyCryptocurrency.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
	private readonly IBinanceClientService _binanceApiClient;
	private readonly ISecureStorageService _secureStorageService;
	private readonly IServiceProvider _serviceProvider;
	public ObservableCollection<string> CryptoPairs { get; set; } = new ObservableCollection<string>();
	public ObservableCollection<AccountTradeList> BuyTransactionHistory { get; set; } = new ObservableCollection<AccountTradeList>();
	public ObservableCollection<AccountTradeList> SellTransactionHistory { get; set; } = new ObservableCollection<AccountTradeList>();

	public MainPageViewModel(IBinanceClientService bianceClientService, ISecureStorageService secureStorageService, IServiceProvider serviceProvider)
	{
		_binanceApiClient = bianceClientService;
		_secureStorageService = secureStorageService;
		_serviceProvider = serviceProvider;
		CryptoPairs.Add("BTCUSDT");
		CryptoPairs.Add("XRPUSDT");
		CryptoPairs.Add("ETHUSDT");
	}

	[RelayCommand]
	public async Task AddCryptoPair(string pair)
	{
		if (!string.IsNullOrEmpty(pair))
		{
			CryptoPairs.Add(pair.ToUpper());
		}
	}

	[RelayCommand]
	public async Task ShowTransactionHistory(string pair)
	{
		BuyTransactionHistory.Clear();
		SellTransactionHistory.Clear();
		try
		{

			var trades = await _binanceApiClient.GetAccountTradeList(pair);
			foreach (var trade in trades)
			{
				// Add to Buy or Sell collection based on IsBuyer
				if (trade.IsBuyer)
				{
					BuyTransactionHistory.Add(trade);
				}
				else
				{
					SellTransactionHistory.Add(trade);
				}
			}
		}
		catch (Exception ex)
		{
			ShowBannerMessage($"Błąd podczas pobierania danych: {ex.Message}");
		}
	}

	[RelayCommand]
	public async Task NavigateToKeySettings()
	{
		try
		{
			await Shell.Current.GoToAsync(nameof(KeyInputPage));
		}

		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}
	private async void ShowBannerMessage(string message)
	{
		await Application.Current.MainPage.DisplayAlert("Rezultat", message, "Ok");
	}
}
