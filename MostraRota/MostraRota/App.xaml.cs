using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.Views;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MostraRota
{
    public partial class App : Application
    {
        // conexão com banco de dados local
        public static ConexaoBD BDLocal { get; set; }

        // dados do usuário que fez o login
        public static UsuariosBD usrCorrente = null;
        public static bool usrNaoSincronizado = false;

        public App()
        {
            // conecta ao banco de dados local (e cria BD, se necessário)
            BDLocal = new ConexaoBD();

            InitializeComponent();

            // verificar se existe conexão com Internet
            if(ExisteConexaoInternet())
            {
                // mostrar página de login
                if (Device.RuntimePlatform == Device.iOS)
                    MainPage = new Login();
                else
                    MainPage = new NavigationPage(new Login());
            }
            else
            {
                // fazer login por algum dos usuários cadastrados
                usrCorrente = UsuariosBD.GetUsuario(null);
                if (usrCorrente == null)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Sem conexão com Internet.");
                    DependencyService.Get<IDeviceSpecific>().CloseApplication();
                }

                // mostra página inicial
                MostrarPaginaInicial();
            }
        }

        // verifica se existe conexão com a Internet
        public static bool ExisteConexaoInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }

        // verifica se Wi-Fi está disponível
        public static bool ExisteWiFi()
        {
            foreach(ConnectionType conn in CrossConnectivity.Current.ConnectionTypes)
            {
                if (conn == ConnectionType.WiFi)
                    return true;
            }

            return false;
        }

        public static async Task MostrarPaginaInicial()
        {
            var telaInicial = Application.Current.MainPage as Views.MasterDetailPrincipal;
            if (telaInicial == null)
            {
                telaInicial = new MasterDetailPrincipal();
                Application.Current.MainPage = telaInicial;
            }
            else
                await telaInicial.IrParaPaginaInicial();
        }

        public static async Task NavegarParaPagina(Page pagina)
        {
            var telaInicial = Application.Current.MainPage as Views.MasterDetailPrincipal;
            await telaInicial.PushAsync(pagina);
        }

        public static void VoltarParaLogin()
        {
            // mostrar página de login
            if (Device.RuntimePlatform == Device.iOS)
                Application.Current.MainPage = new Login();
            else
                Application.Current.MainPage = new NavigationPage(new Login());
        }
    }
}