using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using MyCryptocurrency.Views;

namespace MyCryptocurrency.ViewModels;

[ObservableObject]
public partial class MainPageViewModel
{
	[ObservableProperty] private bool _activityIndicatorIsRunning = true;
	private readonly IBinanceClientService _bianceClientService;
	private readonly IDatabaseService _databaseService;
	private CancellationTokenSource _cancellationTokenSource;
	private readonly IDispatcher _dispatcher;
	public ObservableRangeCollection<CryptocurrencyPair> CryptoPairs { get; } = new ObservableRangeCollection<CryptocurrencyPair>();

	[ObservableProperty] bool _addNewPairMode = false;

	public MainPageViewModel(IBinanceClientService bianceClientService, IDatabaseService databaseService, IDispatcher dispatcher)
	{
		_bianceClientService = bianceClientService;
		_databaseService = databaseService;
		_dispatcher = dispatcher;
	}

	public void GetCryptoPairs()
	{
		Task.Run(async () =>
		{
			ActivityIndicatorIsRunning = true;
			var pairs = await _databaseService.GetPairsAsync();
			CryptoPairs.ReplaceRange(pairs.OrderBy(x => x.CurrencyName1).ToList());
			ActivityIndicatorIsRunning = false;
		});
	}

	[RelayCommand]
	public async Task ShowDetails(CryptocurrencyPair pair)
	{
		await Shell.Current.GoToAsync(nameof(TradeListDetailsPage), new Dictionary<string, object>
		{
			{ "SelectedPair", pair }
		});
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
					await Task.Delay(2000, _cancellationTokenSource.Token); // Wait for 5 seconds
				}
				catch (TaskCanceledException)
				{ }
			}
		});
	}

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
}
