using MyCryptocurrency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.Services.Interfaces;
public interface IBinanceClientService
{
	public Task<List<AccountTradeList>> GetAccountTradeList(string pair);
}
