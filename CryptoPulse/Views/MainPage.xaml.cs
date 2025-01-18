using CryptoPulse.ViewModels;

namespace CryptoPulse.Views
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

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			if (BindingContext is MainPageViewModel viewModel)
			{
				await viewModel.GetCryptoPairs();
				viewModel.StartUpdatingPrices();
			}
		}
	}

}
