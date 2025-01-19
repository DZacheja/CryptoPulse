namespace CryptoPulse.BianceApi.DTOs;

public class BalanceDto
{
	public string Asset { get; set; } = string.Empty;
	public string Free { get; set; } = string.Empty;
	public string Locked { get; set; } = string.Empty;
}