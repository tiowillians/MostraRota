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

        public LoginViewModel()
        {
            BypassLoginCommand = new Command(async () =>
                {
                    // fazer login por algum dos usuários cadastrados
                    App.usrCorrente = UsuariosBD.GetUsuario(null);
                    if (App.usrCorrente != null)
                    {
                        // mostra página inicial
                        await App.MostrarPaginaInicial();
                    }
                }
            );
        }
    }
}
