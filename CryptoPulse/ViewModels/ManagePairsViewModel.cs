using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using CryptoPulse.Helpers;
using CryptoPulse.Models;
using CryptoPulse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoPulse.ViewModels
{
	public partial class ManagePairsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
	{
		private IDatabaseService _databaseService { get; set; }
		[ObservableProperty] public partial bool ActivityIndicatorIsRunning { get; set; } = true;
		[ObservableProperty] public partial CryptocurrencyPair NewCryptoPair { get; set; } = new CryptocurrencyPair();
		public ObservableRangeCollection<CryptocurrencyPair> CryptoPairs { get; } = new ObservableRangeCollection<CryptocurrencyPair>();

		public ManagePairsViewModel(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		public async Task GetCryptoPairs()
		{
			ActivityIndicatorIsRunning = true;
			try
			{
				var pairs = await _databaseService.GetPairsAsync();
				CryptoPairs.ReplaceRange(pairs);
			}
			catch (Exception ex)
			{
				await Application.Current!.Windows[0].Page!.DisplayAlert("Niepowodzenie", $"Błąd podczas odczytu danych z bazy!: {ex.Message}", "OK");
			}
			ActivityIndicatorIsRunning = false;
		}

		[RelayCommand]
		public async Task DeletePair(CryptocurrencyPair pair)
		{
			ActivityIndicatorIsRunning = true;
			await _databaseService.DeletePairAsync(pair);
			ActivityIndicatorIsRunning = false;
			await GetCryptoPairs();
		}

		[RelayCommand]
		public async Task AddCryptoPair()
		{
			if (NewCryptoPair == null || string.IsNullOrEmpty(NewCryptoPair.CurrencyName1) || string.IsNullOrEmpty(NewCryptoPair.CurrencyName2))
			{
				await Application.Current!.Windows[0].Page!.DisplayAlert("Błąd", "Uzupełnij wszytkie dane na oknie!", "OK");
				return;
			}
			try
			{
				CryptoPairs.Clear();
				ActivityIndicatorIsRunning = true;
				int i = await _databaseService.AddPairAsync(NewCryptoPair);
				await GetCryptoPairs();
				ActivityIndicatorIsRunning = false;
				await Application.Current!.Windows[0].Page!.DisplayAlert("Sukces", $"Pomyślnie dodano parę {NewCryptoPair.CurrencyName1} / {NewCryptoPair.CurrencyName2}", "OK");

			}
			catch (Exception ex)
			{
				await Application.Current!.Windows[0].Page!.DisplayAlert("Niepowodzenie", $"Błąd: {ex.Message}", "OK");
			}
		}
	}
}
