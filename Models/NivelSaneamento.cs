namespace SaneaBrasil.Models;

public enum NivelSaneamento
{
    SemDados,
    Critico,
    Baixo,
    Regular,
    Bom,
    Excelente
}

public static class NivelSaneamentoExtensions
{
    public static NivelSaneamento APartirDoPercentual(double percentual) => percentual switch
    {
        < 50 => NivelSaneamento.Critico,
        < 70 => NivelSaneamento.Baixo,
        < 85 => NivelSaneamento.Regular,
        < 95 => NivelSaneamento.Bom,
        _ => NivelSaneamento.Excelente
    };

    public static string Descricao(this NivelSaneamento nivel) => nivel switch
    {
        NivelSaneamento.Critico => "Crítico",
        NivelSaneamento.Baixo => "Baixo",
        NivelSaneamento.Regular => "Regular",
        NivelSaneamento.Bom => "Bom",
        NivelSaneamento.Excelente => "Excelente",
        _ => "Sem dados"
    };
}
