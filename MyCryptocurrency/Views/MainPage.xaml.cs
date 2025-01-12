using MyCryptocurrency.ViewModels;

namespace MyCryptocurrency.Views
{
	public partial class MainPage : ContentPage
	{
		public MainPage(MainPageViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}

}
