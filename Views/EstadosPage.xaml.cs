using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class EstadosPage : ContentPage
{
    private readonly EstadosViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public EstadosPage(EstadosViewModel viewModel)
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
