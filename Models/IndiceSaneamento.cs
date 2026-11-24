using SaneaBrasil.Models.Indicadores;

namespace SaneaBrasil.Models;

/// <summary>
/// Resultado consolidado de um município: os três indicadores e o índice geral
/// (média simples dos indicadores que têm dados). Exemplo de composição.
/// </summary>
public class IndiceSaneamento
{
    public IndiceSaneamento(Municipio municipio, IReadOnlyList<IndicadorSaneamento> indicadores, long? populacao)
    {
        Municipio = municipio;
        Indicadores = indicadores;
        Populacao = populacao;
    }

    public Municipio Municipio { get; }
    public IReadOnlyList<IndicadorSaneamento> Indicadores { get; }
    public long? Populacao { get; }

    public bool PossuiDados => Indicadores.Any(i => i.PossuiDados);

    public double IndiceGeral => PossuiDados
        ? Math.Round(Indicadores.Where(i => i.PossuiDados).Average(i => i.Percentual), 1)
        : 0;

    public NivelSaneamento Nivel => PossuiDados
        ? NivelSaneamentoExtensions.APartirDoPercentual(IndiceGeral)
        : NivelSaneamento.SemDados;

    public string? AnoReferencia => Indicadores.Select(i => i.AnoReferencia).FirstOrDefault(a => a is not null);
}
