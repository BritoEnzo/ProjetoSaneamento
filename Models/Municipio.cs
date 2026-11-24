namespace SaneaBrasil.Models;

/// <summary>Município brasileiro identificado pelo código IBGE de 7 dígitos.</summary>
public class Municipio : Localidade
{
    public string Uf { get; set; } = string.Empty;

    public string NomeCompleto => string.IsNullOrWhiteSpace(Uf) ? Nome : $"{Nome} - {Uf}";

    public override string Descricao => $"Código IBGE {Id}";
}
