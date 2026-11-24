namespace SaneaBrasil.Models.Indicadores;

/// <summary>
/// Classe base abstrata de um indicador de saneamento calculado a partir de uma tabela
/// do Censo 2022 (SIDRA/IBGE). Cada subclasse diz qual tabela usar e quais categorias
/// contam como atendimento adequado (polimorfismo).
/// </summary>
public abstract class IndicadorSaneamento
{
    public abstract string Nome { get; }
    public abstract string Icone { get; }

    /// <summary>Número da tabela no SIDRA/IBGE.</summary>
    public abstract int TabelaSidra { get; }

    /// <summary>Trecho do nome da classificação da tabela que contém as categorias (ex.: "esgotamento").</summary>
    public abstract string PalavraChaveClassificacao { get; }

    /// <summary>Explicação, em texto, do que é considerado adequado.</summary>
    public abstract string CriterioAdequacao { get; }

    /// <summary>Termos que identificam uma categoria adequada.</summary>
    protected abstract string[] TermosAdequados { get; }

    /// <summary>Meta de cobertura do Marco Legal do Saneamento (Lei 14.026/2020) para 2033, quando existe.</summary>
    public virtual double? MetaMarcoLegal => null;

    // Encapsulamento: os valores só mudam pelos métodos abaixo.
    public long TotalDomicilios { get; private set; }
    public long DomiciliosAdequados { get; private set; }
    public string? AnoReferencia { get; private set; }
    public string? MensagemErro { get; private set; }

    public bool PossuiDados => TotalDomicilios > 0;

    public double Percentual => PossuiDados
        ? Math.Round(100.0 * DomiciliosAdequados / TotalDomicilios, 1)
        : 0;

    /// <summary>Valor entre 0 e 1, usado pela ProgressBar.</summary>
    public double Fracao => Percentual / 100.0;

    public NivelSaneamento Nivel => PossuiDados
        ? NivelSaneamentoExtensions.APartirDoPercentual(Percentual)
        : NivelSaneamento.SemDados;

    public string NivelDescricao => Nivel.Descricao();

    public string PercentualTexto => PossuiDados ? $"{Percentual:N1}%" : "—";

    public string Resumo => MensagemErro
        ?? (PossuiDados
            ? $"{DomiciliosAdequados:N0} de {TotalDomicilios:N0} domicílios atendidos"
            : "Sem dados para este município");

    public string MetaTexto => MetaMarcoLegal is double meta
        ? (Percentual >= meta
            ? $"✔ Já atinge a meta de {meta:N0}% do Marco Legal (2033)"
            : $"Faltam {meta - Percentual:N1} p.p. para a meta de {meta:N0}% do Marco Legal (2033)")
        : "Sem meta numérica no Marco Legal do Saneamento";

    /// <summary>Decide se uma categoria do IBGE conta como atendimento adequado. Pode ser sobrescrito.</summary>
    public virtual bool CategoriaEhAdequada(string nomeCategoria) =>
        TermosAdequados.Any(t => nomeCategoria.Contains(t, StringComparison.OrdinalIgnoreCase));

    public void RegistrarCategoria(string nomeCategoria, long domicilios)
    {
        if (domicilios <= 0) return;
        TotalDomicilios += domicilios;
        if (CategoriaEhAdequada(nomeCategoria))
            DomiciliosAdequados += domicilios;
    }

    public void DefinirAnoReferencia(string ano) => AnoReferencia = ano;

    public void RegistrarErro(string mensagem) => MensagemErro = mensagem;
}
