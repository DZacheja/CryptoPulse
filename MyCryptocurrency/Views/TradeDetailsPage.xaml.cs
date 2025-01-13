using MyCryptocurrency.ViewModels;

namespace MyCryptocurrency.Views;

public partial class TradeListDetailsPage : ContentPage
{
	public TradeListDetailsPage(TradeDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}