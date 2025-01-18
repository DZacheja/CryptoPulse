using CryptoPulse.Infrastructure.Services.Interfaces;
using CryptoPulse.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPulse
{
	public partial class App : Application
	{
		private readonly IServiceProvider _serviceProvider;
		public App(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			InitializeComponent();
		}

		protected override async void OnStart()
		{
			base.OnStart();
			var startupService = _serviceProvider.GetRequiredService<IStartupService>();
			await startupService.InitializeAsync();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
				var keyInputPage = _serviceProvider.GetRequiredService<KeyInputPage>();
				return new Window(keyInputPage);
		}
	}
}