namespace SaneaBrasil.Models.Indicadores;

/// <summary>Domicílios com lixo coletado por serviço de limpeza (Censo 2022, tabela 6892).</summary>
public class ColetaLixo : IndicadorSaneamento
{
    public override string Nome => "Coleta de lixo";
    public override string Icone => "🗑️";
    public override int TabelaSidra => 6892;
    public override string PalavraChaveClassificacao => "lixo";
    public override string CriterioAdequacao => "Lixo coletado no domicílio ou depositado em caçamba de serviço de limpeza.";
    protected override string[] TermosAdequados => ["coletado", "caçamba", "cacamba", "serviço de limpeza"];
}
