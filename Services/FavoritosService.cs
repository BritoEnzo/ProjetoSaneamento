using System.Text.Json;
using SaneaBrasil.Models;

namespace SaneaBrasil.Services;

/// <summary>Guarda os favoritos no armazenamento local do aparelho (Preferences), em JSON.</summary>
public class FavoritosService : IFavoritosService
{
    private const string Chave = "favoritos_municipios";
    private readonly List<Municipio> _favoritos;

    public FavoritosService()
    {
        _favoritos = Carregar();
    }

    public IReadOnlyList<Municipio> Listar() => _favoritos.OrderBy(m => m.Nome).ToList();

    public bool EhFavorito(Municipio municipio) => _favoritos.Contains(municipio);

    public void Adicionar(Municipio municipio)
    {
        if (EhFavorito(municipio)) return;
        _favoritos.Add(municipio);
        Salvar();
    }

    public void Remover(Municipio municipio)
    {
        if (_favoritos.Remove(municipio)) Salvar();
    }

    private static List<Municipio> Carregar()
    {
        try
        {
            var json = Preferences.Default.Get(Chave, string.Empty);
            return string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<Municipio>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private void Salvar() => Preferences.Default.Set(Chave, JsonSerializer.Serialize(_favoritos));
}
