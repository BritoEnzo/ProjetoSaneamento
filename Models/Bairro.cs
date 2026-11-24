namespace SaneaBrasil.Models;

/// <summary>Bairro da Zona Leste da cidade de São Paulo.</summary>
public class Bairro
{
    public string Nome { get; set; } = string.Empty;
    public string MunicipioNome => "São Paulo - SP";
    public string Descricao => "Zona Leste - São Paulo";
}
