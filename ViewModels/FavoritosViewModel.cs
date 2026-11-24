using System.Collections.ObjectModel;
using SaneaBrasil.Models;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Municípios salvos pelo usuário.</summary>
public class FavoritosViewModel : BaseViewModel
{
    private readonly IFavoritosService _favoritos;
    private readonly INavegacaoService _navegacao;
    private Municipio? _municipioSelecionado;

    public FavoritosViewModel(IFavoritosService favoritos, INavegacaoService navegacao)
    {
        _favoritos = favoritos;
        _navegacao = navegacao;
        Titulo = "Favoritos";
    }

    public ObservableCollection<Municipio> Municipios { get; } = [];

    public Municipio? MunicipioSelecionado
    {
        get => _municipioSelecionado;
        set
        {
            if (!SetProperty(ref _municipioSelecionado, value) || value is null) return;
            _ = AbrirDetalheAsync(value);
        }
    }

    public override Task AoAparecerAsync()
    {
        Municipios.Clear();
        foreach (var m in _favoritos.Listar())
            Municipios.Add(m);
        return Task.CompletedTask;
    }

    private async Task AbrirDetalheAsync(Municipio municipio)
    {
        await _navegacao.IrParaAsync("detalhe", new Dictionary<string, object> { ["Municipio"] = municipio });
        MunicipioSelecionado = null;
    }
}
