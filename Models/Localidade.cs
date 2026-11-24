namespace SaneaBrasil.Models;

/// <summary>
/// Classe base abstrata para qualquer localidade do IBGE (estado, município).
/// Concentra o que é comum: código IBGE e nome.
/// </summary>
public abstract class Localidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Texto curto exibido abaixo do nome nas listas. Cada subclasse define o seu.</summary>
    public abstract string Descricao { get; }

    public override bool Equals(object? obj) => obj is Localidade outra && outra.GetType() == GetType() && outra.Id == Id;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public override string ToString() => Nome;
}
