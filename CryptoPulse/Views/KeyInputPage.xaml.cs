using CryptoPulse.ViewModels;
using System.Diagnostics;

namespace CryptoPulse.Views;

public partial class KeyInputPage : ContentPage
{
	private KeyInputViewModel _viewModel { get; set; }
	public KeyInputPage(KeyInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.Initialize();
	}
}