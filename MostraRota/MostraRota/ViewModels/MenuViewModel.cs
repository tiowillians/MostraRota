using MostraRota.Interfaces;
using MostraRota.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MostraRota.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public string Nome { get; set; }
        public object Foto { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void InformaAlteracao(string propriedade)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }

        public ICommand MenuTapped { protected set; get; }

        public MenuViewModel(Menu menu)
        {
            if (App.usrCorrente == null)
            {
                Nome = "Não Autenticado";
                Foto = ImageSource.FromResource("usuario.png");
            }
            else
            {
                Nome = App.usrCorrente.Nome;
                Foto = Utils.ConvertToImage(App.usrCorrente.Foto);
            }

            this.MenuTapped = new Command<string>(async (id) =>
            {
                switch (id)
                {
                    // Configurações
                    case "config":
                        await App.NavegarParaPagina(new Configuracoes());
                        break;

                    // Sincronizar Rotas
                    case "sinc":
                        await App.NavegarParaPagina(new Sincronizar());
                        break;

                    // Visualizar Rotas
                    case "ver":
                        await App.NavegarParaPagina(new VerRotas());
                        break;

                    // Fazer novo Login
                    case "login":
                        App.VoltarParaLogin();
                        break;

                    // Sobre o Aplicativo
                    case "sobre":
                        await App.NavegarParaPagina(new Sobre());
                        break;

                    // Sair da Aplicação
                    case "sair":
                        DependencyService.Get<IDeviceSpecific>().CloseApplication();
                        break;
                }
            });
        }
    }
}
