using Microsoft.Extensions.Logging;
using SaneaBrasil.Services;
using SaneaBrasil.ViewModels;
using SaneaBrasil.Views;

namespace SaneaBrasil;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // HttpClient único, compartilhado pelos três clientes de API.
        builder.Services.AddSingleton(_ =>
        {
            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("SaneaBrasil/1.0");
            return http;
        });

        // Serviços (programação para interfaces + injeção de dependência)
        builder.Services.AddSingleton<ILocalidadesService, IbgeLocalidadesService>();
        builder.Services.AddSingleton<ISaneamentoService, IbgeSaneamentoService>();
        builder.Services.AddSingleton<ICepService, ViaCepService>();
        builder.Services.AddSingleton<IBairroService, BairroService>();
        builder.Services.AddSingleton<IFavoritosService, FavoritosService>();
        builder.Services.AddSingleton<INavegacaoService, ShellNavegacaoService>();

        // ViewModels
        builder.Services.AddTransient<InicioViewModel>();
        builder.Services.AddTransient<EstadosViewModel>();
        builder.Services.AddTransient<MunicipiosViewModel>();
        builder.Services.AddTransient<DetalheViewModel>();
        builder.Services.AddTransient<FavoritosViewModel>();
        builder.Services.AddTransient<SobreDadosViewModel>();

        // Páginas (o Shell cria as páginas pelo contêiner de DI)
        builder.Services.AddTransient<InicioPage>();
        builder.Services.AddTransient<EstadosPage>();
        builder.Services.AddTransient<MunicipiosPage>();
        builder.Services.AddTransient<DetalhePage>();
        builder.Services.AddTransient<FavoritosPage>();
        builder.Services.AddTransient<SobrePage>();
        builder.Services.AddTransient<SobreDadosPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
