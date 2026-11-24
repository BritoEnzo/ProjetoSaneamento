using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class InicioPage : ContentPage
{
    private readonly InicioViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public InicioPage(InicioViewModel viewModel)
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
