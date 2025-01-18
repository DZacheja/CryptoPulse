using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using CryptoPulse.Helpers;
using CryptoPulse.Models;
using CryptoPulse.Services.Interfaces;
using System.Collections.ObjectModel;

namespace CryptoPulse.ViewModels;

[QueryProperty(nameof(CurrencyPair), "SelectedPair")]
public partial class TradeDetailsViewModel: CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
	private readonly IBinanceClientService _bianceClientService;

	[ObservableProperty] public partial string Pair { get; set; } = string.Empty;
	[ObservableProperty] public partial string CurrencyName1 { get; set; } = string.Empty;
	[ObservableProperty] public partial string CurrencyName2 { get; set; } = string.Empty;
	[ObservableProperty] public partial bool ActivityIndicatorIsRunning { get; set; } = true;

	private CryptocurrencyPair _currencyPair = new CryptocurrencyPair();

	public CryptocurrencyPair CurrencyPair
	{
		get => _currencyPair;
		set
		{
			if (value != null)
			{
				Pair = value.Symbol;
				CurrencyName1 = value.CurrencyName1;
				CurrencyName2 = value.CurrencyName2;
				ShowTransactionHistory();
			}

			_currencyPair = value ?? throw new NullReferenceException();
		}
	}

	public ObservableCollection<AccountTrade> BuyTransactionHistory { get; set; } = new ObservableCollection<AccountTrade>();
	public ObservableCollection<AccountTrade> SellTransactionHistory { get; set; } = new ObservableCollection<AccountTrade>();
	public ObservableRangeCollection<AccountTrade> TransactionHistory { get; set; } = new ObservableRangeCollection<AccountTrade>();

	public TradeDetailsViewModel(IBinanceClientService binanceClientService)
	{
		_bianceClientService = binanceClientService;
	}

	public async void ShowTransactionHistory()
	{
		if (string.IsNullOrEmpty(Pair))
			return;

		ActivityIndicatorIsRunning = true;
		BuyTransactionHistory.Clear();
		SellTransactionHistory.Clear();
		try
		{

			var trades = await _bianceClientService.GetAccountTradeList(Pair);
			TransactionHistory.AddRange(trades.OrderByDescending(x => x.Time).ToList());
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
			await SnackbarHelper.ShowSnackbarAsync($"Błąd podczas pobierania danych: {ex.Message}");
		}
		ActivityIndicatorIsRunning = false;
	}
}
