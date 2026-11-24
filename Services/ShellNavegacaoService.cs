namespace SaneaBrasil.Services;

/// <summary>Implementação de INavegacaoService usando o .NET MAUI Shell.</summary>
public class ShellNavegacaoService : INavegacaoService
{
    public Task IrParaAsync(string rota, IDictionary<string, object>? parametros = null) =>
        parametros is null
            ? Shell.Current.GoToAsync(rota)
            : Shell.Current.GoToAsync(rota, parametros);

    public Task VoltarAsync() => Shell.Current.GoToAsync("..");

    public Task MostrarAlertaAsync(string titulo, string mensagem) =>
        Shell.Current.DisplayAlertAsync(titulo, mensagem, "OK");
}
