
namespace MostraRota
{
    public static class Constants
    {
        public const string AppName = "MostraRota";

        // tipo de login
        public const int FROM_GOOGLE = 1;
        public const int FROM_FACEBOOK = 2;


        // ==================================================================
        // PARAMETROS DE CONFIGURAÇÃO PARA AUTENTICAÇÃO NA GOOGLE

        // Para fazer login via Google, configure em https://console.developers.google.com/
        public const string iOSGoogleClientId = "<ID do Cliente OAuth para iOS cadastrado na Google>";

        public const string AndroidGoogleClientId = "<ID do Cliente OAuth para Android cadastrado na Google>";
        public const string AndroidGoogleClientSecret = "<Chave secreta do cliente>";

        // Esses valores não precisam serem alterados
        // lista dos Google Scope: https://developers.google.com/identity/protocols/googlescopes
        public const string GoogleScope = "https://www.googleapis.com/auth/userinfo.email";
        public const string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public const string GoogleAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public const string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Inverter os Client IDs para iOS/Android, adicionando ":/oauth2redirect" no final
        public const string iOSGoogleRedirectUrl = "<iOSGoogleClientId inverso para iOS>:/oauth2redirect";
        public const string AndroidGoogleRedirectUrl = "<AndroidGoogleClientId inverso para iOS>:/oauth2redirect";
        // ==================================================================

        // ==================================================================
        // PARAMETROS DE CONFIGURAÇÃO PARA AUTENTICAÇÃO NO FACEBOOK

        // Para fazer login via Facebook, configure em https://developers.facebook.com
        public const string iOSFacebookClientId = "<ID do Cliente OAuth para iOS cadastrado no Facebook>";
        public const string AndroidFacebookClientId = "<Código do App no Facebook>";

        // Esses valores não precisam serem alterados
        public const string FacebookScope = "";
        public const string FacebookAuthorizeUrl = "https://m.facebook.com/dialog/oauth/";
        public const string FacebookUserInfoUrl = "https://graph.facebook.com/me?fields=id,name,email";
        public const string FacebookRedirectUrl = "http://www.facebook.com/connect/login_success.html";
        // ==================================================================
    }
}
