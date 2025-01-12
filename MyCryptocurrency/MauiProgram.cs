using Microsoft.Extensions.Logging;
using MyCryptocurrency.BianceApi.Mapping;
using MyCryptocurrency.Services;
using MyCryptocurrency.ViewModels;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using MyCryptocurrency.Services.Interfaces;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using MyCryptocurrency.Infrastructure.Services;
using MyCryptocurrency.Views;
using CommunityToolkit.Maui;

namespace MyCryptocurrency
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});


			//Mappers
			builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new AccountTradeListProfile());
			}).CreateMapper());

			//Dependency injections
			builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
			builder.Services.AddSingleton<IBinanceClientService, BinanceClientService>();
			builder.Services.AddSingleton<IBinanceApiClient, BinanceApiClient>();

			// Register services and viewmodels
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<KeyInputViewModel>();

			//Registyer Pages
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<KeyInputPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
