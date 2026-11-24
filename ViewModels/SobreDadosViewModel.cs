using System.Windows.Input;
using SaneaBrasil.Models.Indicadores;
using SaneaBrasil.Services;

namespace SaneaBrasil.ViewModels;

/// <summary>Tela modal que explica como o índice é calculado.</summary>
public class SobreDadosViewModel : BaseViewModel
{
    public SobreDadosViewModel(INavegacaoService navegacao)
    {
        Titulo = "Como calculamos";
        FecharCommand = new Command(async () => await navegacao.VoltarAsync());
    }

    public ICommand FecharCommand { get; }

    // Instâncias usadas só para ler os textos de cada critério (polimorfismo).
    public IReadOnlyList<IndicadorSaneamento> Indicadores { get; } =
        new List<IndicadorSaneamento>
        {
            new AbastecimentoAgua(),
            new EsgotamentoSanitario(),
            new ColetaLixo()
        };
}
