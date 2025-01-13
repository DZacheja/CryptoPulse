using MyCryptocurrency.ViewModels;

namespace MyCryptocurrency.Views;

public partial class ManagePairsPage : ContentPage
{
	public ManagePairsPage(ManagePairsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}