using AutoMapper;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MyCryptocurrency.Mapping;
using MyCryptocurrency.BianceApi.Services.Interfaces;
using MyCryptocurrency.Database.Service;
using MyCryptocurrency.Database.Service.Interfaces;
using MyCryptocurrency.Infrastructure.Services;
using MyCryptocurrency.Infrastructure.Services.Interfaces;
using MyCryptocurrency.Services;
using MyCryptocurrency.Services.Interfaces;
using MyCryptocurrency.ViewModels;
using MyCryptocurrency.Views;
using Microsoft.Maui.Controls;

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

			//builder.Services.AddSingleton<IDispatcher>(provider => Application.Current.Dispatcher);
			//Mappers
			builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfiles());
			}).CreateMapper());

			//Dependency injections
			builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
			builder.Services.AddSingleton<IBinanceClientService, BinanceClientService>();
			builder.Services.AddSingleton<IBinanceApiClient, BinanceApiClient>();
			builder.Services.AddSingleton<IDatabaseClientService, DatabaseSQLLiteService>();
			builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

			// Register services and viewmodels
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<KeyInputViewModel>();
			builder.Services.AddTransient<TradeDetailsViewModel>();
			builder.Services.AddTransient<SettingsPage>();

			//Registyer Pages
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<KeyInputPage>();
			builder.Services.AddTransient<TradeDetailsViewModel>();
			builder.Services.AddTransient<SettingsPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
