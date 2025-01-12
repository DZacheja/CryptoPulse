using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.BianceApi.DTOs;
public class AccountTradeListDto
{
	public string Symbol { get; set; }
	public long Id { get; set; }
	public long OrderId { get; set; }
	public long OrderListId { get; set; }
	public decimal Price { get; set; }
	public decimal Qty { get; set; }
	public decimal QuoteQty { get; set; }
	public decimal Commission { get; set; }
	public string CommissionAsset { get; set; }
	public long Time { get; set; }
	public bool IsBuyer { get; set; }
	public bool IsMaker { get; set; }
	public bool IsBestMatch { get; set; }
	public string FormattedTime
	{
		get
		{
			// Convert the Time (Unix timestamp in milliseconds) to DateTime
			var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(Time).DateTime;
			return dateTime.ToString("dd-MM-yy: HH:mm:ss");
		}
	}
}
