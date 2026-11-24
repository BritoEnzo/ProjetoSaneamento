using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using SaneaBrasil.Models;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Municípios de um estado, com filtro por nome.</summary>
public class MunicipiosViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILocalidadesService _localidades;
    private readonly INavegacaoService _navegacao;
    private IReadOnlyList<Municipio> _todos = [];
    private string _filtro = string.Empty;
    private Municipio? _municipioSelecionado;

    public MunicipiosViewModel(ILocalidadesService localidades, INavegacaoService navegacao)
    {
        _localidades = localidades;
        _navegacao = navegacao;
    }

    public Estado? Estado { get; private set; }

    public ObservableCollection<Municipio> Municipios { get; } = [];

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (SetProperty(ref _filtro, value)) AplicarFiltro();
        }
    }

    public Municipio? MunicipioSelecionado
    {
        get => _municipioSelecionado;
        set
        {
            if (!SetProperty(ref _municipioSelecionado, value) || value is null) return;
            _ = AbrirDetalheAsync(value);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Estado", out var valor) && valor is Estado estado)
        {
            Estado = estado;
            Titulo = $"Municípios - {estado.Sigla}";
            _ = CarregarAsync();
        }
    }

    private Task CarregarAsync() => ExecutarAsync(async () =>
    {
        if (Estado is null) return;
        _todos = await _localidades.ObterMunicipiosAsync(Estado);
        AplicarFiltro();
    });

    private void AplicarFiltro()
    {
        var termo = SemAcento(Filtro.Trim());
        Municipios.Clear();
        foreach (var m in _todos.Where(m => termo.Length == 0 || SemAcento(m.Nome).Contains(termo, StringComparison.OrdinalIgnoreCase)))
            Municipios.Add(m);
    }

    private async Task AbrirDetalheAsync(Municipio municipio)
    {
        await _navegacao.IrParaAsync("detalhe", new Dictionary<string, object> { ["Municipio"] = municipio });
        MunicipioSelecionado = null;
    }

    /// <summary>Permite achar "Sao Paulo" digitando sem acento.</summary>
    private static string SemAcento(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
