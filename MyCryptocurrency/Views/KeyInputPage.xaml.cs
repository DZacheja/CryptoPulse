using MyCryptocurrency.ViewModels;

namespace MyCryptocurrency.Views;

public partial class KeyInputPage : ContentPage
{
	public KeyInputPage(KeyInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}