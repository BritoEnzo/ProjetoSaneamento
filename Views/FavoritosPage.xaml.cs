using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class FavoritosPage : ContentPage
{
    private readonly FavoritosViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public FavoritosPage(FavoritosViewModel viewModel)
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
