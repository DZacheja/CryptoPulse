using AutoMapper;
using MyCryptocurrency.BianceApi.DTOs;
using MyCryptocurrency.Models;

namespace MyCryptocurrency.BianceApi.Mapping;
public class AccountTradeListProfile : Profile
{
	public AccountTradeListProfile()
	{
		CreateMap<AccountTradeListDto, AccountTradeList>()
			.ForMember(x => x.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.Time).DateTime));
	}
}
