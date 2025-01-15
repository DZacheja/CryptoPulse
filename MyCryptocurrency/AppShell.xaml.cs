using MyCryptocurrency.Views;

namespace MyCryptocurrency
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			Routing.RegisterRoute(nameof(KeyInputPage), typeof(KeyInputPage));
			Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
			Routing.RegisterRoute(nameof(TradeListDetailsPage), typeof(TradeListDetailsPage));

			if (AppSettings.ShowPageForTest)
			{
				var testPage = new ShellContent
				{
					Title = "Test Page",
					Icon = "testpage",
					ContentTemplate = new DataTemplate(typeof(TestPage))
				};
				Items.Add(testPage);
			}
		}
	}
}
