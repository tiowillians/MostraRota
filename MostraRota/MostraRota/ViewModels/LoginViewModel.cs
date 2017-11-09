using MostraRota.BDLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MostraRota.ViewModels
{
    public class LoginViewModel
    {
        public ICommand BypassLoginCommand { protected set; get; }

        public string URL { get; set; }

        public LoginViewModel()
        {
            UsuariosBD usr = UsuariosBD.GetUsuario(null);
            if (usr == null)
                URL = string.Empty;
            else
                URL = usr.WSUrl;

            BypassLoginCommand = new Command(async () =>
                {
                    // fazer login por algum dos usuários cadastrados
                    App.usrCorrente = UsuariosBD.GetUsuario(null);
                    if (App.usrCorrente != null)
                    {
                        if (string.IsNullOrEmpty(URL) == false)
                            App.usrCorrente.WSUrl = URL;

                        // mostra página inicial
                        await App.MostrarPaginaInicial();
                    }
                }
            );
        }
    }
}
