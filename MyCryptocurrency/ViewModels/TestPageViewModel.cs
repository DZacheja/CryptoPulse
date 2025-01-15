using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MyCryptocurrency.ViewModels;
[ObservableObject]
public partial class TestPageViewModel
{
	private readonly IBinanceClientService _binanceClientService;
	public ObservableCollection<KlineData> KlineDataCollection = new ObservableCollection<KlineData>();
	public ObservableCollection<KlineData> TestDataCollection = new ObservableCollection<KlineData>();
	public TestPageViewModel(IBinanceClientService binanceClientService)
	{
			_binanceClientService = binanceClientService;
		var tst = GenerateTestData();
		foreach (var item in tst)
			TestDataCollection.Add(item);
		GetExchangeData();
	}

	private void GetExchangeData()
	{
		Task.Run(async () =>
		{
			var kilneData = await _binanceClientService.GetHistoricalDataAsync("BTCUSDT","1h", 168);
			foreach(var k in kilneData)
				KlineDataCollection.Add(k);
		});
		OnPropertyChanged(nameof(KlineDataCollection));
	}

	[RelayCommand]
	public async Task RefreshData()
	{
		GetExchangeData();
	}

	public List<KlineData> GenerateTestData()
	{
		var random = new Random();
		var testData = new List<KlineData>();

		for (int i = 0; i < 10; i++)
		{
			var openTime = DateTime.Now.AddMinutes(-i * 15); // 15-minute intervals for example
			var closeTime = openTime.AddMinutes(15);
			var avgTime = openTime.AddMinutes(7); // Halfway between open and close

			var openPrice = (decimal)random.NextDouble() * 100 + 1000; // Random price between 1000 and 1100
			var highPrice = openPrice + (decimal)random.NextDouble() * 50; // Random high
			var lowPrice = openPrice - (decimal)random.NextDouble() * 50; // Random low
			var closePrice = lowPrice + (decimal)random.NextDouble() * (highPrice - lowPrice); // Close within range
			var avgPrice = (openPrice + closePrice + highPrice + lowPrice) / 4; // Average of prices

			testData.Add(new KlineData
			{
				OpenTime = openTime,
				CloseTime = closeTime,
				AvgTime = avgTime,
				AvgTimeLng = avgTime.ToFileTime(),
				OpenPrice = openPrice,
				HighPrice = highPrice,
				LowPrice = lowPrice,
				ClosePrice = closePrice,
				AvgPrice = avgPrice
			});
		}

		return testData;
	}
}

