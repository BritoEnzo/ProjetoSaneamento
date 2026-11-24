namespace SaneaBrasil.Models;

/// <summary>Unidade da Federação (UF).</summary>
public class Estado : Localidade
{
    public string Sigla { get; set; } = string.Empty;
    public Regiao? Regiao { get; set; }

    public override string Descricao => Regiao is null ? Sigla : $"{Sigla} · Região {Regiao.Nome}";
}
