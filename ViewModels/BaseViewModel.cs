using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SaneaBrasil.ViewModels;

/// <summary>
/// Base de todos os ViewModels (padrão MVVM): notificação de propriedades,
/// estado de carregamento e mensagem de erro.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _carregando;
    private string _titulo = string.Empty;
    private string? _mensagemErro;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool Carregando
    {
        get => _carregando;
        set
        {
            if (SetProperty(ref _carregando, value))
                OnPropertyChanged(nameof(Livre));
        }
    }

    public bool Livre => !Carregando;

    public string Titulo
    {
        get => _titulo;
        set => SetProperty(ref _titulo, value);
    }

    public string? MensagemErro
    {
        get => _mensagemErro;
        set
        {
            if (SetProperty(ref _mensagemErro, value))
                OnPropertyChanged(nameof(TemErro));
        }
    }

    public bool TemErro => !string.IsNullOrWhiteSpace(MensagemErro);

    /// <summary>Chamado pela página no OnAppearing. Cada ViewModel sobrescreve se precisar.</summary>
    public virtual Task AoAparecerAsync() => Task.CompletedTask;

    /// <summary>Executa uma ação assíncrona controlando "Carregando" e erros num lugar só.</summary>
    protected async Task ExecutarAsync(Func<Task> acao)
    {
        if (Carregando) return;
        try
        {
            MensagemErro = null;
            Carregando = true;
            await acao();
        }
        catch (Exception ex)
        {
            MensagemErro = ex.Message;
        }
        finally
        {
            Carregando = false;
        }
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nome = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        OnPropertyChanged(nome);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
