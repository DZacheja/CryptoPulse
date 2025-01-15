using MyCryptocurrency.ViewModels;

namespace MyCryptocurrency.Views;

public partial class TestPage : ContentPage
{
	public TestPage(TestPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}