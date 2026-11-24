using SaneaBrasil.Views;

namespace SaneaBrasil;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 3: rotas de navegação em pilha (push). "sobredados" abre como modal
        //    porque a página define Shell.PresentationMode="ModalAnimated".
        Routing.RegisterRoute("municipios", typeof(MunicipiosPage));
        Routing.RegisterRoute("detalhe", typeof(DetalhePage));
        Routing.RegisterRoute("sobredados", typeof(SobreDadosPage));
    }
}
