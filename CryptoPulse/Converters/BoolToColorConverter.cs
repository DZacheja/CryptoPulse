using System.Globalization;

namespace CryptoPulse.Converters;
public class BoolToColorConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null && value is bool isActive)
		{
			return isActive ? "Green" : "Red";
		}
		return "Unknown";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

