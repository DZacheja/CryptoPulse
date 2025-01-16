using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using MyCryptocurrency.Helpers;
using MyCryptocurrency.Models;
using MyCryptocurrency.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCryptocurrency.ViewModels
{
	public partial class ManagePairsViewModel:CommunityToolkit.Mvvm.ComponentModel.ObservableObject
	{
		private IDatabaseService _databaseService { get; set; }
		[ObservableProperty] public partial bool ActivityIndicatorIsRunning { get; set; } = true;
		[ObservableProperty] public partial CryptocurrencyPair NewCryptoPair { get; set; } = new CryptocurrencyPair();
		public ObservableRangeCollection<CryptocurrencyPair> CryptoPairs { get; } = new ObservableRangeCollection<CryptocurrencyPair>();

		public ManagePairsViewModel(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
			GetCryptoPairs();
		}

		private void GetCryptoPairs()
		{
			Task.Run(async () =>
			{
				ActivityIndicatorIsRunning = true;
				var pairs = await _databaseService.GetPairsAsync();
				CryptoPairs.ReplaceRange(pairs);
				ActivityIndicatorIsRunning = false;
			});
		}

		[RelayCommand]
		public async Task DeletePair(CryptocurrencyPair pair)
		{
			ActivityIndicatorIsRunning = true;
			await _databaseService.DeletePairAsync(pair);
			ActivityIndicatorIsRunning = false;
			GetCryptoPairs();
		}

		[RelayCommand]
		public async Task AddCryptoPair()
		{
			if (NewCryptoPair == null || string.IsNullOrEmpty(NewCryptoPair.CurrencyName1) || string.IsNullOrEmpty(NewCryptoPair.CurrencyName2))
			{
				await SnackbarHelper.ShowSnackbarAsync("Uzupełnij wszytkie dane na oknie!");
				return;
			}
			try
			{
				CryptoPairs.Clear();
				ActivityIndicatorIsRunning = true;
				int i = await _databaseService.AddPairAsync(NewCryptoPair);
				GetCryptoPairs();
				ActivityIndicatorIsRunning = false;
				await SnackbarHelper.ShowSnackbarAsync($"Pomyślnie dodano parę {NewCryptoPair.CurrencyName1} / {NewCryptoPair.CurrencyName2}");

			}
			catch (Exception ex)
			{
				await SnackbarHelper.ShowSnackbarAsync($"Błąd: {ex.Message}");
				GetCryptoPairs();
			}
		}
	}
}
