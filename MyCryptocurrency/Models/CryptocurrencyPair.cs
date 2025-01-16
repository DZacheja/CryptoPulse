using CommunityToolkit.Mvvm.ComponentModel;

namespace MyCryptocurrency.Models;

/// <summary>
/// Represents a pair of cryptocurrencies with properties to display on the user interface.
/// </summary>
public partial class CryptocurrencyPair : ObservableObject
{
	#region Properties

	public string Id { get; set; } = string.Empty;

	public string CurrencyName1 { get; set; } = string.Empty;

	public string CurrencyName2 { get; set; } = string.Empty;

	public int OrderID { get; set; }

	public string Symbol { get; set; } = string.Empty;

	public AccountTrade LastOperation { get; set; } = new AccountTrade();

	public bool FoundLastOperation { get; set; }

	public string LastOperationPair { get; set; } = string.Empty;

	private decimal _currentExchangeRate;

	public decimal LastPrice { get; set; }

	[ObservableProperty]
	public partial string PriceColor { get; set; } = "Black";

	[ObservableProperty]
	public partial decimal DifferenceFromCurrent { get; set; }

	[ObservableProperty]
	public partial decimal PercentageFromCurrent { get; set; }

	[ObservableProperty]
	public partial bool PercentageIsPositiveNumber { get; set; }

	#endregion

	#region Logic

	public decimal CurrentExchangeRate
	{
		get => _currentExchangeRate;
		set
		{
			if (_currentExchangeRate != value)
			{
				_currentExchangeRate = value;
				OnPropertyChanged(nameof(CurrentExchangeRate));
				UpdatePriceColor();
				UpdateDifference();
			}
		}
	}

	/// <summary>
	/// Updates the price colour based on the difference between the last price and the current price.
	/// </summary>
	private void UpdatePriceColor()
	{
		PriceColor = LastPrice < CurrentExchangeRate ? "Green" :
					 LastPrice > CurrentExchangeRate ? "Red" : "Black";
	}

	/// <summary>
	/// Calculates the difference between the current rate and the price from the last operation.
	/// </summary>
	private void UpdateDifference()
	{
		if (LastOperation != null)
		{
			DifferenceFromCurrent = CurrentExchangeRate - LastOperation.Price;
			PercentageFromCurrent = 100 - ((LastOperation.Price / CurrentExchangeRate) * 100);
			if (LastOperation.IsBuyer)
			{
				PercentageIsPositiveNumber = PercentageFromCurrent > 0;
			}
			else
			{
				PercentageIsPositiveNumber = PercentageFromCurrent < 0;
				PercentageFromCurrent = PercentageFromCurrent * (-1);
			}
		}
	}

	#endregion
}
