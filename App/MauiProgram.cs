using App.Pages;
using App.ViewModels;
using App.Services;
using Microsoft.Extensions.Logging;
using App.Interface;
using CommunityToolkit.Maui;
using SQLitePCL;
namespace App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

			});
        builder.Services.AddSingleton<BookViewModel>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<BookApi>();
        Batteries_V2.Init();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
