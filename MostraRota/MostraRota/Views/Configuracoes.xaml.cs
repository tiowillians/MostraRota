using MostraRota.BDLocal;
using MostraRota.JSON;
using MostraRota.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Configuracoes : ContentPage
    {
        ConfiguracoesViewModel viewModel;

        public Configuracoes()
        {
            InitializeComponent();
            viewModel = new ConfiguracoesViewModel();
            this.BindingContext = viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // atualiza dados na base de dados
            UsuariosBD.AtualizaConfiguracoes(App.usrCorrente.Email, viewModel.URL,
                                             (grava_switch.IsToggled ? 1 : 0), (wifi_switch.IsToggled ? 1 : 0));

            // atualiza dados do usuário corrente
            App.usrCorrente.WSUrl = viewModel.URL;
            App.usrCorrente.Grava = (grava_switch.IsToggled ? 1 : 0);
            App.usrCorrente.WiFi = (wifi_switch.IsToggled ? 1 : 0);

            // envia dados do usuário corrente para o servidor
            // caso tenha ocorrido erro no login
            if (App.usrNaoSincronizado)
            {
                WSUsuariosJson.UpdateData();
                App.usrNaoSincronizado = false;
            }
        }
    }
}