using System.Globalization;

namespace MyCryptocurrency.Converters;
public class BoolToCornerRadious : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null && value is bool isActive)
		{
			return isActive ? "20,20,0,20" : "20,20,20,0";
		}
		return "Unknown";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

