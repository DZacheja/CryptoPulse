using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;

namespace MyCryptocurrency.Helpers
{
	public static class SnackbarHelper
	{
		public static async Task ShowSnackbarAsync(string message, string actionButtonText = "Dismiss", Action? action = null, TimeSpan? duration = null)
		{
			var snackbarOptions = new SnackbarOptions
			{
				BackgroundColor = Colors.Red,
				TextColor = Colors.Green,
				ActionButtonTextColor = Colors.Yellow,
				CornerRadius = new CornerRadius(10),
				Font = Microsoft.Maui.Font.SystemFontOfSize(14),
				ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
				CharacterSpacing = 0.5
			};

			var snackbarDuration = duration ?? TimeSpan.FromSeconds(3);
			var snackbarAction = action ?? (() => { /* Default action */ });

			var snackbar = Snackbar.Make(message, snackbarAction, actionButtonText, snackbarDuration, snackbarOptions);

			using var cancellationTokenSource = new CancellationTokenSource();
			await snackbar.Show(cancellationTokenSource.Token);
		}
	}
}
