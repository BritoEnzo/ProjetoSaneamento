using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Serialization;
using SaneaBrasil.Models;
using SaneaBrasil.Models.Indicadores;

namespace SaneaBrasil.Services;

/// <summary>
/// Cliente da API de Agregados do IBGE (SIDRA), versão 3.
/// Docs: https://servicodados.ibge.gov.br/api/docs/agregados?versao=3
///
/// Para cada indicador, o serviço:
///  1. lê os metadados da tabela (variável, classificação e categorias);
///  2. pede os valores do município só para as categorias de primeiro nível (evita contar duas vezes);
///  3. entrega cada categoria ao indicador, que decide se ela é adequada.
/// </summary>
public class IbgeSaneamentoService : ApiServiceBase, ISaneamentoService
{
    private const int TabelaPopulacao = 4714;   // Censo 2022 — população residente
    private const int VariavelPopulacao = 93;

    private readonly ConcurrentDictionary<int, MetadadosDto> _cacheMetadados = new();

    public IbgeSaneamentoService(HttpClient http) : base(http) { }

    protected override string UrlBase => "https://servicodados.ibge.gov.br/api/v3/agregados/";
    protected override string NomeApi => "API de Agregados do IBGE";

    public async Task<IndiceSaneamento> ObterIndiceAsync(Municipio municipio, CancellationToken ct = default)
    {
        // A lista é tratada pela classe base: polimorfismo.
        IndicadorSaneamento[] indicadores =
        [
            new AbastecimentoAgua(),
            new EsgotamentoSanitario(),
            new ColetaLixo()
        ];

        var tarefaPopulacao = ObterPopulacaoAsync(municipio, ct);
        await Task.WhenAll(indicadores.Select(i => PreencherIndicadorAsync(i, municipio, ct)));

        return new IndiceSaneamento(municipio, indicadores, await tarefaPopulacao);
    }

    private async Task PreencherIndicadorAsync(IndicadorSaneamento indicador, Municipio municipio, CancellationToken ct)
    {
        try
        {
            var meta = await ObterMetadadosAsync(indicador.TabelaSidra, ct);

            var variavel = meta.Variaveis.FirstOrDefault(v => !(v.Unidade ?? "").Contains('%'))
                           ?? meta.Variaveis.First();

            var classificacao = meta.Classificacoes.FirstOrDefault(c =>
                                    c.Nome.Contains(indicador.PalavraChaveClassificacao, StringComparison.OrdinalIgnoreCase))
                                ?? meta.Classificacoes.First();

            // Descarta o "Total" (nível 0) e fica só com as categorias de primeiro nível.
            var categorias = classificacao.Categorias
                .Where(c => !c.Nome.Equals("Total", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (categorias.Any(c => c.Nivel > 0))
                categorias = categorias.Where(c => c.Nivel > 0).ToList();
            if (categorias.Count == 0)
                throw new ApiException($"A tabela {indicador.TabelaSidra} não tem categorias.");
            var nivelTopo = categorias.Min(c => c.Nivel);
            var ids = string.Join(",", categorias.Where(c => c.Nivel == nivelTopo).Select(c => c.Id));

            var caminho = $"{indicador.TabelaSidra}/periodos/-1/variaveis/{variavel.Id}" +
                          $"?localidades=N6[{municipio.Id}]&classificacao={classificacao.Id}[{ids}]";

            var resposta = await GetAsync<List<ResultadoVariavelDto>>(caminho, ct);
            var resultados = resposta?.FirstOrDefault()?.Resultados ?? [];

            foreach (var resultado in resultados)
            {
                var classif = resultado.Classificacoes.FirstOrDefault(c => c.Id == classificacao.Id.ToString())
                              ?? resultado.Classificacoes.FirstOrDefault();
                var nomeCategoria = classif?.Categoria.Values.FirstOrDefault();
                var serie = resultado.Series.FirstOrDefault()?.Serie;
                if (nomeCategoria is null || serie is null || serie.Count == 0) continue;

                var (ano, valor) = serie.Last();
                indicador.DefinirAnoReferencia(ano);
                indicador.RegistrarCategoria(nomeCategoria, ConverterValor(valor));
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (ApiException ex) { indicador.RegistrarErro(ex.Message); }
        catch (Exception) { indicador.RegistrarErro("Não foi possível interpretar os dados do IBGE."); }
    }

    private async Task<long?> ObterPopulacaoAsync(Municipio municipio, CancellationToken ct)
    {
        try
        {
            var caminho = $"{TabelaPopulacao}/periodos/-1/variaveis/{VariavelPopulacao}?localidades=N6[{municipio.Id}]";
            var resposta = await GetAsync<List<ResultadoVariavelDto>>(caminho, ct);
            var valor = resposta?.FirstOrDefault()?.Resultados.FirstOrDefault()?.Series.FirstOrDefault()?.Serie.Values.LastOrDefault();
            return valor is null ? null : ConverterValor(valor);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { return null; } // População é informação extra: não derruba a tela.
    }

    private async Task<MetadadosDto> ObterMetadadosAsync(int tabela, CancellationToken ct)
    {
        if (_cacheMetadados.TryGetValue(tabela, out var emCache)) return emCache;

        var meta = await GetAsync<MetadadosDto>($"{tabela}/metadados", ct)
                   ?? throw new ApiException($"Metadados da tabela {tabela} indisponíveis.");
        _cacheMetadados[tabela] = meta;
        return meta;
    }

    /// <summary>O SIDRA usa "-" para zero e "...", "X" etc. para dado indisponível.</summary>
    private static long ConverterValor(string? valor) =>
        long.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;

    #region DTOs (formato do JSON do IBGE)

    private sealed class MetadadosDto
    {
        public List<VariavelDto> Variaveis { get; set; } = [];
        public List<ClassificacaoDto> Classificacoes { get; set; } = [];
    }

    private sealed class VariavelDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Unidade { get; set; }
    }

    private sealed class ClassificacaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<CategoriaDto> Categorias { get; set; } = [];
    }

    private sealed class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Nivel { get; set; }
    }

    private sealed class ResultadoVariavelDto
    {
        public string Id { get; set; } = string.Empty;
        public List<ResultadoDto> Resultados { get; set; } = [];
    }

    private sealed class ResultadoDto
    {
        public List<ClassificacaoResultadoDto> Classificacoes { get; set; } = [];
        public List<SerieDto> Series { get; set; } = [];
    }

    private sealed class ClassificacaoResultadoDto
    {
        public string Id { get; set; } = string.Empty;
        public Dictionary<string, string> Categoria { get; set; } = new();
    }

    private sealed class SerieDto
    {
        [JsonPropertyName("serie")]
        public Dictionary<string, string> Serie { get; set; } = new();
    }

    #endregion
}
