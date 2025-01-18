using CryptoPulse.ViewModels;

namespace CryptoPulse.Views;

public partial class TradeListDetailsPage : ContentPage
{
	public TradeListDetailsPage(TradeDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}