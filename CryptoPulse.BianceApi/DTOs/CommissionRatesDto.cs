using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.BianceApi.DTOs;
public class CommissionRatesDto
{
	public string Maker { get; set; } = string.Empty;
	public string Taker { get; set; } = string.Empty;
	public string Buyer { get; set; } = string.Empty;
	public string Seller { get; set; } = string.Empty;
}
