using AutoMapper;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using CryptoPulse.BianceApi.Services.Interfaces;
using CryptoPulse.Database.Service;
using CryptoPulse.Database.Service.Interfaces;
using CryptoPulse.Infrastructure.Services;
using CryptoPulse.Infrastructure.Services.Interfaces;
using CryptoPulse.Mapping;
using CryptoPulse.Services;
using CryptoPulse.Services.Interfaces;
using CryptoPulse.ViewModels;
using CryptoPulse.Views;
using Syncfusion.Maui.Core.Hosting;

namespace CryptoPulse
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureSyncfusionCore()
				.UseMauiCommunityToolkit(
				options =>
				{
					options.SetShouldEnableSnackbarOnWindows(true);
				})
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
			builder.Services.AddTransient<IStartupService, StartupService>();

			// Register services and viewmodels
			builder.Services.AddTransient<TestPageViewModel>();
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<KeyInputViewModel>();
			builder.Services.AddTransient<TradeDetailsViewModel>();
			builder.Services.AddTransient<SettingsPage>();
			builder.Services.AddTransient<ManagePairsViewModel>();

			//Registyer Pages
			builder.Services.AddTransient<TestPage>();
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<KeyInputPage>();
			builder.Services.AddTransient<TradeDetailsViewModel>();
			builder.Services.AddTransient<SettingsPage>();
			builder.Services.AddTransient<ManagePairsPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
