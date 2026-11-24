namespace SaneaBrasil.Models.Indicadores;

/// <summary>Domicílios abastecidos pela rede geral de distribuição de água (Censo 2022, tabela 6804).</summary>
public class AbastecimentoAgua : IndicadorSaneamento
{
    public override string Nome => "Abastecimento de água";
    public override string Icone => "💧";
    public override int TabelaSidra => 6804;
    public override string PalavraChaveClassificacao => "abastecimento";
    public override string CriterioAdequacao => "Domicílios ligados à rede geral de distribuição de água.";
    protected override string[] TermosAdequados => ["rede geral"];
    public override double? MetaMarcoLegal => 99;
}
