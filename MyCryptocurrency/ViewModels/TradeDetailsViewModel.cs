using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyCryptocurrency.ViewModels;

[ObservableObject]
[QueryProperty(nameof(CurrencyPair), "SelectedPair")]
public partial class TradeDetailsViewModel
{
	private readonly IBinanceClientService _bianceClientService;
	[ObservableProperty] private string _pair;
	[ObservableProperty] private string _currencyName1;
	[ObservableProperty] private string _currencyName2;
	[ObservableProperty] private bool _activityIndicatorIsRunning = true;

	private CryptocurrencyPair _currencyPair;
	public CryptocurrencyPair CurrencyPair
	{
		get => _currencyPair;
		set
		{
			Pair = value.Symbol;
			CurrencyName1 = value.CurrencyName1;
			CurrencyName2 = value.CurrencyName2;
			ShowTransactionHistory();
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
			ShowBannerMessage($"Błąd podczas pobierania danych: {ex.Message}");
		}
		ActivityIndicatorIsRunning = false;
	}

	private async void ShowBannerMessage(string message)
	{
		await Application.Current.MainPage.DisplayAlert("Rezultat", message, "Ok");
	}
}
