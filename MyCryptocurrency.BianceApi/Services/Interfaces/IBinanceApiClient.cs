using MyCryptocurrency.BianceApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCryptocurrency.BianceApi.Services.Interfaces;
public interface IBinanceApiClient
{
	public Task<List<AccountTradeListDto>> GetAccountTradeLis(string pair);
	public Task SetNewKeyApiValue(string apiKey);
	public Task SetNewPrivateKeyValue(string privateKey);
}
