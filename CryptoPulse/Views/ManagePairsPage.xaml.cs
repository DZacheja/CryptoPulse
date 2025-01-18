using CryptoPulse.ViewModels;

namespace CryptoPulse.Views;

public partial class ManagePairsPage : ContentPage
{
	private ManagePairsViewModel _viewModel;
	public ManagePairsPage(ManagePairsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		_viewModel = vm;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.GetCryptoPairs();
	}
}