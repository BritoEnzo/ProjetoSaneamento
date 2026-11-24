namespace SaneaBrasil.Models.Indicadores;

/// <summary>
/// Domicílios com esgotamento sanitário adequado (Censo 2022, tabela 6805).
/// Segue o critério do IBGE: rede geral/pluvial (ou fossa ligada à rede) e fossa séptica.
/// </summary>
public class EsgotamentoSanitario : IndicadorSaneamento
{
    public override string Nome => "Esgotamento sanitário";
    public override string Icone => "🚽";
    public override int TabelaSidra => 6805;
    public override string PalavraChaveClassificacao => "esgotamento";
    public override string CriterioAdequacao => "Domicílios ligados à rede geral ou pluvial, ou com fossa séptica / fossa filtro.";
    protected override string[] TermosAdequados => ["rede geral", "fossa séptica", "fossa septica"];
    public override double? MetaMarcoLegal => 90;

    // Polimorfismo: "fossa rudimentar" nunca é adequada, mesmo que algum termo apareça no nome.
    public override bool CategoriaEhAdequada(string nomeCategoria) =>
        !nomeCategoria.Contains("rudimentar", StringComparison.OrdinalIgnoreCase)
        && base.CategoriaEhAdequada(nomeCategoria);
}
