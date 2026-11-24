using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class DetalhePage : ContentPage
{
    private readonly DetalheViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public DetalhePage(DetalheViewModel viewModel)
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
