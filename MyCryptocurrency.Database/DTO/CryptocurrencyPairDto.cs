using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Database.DTO;
public class CryptocurrencyPairDto
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string CurrencyName1 { get; set; }
	public string CurrencyName2 { get; set; }
	public int OrderID { get; set; }
}
