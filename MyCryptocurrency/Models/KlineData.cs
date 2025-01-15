namespace MyCryptocurrency.Models;
public class KlineData
{
	public DateTime OpenTime { get; set; }
	public DateTime CloseTime { get; set; }
	public DateTime AvgTime { get; set; }
	public long AvgTimeLng { get; set; }
	public decimal OpenPrice { get; set; }
	public decimal HighPrice { get; set; }
	public decimal LowPrice { get; set; }
	public decimal ClosePrice { get; set; }
	public decimal AvgPrice { get; set; }
}
