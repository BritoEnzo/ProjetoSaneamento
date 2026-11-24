using SaneaBrasil.Models;

namespace SaneaBrasil.Services;

/// <summary>
/// Cliente da API de Localidades do IBGE.
/// Docs: https://servicodados.ibge.gov.br/api/docs/localidades
/// </summary>
public class IbgeLocalidadesService : ApiServiceBase, ILocalidadesService
{
    private IReadOnlyList<Estado>? _cacheEstados;
    private readonly Dictionary<string, IReadOnlyList<Municipio>> _cacheMunicipios = new();

    public IbgeLocalidadesService(HttpClient http) : base(http) { }

    protected override string UrlBase => "https://servicodados.ibge.gov.br/api/v1/localidades/";
    protected override string NomeApi => "API de Localidades do IBGE";

    public async Task<IReadOnlyList<Estado>> ObterEstadosAsync(CancellationToken ct = default)
    {
        if (_cacheEstados is not null) return _cacheEstados;

        var estados = await GetAsync<List<Estado>>("estados?orderBy=nome", ct) ?? [];
        _cacheEstados = estados;
        return estados;
    }

    public async Task<IReadOnlyList<Municipio>> ObterMunicipiosAsync(Estado estado, CancellationToken ct = default)
    {
        if (_cacheMunicipios.TryGetValue(estado.Sigla, out var emCache)) return emCache;

        var dtos = await GetAsync<List<MunicipioDto>>($"estados/{estado.Sigla}/municipios?orderBy=nome", ct) ?? [];
        var municipios = dtos
            .Select(d => new Municipio { Id = d.Id, Nome = d.Nome, Uf = estado.Sigla })
            .ToList();

        _cacheMunicipios[estado.Sigla] = municipios;
        return municipios;
    }

    // Só os campos de que precisamos; o resto do JSON é ignorado.
    private sealed class MunicipioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
