using CryptoPulse.Views;
using static System.Net.Mime.MediaTypeNames;

namespace CryptoPulse
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			CheckKeysAndSetShellItems();
		}

		private void CheckKeysAndSetShellItems()
		{
				Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
				Routing.RegisterRoute(nameof(KeyInputPage), typeof(KeyInputPage));
				Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
				Routing.RegisterRoute(nameof(TradeListDetailsPage), typeof(TradeListDetailsPage));

				// Dodaj elementy AppShell dynamicznie
				var tabBar = new TabBar
				{
					Items =
					{
						new ShellContent
						{
							Title = "Home",
							ContentTemplate = new DataTemplate(typeof(MainPage)),
							Icon = "home_button"
						},
						new ShellContent
						{
							Title = "Manage Pairs",
							ContentTemplate = new DataTemplate(typeof(ManagePairsPage)),
							Icon = "manage_pairs"
						},
					}
				};
				if (AppSettings.ShowPageForTest)
				{
					tabBar.Items.Add(new ShellContent
					{
						Title = "Test Page",
						ContentTemplate = new DataTemplate(typeof(TestPage)),
						Icon = "testpage"
					});
				}
				Items.Clear();
				Items.Add(tabBar);
			//}
		}
	}
}
