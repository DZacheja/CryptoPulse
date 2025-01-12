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

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			if (BindingContext is MainPageViewModel viewModel)
			{
				viewModel.StopUpdatingPrices();
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (BindingContext is MainPageViewModel viewModel)
			{
				viewModel.StartUpdatingPrices();
			}
		}
	}

}
