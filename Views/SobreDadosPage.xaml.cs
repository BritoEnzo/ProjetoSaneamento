using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class SobreDadosPage : ContentPage
{
    private readonly SobreDadosViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public SobreDadosPage(SobreDadosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.AoAparecerAsync();
    }
}
