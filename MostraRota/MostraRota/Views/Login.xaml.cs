using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.JSON;
using MostraRota.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private Account account;
        private AccountStore store;
        private int loginFrom;
        private OAuth2Authenticator authenticator;

        private LoginViewModel viewModel;

        public Login()
        {
            InitializeComponent();
            viewModel = new LoginViewModel();
            this.BindingContext = viewModel;

            // objetos do tipo Account para um serviço podem ser recuperados chamando o método
            // FindAccountsForService. O método retorna uma coleção IEnumerable de objectos Account,
            // com o primeiro item sendo o Account procurado.
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
        }

        public void OnLoginFacebook_Clicked(object sender, EventArgs e)
        {
            loginFrom = Constants.FROM_FACEBOOK;

            string myClientId = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    myClientId = Constants.iOSFacebookClientId;
                    break;

                case Device.Android:
                    myClientId = Constants.AndroidFacebookClientId;
                    break;
            }

            authenticator = new OAuth2Authenticator(
                                        clientId: myClientId,
                                        scope: Constants.FacebookScope,
                                        authorizeUrl: new Uri(Constants.FacebookAuthorizeUrl),
                                        redirectUrl: new Uri(Constants.FacebookRedirectUrl)
                                );

            DoAuthetication();
        }

        public void OnLoginGoogle_Clicked(object sender, EventArgs e)
        {
            loginFrom = Constants.FROM_GOOGLE;

            string myClientId = null;
            string myRedirectUri = null;
            string myClientSecret = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    myClientId = Constants.iOSGoogleClientId;
                    myRedirectUri = Constants.iOSGoogleRedirectUrl;
                    break;

                case Device.Android:
                    myClientId = Constants.AndroidGoogleClientId;
                    myRedirectUri = Constants.AndroidGoogleRedirectUrl;
                    myClientSecret = Constants.AndroidGoogleClientSecret;
                    break;
            }

            authenticator = new OAuth2Authenticator(
                                        clientId: myClientId,
                                        clientSecret: myClientSecret,
                                        scope: Constants.GoogleScope,
                                        authorizeUrl: new Uri(Constants.GoogleAuthorizeUrl),
                                        redirectUrl: new Uri(myRedirectUri),
                                        accessTokenUrl: new Uri(Constants.GoogleAccessTokenUrl),
                                        getUsernameAsync: null,
                                        isUsingNativeUI: true
                                );

            DoAuthetication();
        }

        public void DoAuthetication()
        {
            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        public async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (sender is OAuth2Authenticator authenticator)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            if (e.IsAuthenticated)
            {
                User user = null;
                switch (loginFrom)
                {
                    case Constants.FROM_GOOGLE:
                        user = await GetGoogleUserInfo(e.Account);
                        break;

                    case Constants.FROM_FACEBOOK:
                        user = await GetFacebookUserInfo(e.Account);
                        break;
                }

                if (account != null)
                    store.Delete(account, Constants.AppName);

                await store.SaveAsync(account = e.Account, Constants.AppName);

                // salva dados do usuário na base de dados local
                App.usrCorrente = await UsuariosBD.InsereAtualizaUsuario(user, loginFrom);

                if (string.IsNullOrEmpty(viewModel.URL) == false)
                    App.usrCorrente.WSUrl = viewModel.URL;

                // atualiza dados do usuário no servidor via Web Service
                bool ret = await WSUsuariosJson.UpdateData();
                App.usrNaoSincronizado = !ret;

                // mostra página inicial
                await App.MostrarPaginaInicial();
            }
        }

        // Obtem dados do usuário
        private async Task<User> GetGoogleUserInfo(Account account)
        {
            var request = new OAuth2Request("GET", new Uri(Constants.GoogleUserInfoUrl), null, account);
            var response = await request.GetResponseAsync();
            if (response == null)
                return null;

            // "Desserializa" os dados e armazena-os no account store.
            // O email do usuário será usado para identificá-lo no banco de dados
            string userJson = await response.GetResponseTextAsync();
            User user = JsonConvert.DeserializeObject<User>(userJson);

            // email não pode ser vazio
            if (string.IsNullOrEmpty(user.Email))
                user.Email = user.Id + "@Google";

            return user;
        }

        // Obtem dados do usuário
        private async Task<User> GetFacebookUserInfo(Account account)
        {
            var request = new OAuth2Request("GET", new Uri(Constants.FacebookUserInfoUrl), null, account);
            var response = await request.GetResponseAsync();
            if (response == null)
                return null;

            var obj = JObject.Parse(response.GetResponseText());

            User user = new User
            {
                Id = obj["id"].ToString().Replace("\"", ""),
                Name = obj["name"].ToString().Replace("\"", "")
            };
            user.Picture = "https://graph.facebook.com/" + user.Id + "/picture";

            // email não pode ser vazio
            if (obj["email"] != null)
                user.Email = obj["email"].ToString().Replace("\"", "");
            else
                user.Email = user.Id + "@Facebook";

            return user;
        }

        public void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            if (sender is OAuth2Authenticator authenticator)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            // mostra mensagem de erro
            DependencyService.Get<IMessage>().LongAlert("Authentication error: " + e.Message);
        }
    }
}