using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.BianceApi.DTOs;
public class PairPriceTickerDto
{
	public string Symbol { get; set; }
	public decimal Price { get; set; }
}
