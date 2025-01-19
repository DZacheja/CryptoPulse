using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPulse.BianceApi.DTOs;
public class AccountInfoDto
{
	public int MakerCommission { get; set; }
	public int TakerCommission { get; set; }
	public int BuyerCommission { get; set; }
	public int SellerCommission { get; set; }
	public CommissionRatesDto CommissionRates { get; set; }
	public bool CanTrade { get; set; }
	public bool CanWithdraw { get; set; }
	public bool CanDeposit { get; set; }
	public bool Brokered { get; set; }
	public bool RequireSelfTradePrevention { get; set; }
	public bool PreventSor { get; set; }
	public long UpdateTime { get; set; }
	public string AccountType { get; set; } = string.Empty;
	public List<BalanceDto> Balances { get; set; }
	public List<string> Permissions { get; set; } = new List<string>();
	public long Uid { get; set; }
}
