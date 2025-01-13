using System.Globalization;

namespace MyCryptocurrency.Converters;
public class BoolToColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool isActive)
		{
			return isActive ? "Green" : "Red";
		}
		return "Unknown";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		// Optionally implement ConvertBack if needed.
		return null;
	}
}

