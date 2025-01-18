using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Models;
public class AccountTrade
{
	public string? Symbol { get; set; }
	public decimal Price { get; set; }
	public decimal Qty { get; set; }
	public decimal QuoteQty { get; set; }
	public decimal Commission { get; set; }
	public string? CommissionAsset { get; set; }
	public DateTime Time { get; set; }
	public bool IsBuyer { get; set; }
}
