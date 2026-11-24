using System.Collections.ObjectModel;
using SaneaBrasil.Models;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Lista as 27 UFs (IBGE Localidades).</summary>
public class EstadosViewModel : BaseViewModel
{
    private readonly ILocalidadesService _localidades;
    private readonly INavegacaoService _navegacao;
    private Estado? _estadoSelecionado;

    public EstadosViewModel(ILocalidadesService localidades, INavegacaoService navegacao)
    {
        _localidades = localidades;
        _navegacao = navegacao;
        Titulo = "Estados";
    }

    public ObservableCollection<Estado> Estados { get; } = [];

    public Estado? EstadoSelecionado
    {
        get => _estadoSelecionado;
        set
        {
            if (!SetProperty(ref _estadoSelecionado, value) || value is null) return;
            _ = AbrirMunicipiosAsync(value);
        }
    }

    public override Task AoAparecerAsync() =>
        Estados.Count > 0 ? Task.CompletedTask : ExecutarAsync(async () =>
        {
            foreach (var estado in await _localidades.ObterEstadosAsync())
                Estados.Add(estado);
        });

    private async Task AbrirMunicipiosAsync(Estado estado)
    {
        await _navegacao.IrParaAsync("municipios", new Dictionary<string, object> { ["Estado"] = estado });
        EstadoSelecionado = null; // permite tocar no mesmo estado de novo
    }
}
