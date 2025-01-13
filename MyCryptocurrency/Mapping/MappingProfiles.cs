using AutoMapper;
using MyCryptocurrency.BianceApi.DTOs;
using MyCryptocurrency.Database.DTO;
using MyCryptocurrency.Models;

namespace MyCryptocurrency.Mapping;
public class MappingProfiles : Profile
{
	public MappingProfiles()
	{

		//AccountTrade
		CreateMap<AccountTradeListDto, AccountTrade>()
			.ForMember(x => x.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.Time).DateTime.ToLocalTime()));

		//CryptocurrencyPair
		CreateMap<CryptocurrencyPairDto, CryptocurrencyPair>()
			.ForMember(x => x.Symbol, opt => opt.MapFrom(src => src.CurrencyName1 + src.CurrencyName2));

		CreateMap<CryptocurrencyPair, CryptocurrencyPairDto>();

		//PairPriceTicker
		CreateMap<PairPriceTickerDto, PairPriceTicker>();

		//PairAvgPrice
		CreateMap<PairAvgPriceDTo, PairAvgPrice>();
	}
}
