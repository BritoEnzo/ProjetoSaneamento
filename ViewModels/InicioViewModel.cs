using System.Collections.ObjectModel;
using System.Windows.Input;
using SaneaBrasil.Models;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Tela inicial: busca bairros da Zona Leste de São Paulo.</summary>
public class InicioViewModel : BaseViewModel
{
    private readonly IBairroService _bairroService;
    private readonly INavegacaoService _navegacao;
    private string _filtro = string.Empty;
    private Bairro? _bairroSelecionado;

    public InicioViewModel(IBairroService bairroService, INavegacaoService navegacao)
    {
        _bairroService = bairroService;
        _navegacao = navegacao;
        Titulo = "SaneaBrasil";
        ExplorarEstadosCommand = new Command(async () => await _navegacao.IrParaAsync("//estados"));
        AplicarFiltro();
    }

    public ObservableCollection<Bairro> Bairros { get; } = [];

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (SetProperty(ref _filtro, value)) AplicarFiltro();
        }
    }

    public Bairro? BairroSelecionado
    {
        get => _bairroSelecionado;
        set
        {
            if (!SetProperty(ref _bairroSelecionado, value) || value is null) return;
            _ = AbrirDetalheAsync(value);
        }
    }

    public ICommand ExplorarEstadosCommand { get; }

    private void AplicarFiltro()
    {
        Bairros.Clear();
        foreach (var bairro in _bairroService.Buscar(Filtro))
            Bairros.Add(bairro);

        MensagemErro = Bairros.Count == 0 ? "Nenhum bairro encontrado na Zona Leste." : string.Empty;
    }

    private async Task AbrirDetalheAsync(Bairro bairro)
    {
        var municipio = new Municipio { Id = 3550308, Nome = "São Paulo", Uf = "SP" };
        await _navegacao.IrParaAsync("detalhe", new Dictionary<string, object>
        {
            ["Municipio"] = municipio,
            ["Bairro"] = bairro.Nome
        });
        BairroSelecionado = null;
    }
}
