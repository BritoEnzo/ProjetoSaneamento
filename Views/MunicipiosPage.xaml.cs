using SaneaBrasil.ViewModels;

namespace SaneaBrasil.Views;

public partial class MunicipiosPage : ContentPage
{
    private readonly MunicipiosViewModel _viewModel;

    // O ViewModel chega por injeção de dependência (registrado no MauiProgram).
    public MunicipiosPage(MunicipiosViewModel viewModel)
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
