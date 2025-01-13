using CommunityToolkit.Mvvm.ComponentModel;

namespace MyCryptocurrency.Models;
public partial class CryptocurrencyPair : ObservableObject
{
	public string Id { get; set; }
	public string CurrencyName1 { get; set; }
	public string CurrencyName2 { get; set; }
	public int OrderID { get; set; }
	public string Symbol { get; set; }

	public AccountTrade LastOperation {  get; set; } = new AccountTrade();
	public bool FoundLastOperation { get; set; }
	public string LastOperationPair { get; set; }

	private decimal _currentExchangeRate;

	public decimal LastPrice;

	[ObservableProperty] private string _priceColor;
	[ObservableProperty] private decimal _differenceFromCurrent;
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
	private void UpdatePriceColor()
	{
		if (LastPrice < CurrentExchangeRate)
			PriceColor = "Green";
		else if (LastPrice > CurrentExchangeRate)
			PriceColor = "Red";
		else
			PriceColor = "Black"; // Neutral color
	}

	private void UpdateDifference()
	{
		if(LastOperation != null)
			DifferenceFromCurrent = _currentExchangeRate - LastOperation.Price;
	}
}
