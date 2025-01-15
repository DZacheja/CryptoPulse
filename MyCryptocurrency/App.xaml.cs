using MyCryptocurrency.Infrastructure.Services.Interfaces;

namespace MyCryptocurrency
{
	public partial class App : Application
	{
		public App()
		{
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf0x0RHxbf1x1ZFZMYVlbQXJPIiBoS35Rc0ViWHtfdXVQQ2RbUEx0");
			InitializeComponent();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			return new Window(new AppShell());
		}
	}
}