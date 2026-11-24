namespace SaneaBrasil.Models;

/// <summary>Grande região do Brasil (Norte, Nordeste, Sudeste, Sul, Centro-Oeste).</summary>
public class Regiao
{
    public int Id { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
