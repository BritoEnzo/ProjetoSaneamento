using System.Collections.ObjectModel;
using System.Windows.Input;
using SaneaBrasil.Models;
using SaneaBrasil.Models.Indicadores;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Mostra o índice de saneamento de um município (IBGE Agregados).</summary>
public class DetalheViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ISaneamentoService _saneamento;
    private readonly IFavoritosService _favoritos;
    private readonly INavegacaoService _navegacao;
    private Municipio? _municipio;
    private IndiceSaneamento? _indice;
    private string? _bairro;

    public DetalheViewModel(ISaneamentoService saneamento, IFavoritosService favoritos, INavegacaoService navegacao)
    {
        _saneamento = saneamento;
        _favoritos = favoritos;
        _navegacao = navegacao;
        Titulo = "Saneamento";

        AlternarFavoritoCommand = new Command(AlternarFavorito);
        RecarregarCommand = new Command(async () => await CarregarAsync());
        // Navegação modal: a rota "sobredados" abre com PresentationMode = ModalAnimated.
        ComoCalculamosCommand = new Command(async () => await _navegacao.IrParaAsync("sobredados"));
    }

    public ObservableCollection<IndicadorSaneamento> Indicadores { get; } = [];

    public ICommand AlternarFavoritoCommand { get; }
    public ICommand RecarregarCommand { get; }
    public ICommand ComoCalculamosCommand { get; }

    public string NomeMunicipio => string.IsNullOrWhiteSpace(_bairro)
        ? _municipio?.NomeCompleto ?? ""
        : $"{_bairro} - {_municipio?.NomeCompleto}";

    public string ContextoBairro => string.IsNullOrWhiteSpace(_bairro)
        ? ""
        : "Dados municipais de São Paulo: bairros da Zona Leste usam o índice do município.";

    public string PopulacaoTexto => _indice?.Populacao is long p ? $"População (Censo 2022): {p:N0} habitantes" : "";

    public bool TemIndice => _indice?.PossuiDados == true;

    public string IndiceTexto => TemIndice ? $"{_indice!.IndiceGeral:N1}" : "—";

    public NivelSaneamento NivelGeral => _indice?.Nivel ?? NivelSaneamento.SemDados;

    public string NivelTexto => NivelGeral.Descricao();

    public string FonteTexto => $"Fonte: IBGE, Censo Demográfico {_indice?.AnoReferencia ?? "2022"} " +
                                "(SIDRA, tabelas 6804, 6805 e 6892).";

    public bool EhFavorito => _municipio is not null && _favoritos.EhFavorito(_municipio);

    public string TextoFavorito => EhFavorito ? "★ Remover dos favoritos" : "☆ Adicionar aos favoritos";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Municipio", out var valor) && valor is Municipio municipio)
        {
            _municipio = municipio;
            _bairro = query.TryGetValue("Bairro", out var bairroValor) ? bairroValor as string : null;
            Titulo = _bairro ?? municipio.Nome;
            OnPropertyChanged(nameof(NomeMunicipio));
            OnPropertyChanged(nameof(ContextoBairro));
            AtualizarFavorito();
            _ = CarregarAsync();
        }
    }

    private Task CarregarAsync() => ExecutarAsync(async () =>
    {
        if (_municipio is null) return;

        Indicadores.Clear();
        _indice = await _saneamento.ObterIndiceAsync(_municipio);

        foreach (var indicador in _indice.Indicadores)
            Indicadores.Add(indicador);

        if (!_indice.PossuiDados)
            MensagemErro = _indice.Indicadores.Select(i => i.MensagemErro).FirstOrDefault(m => m is not null)
                           ?? "O IBGE não retornou dados de saneamento para este município.";

        OnPropertyChanged(nameof(PopulacaoTexto));
        OnPropertyChanged(nameof(TemIndice));
        OnPropertyChanged(nameof(IndiceTexto));
        OnPropertyChanged(nameof(NivelGeral));
        OnPropertyChanged(nameof(NivelTexto));
        OnPropertyChanged(nameof(FonteTexto));
    });

    private void AlternarFavorito()
    {
        if (_municipio is null) return;
        if (EhFavorito) _favoritos.Remover(_municipio);
        else _favoritos.Adicionar(_municipio);
        AtualizarFavorito();
    }

    private void AtualizarFavorito()
    {
        OnPropertyChanged(nameof(EhFavorito));
        OnPropertyChanged(nameof(TextoFavorito));
    }
}
