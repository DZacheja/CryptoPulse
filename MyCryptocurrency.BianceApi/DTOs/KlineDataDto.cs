namespace MyCryptocurrency.BianceApi.DTOs;
public class KlineDataDto
{
	public long OpenTime { get; set; }
	public decimal OpenPrice { get; set; }
	public decimal HighPrice { get; set; }
	public decimal LowPrice { get; set; }
	public decimal ClosePrice { get; set; }
	public double Volume { get; set; }
	public long CloseTime { get; set; }
	public double QuoteAssetVolume { get; set; }
	public long NumberOfTrades { get; set; }
	public decimal TakerBuyBaseAssetVolume { get; set; }
	public decimal TakerBuyQuoteAssetVolume { get; set; }
}
