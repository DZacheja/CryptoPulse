using AutoMapper;
using CryptoPulse.BianceApi.DTOs;
using CryptoPulse.Database.DTO;
using CryptoPulse.Models;

namespace CryptoPulse.Mapping;
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

		CreateMap<CryptocurrencyPair, CryptocurrencyPairDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.CurrencyName1, opt => opt.MapFrom(src => src.CurrencyName1))
			.ForMember(dest => dest.CurrencyName2, opt => opt.MapFrom(src => src.CurrencyName2))
			.ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID));

		//PairPriceTicker
		CreateMap<PairPriceTickerDto, PairPriceTicker>();

		//PairAvgPrice
		CreateMap<PairAvgPriceDTo, PairAvgPrice>();

		//KlineData
		CreateMap<KlineDataDto, KlineData>()
			.ForMember(x => x.OpenTime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.OpenTime).DateTime.ToLocalTime()))
			.ForMember(x => x.AvgTime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds((src.OpenTime + src.CloseTime) / 2).DateTime.ToLocalTime()))
			.ForMember(x => x.AvgTimeLng, opt => opt.MapFrom(src => (src.OpenTime + src.CloseTime) / 2))
			.ForMember(x => x.AvgPrice, opt => opt.MapFrom(src => ((src.HighPrice + src.LowPrice) / 2)));
	}
}
