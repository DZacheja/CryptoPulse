using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.Models;
public class PairAvgPrice
{
	public int Mins { get; set; }
	public decimal Price { get; set; }
	public long CloseTime { get; set; }
}
