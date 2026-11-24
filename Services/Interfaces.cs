using SaneaBrasil.Models;

namespace SaneaBrasil.Services;

/// <summary>API 1 — IBGE Localidades: estados e municípios.</summary>
public interface ILocalidadesService
{
    Task<IReadOnlyList<Estado>> ObterEstadosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Municipio>> ObterMunicipiosAsync(Estado estado, CancellationToken ct = default);
}

/// <summary>API 2 — IBGE Agregados (SIDRA): indicadores de saneamento do Censo 2022.</summary>
public interface ISaneamentoService
{
    Task<IndiceSaneamento> ObterIndiceAsync(Municipio municipio, CancellationToken ct = default);
}

/// <summary>API 3 — ViaCEP: converte um CEP no município correspondente.</summary>
public interface ICepService
{
    Task<Municipio?> BuscarMunicipioPorCepAsync(string cep, CancellationToken ct = default);
}

/// <summary>Busca bairros da Zona Leste de São Paulo.</summary>
public interface IBairroService
{
    IReadOnlyList<Bairro> Buscar(string nome);
}

/// <summary>Persistência local dos municípios favoritos.</summary>
public interface IFavoritosService
{
    IReadOnlyList<Municipio> Listar();
    bool EhFavorito(Municipio municipio);
    void Adicionar(Municipio municipio);
    void Remover(Municipio municipio);
}

/// <summary>Abstração da navegação, para os ViewModels não dependerem do Shell diretamente.</summary>
public interface INavegacaoService
{
    Task IrParaAsync(string rota, IDictionary<string, object>? parametros = null);
    Task VoltarAsync();
    Task MostrarAlertaAsync(string titulo, string mensagem);
}
