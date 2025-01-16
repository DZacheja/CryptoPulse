using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Converters;
public class RemoveTrailingZerosConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value != null && parameter != null)
		{
			if (value is double doubleValue)
			{
				return doubleValue.ToString("G", CultureInfo.InvariantCulture);
			}

			if (value is decimal decimalValue)
			{
				var test = decimalValue.ToString("G", CultureInfo.InvariantCulture);
				return decimalValue.ToString("G", CultureInfo.InvariantCulture);
			}

			return value; // Fallback if the value is not a number.
		}

		return new object();
		
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
