using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using MyCryptocurrency.Views;
using System.Collections.ObjectModel;

namespace MyCryptocurrency.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
	[ObservableProperty] private bool _activityIndicatorIsRunning = true;
	private readonly IBinanceClientService _bianceClientService;
	private readonly IDatabaseService _databaseService;
	private CancellationTokenSource _cancellationTokenSource;
	private readonly IDispatcher _dispatcher;
	public ObservableCollection<CryptocurrencyPair> CryptoPairs { get; } = new ObservableCollection<CryptocurrencyPair>();

	[ObservableProperty] CryptocurrencyPair _newCryptoPair = new CryptocurrencyPair();

	public MainPageViewModel(IBinanceClientService bianceClientService, IDatabaseService databaseService, IDispatcher dispatcher)
	{
		_bianceClientService = bianceClientService;
		_databaseService = databaseService;
		_dispatcher = dispatcher;
		GetCryptoPairs();
	}

	private async void GetCryptoPairs()
	{
		ActivityIndicatorIsRunning = true;
		var pairs = await _databaseService.GetPairsAsync();
		CryptoPairs.Clear();
		foreach (var pair in pairs.OrderBy(x => x.OrderID))
			CryptoPairs.Add(pair);

		ActivityIndicatorIsRunning = false;
	}

	[RelayCommand]
	public async Task AddCryptoPair()
	{
		if (NewCryptoPair == null || string.IsNullOrEmpty(NewCryptoPair.CurrencyName1) || string.IsNullOrEmpty(NewCryptoPair.CurrencyName2))
		{
			ShowBannerMessage("Uzupełnij wszytkie dane na oknie!");
			return;
		}
		try
		{
			_cancellationTokenSource.Cancel();
			CryptoPairs.Clear();
			_activityIndicatorIsRunning = true;
			int i = await _databaseService.AddPairAsync(NewCryptoPair);
			GetCryptoPairs();
			_activityIndicatorIsRunning = false;
			StartUpdatingPrices();
		}
		catch (Exception ex)
		{
			_activityIndicatorIsRunning = true;
			ShowBannerMessage($"Błąd: {ex.Message}");
			GetCryptoPairs();
			StartUpdatingPrices();
		}
	}

	[RelayCommand]
	public async Task ShowDetails(CryptocurrencyPair pair)
	{
		await Shell.Current.GoToAsync(nameof(TradeListDetailsPage), new Dictionary<string, object>
		{
			{ "SelectedPair", pair }
		});
	}

	[RelayCommand]
	public async Task DeletePair(CryptocurrencyPair pair)
	{
		_cancellationTokenSource?.Cancel();
		await _databaseService.DeletePairAsync(pair);
		GetCryptoPairs();
		StartUpdatingPrices();
	}

	public void StartUpdatingPrices()
	{
		_cancellationTokenSource = new CancellationTokenSource();

		_dispatcher.Dispatch(async () =>
		{
			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					await UpdateAveragePrices(_cancellationTokenSource);
					await Task.Delay(5000, _cancellationTokenSource.Token); // Wait for 5 seconds
				}
				catch (TaskCanceledException)
				{ }
			}
		});
	}

	[RelayCommand]
	private async Task UpdateAveragePrices(CancellationTokenSource cancellationToken)
	{
		foreach (var pair in CryptoPairs)
		{
			try
			{
				if (!_cancellationTokenSource.IsCancellationRequested && !ActivityIndicatorIsRunning)
				{
					var currPrice = await _bianceClientService.GetSymbolCurrentPrice(pair.Symbol, cancellationToken);
					pair.LastPrice = pair.CurrentExchangeRate;
					pair.CurrentExchangeRate = currPrice.Price;
				}
				else
				{
					return;
				}
			}
			catch (TaskCanceledException)
			{ }
			catch (Exception ex)
			{ }
		}
		OnPropertyChanged(nameof(CryptoPairs));
	}

	public void StopUpdatingPrices()
	{
		_cancellationTokenSource?.Cancel();
	}

	private async void ShowBannerMessage(string message)
	{
		await Application.Current.MainPage.DisplayAlert("Rezultat", message, "Ok");
	}
}
