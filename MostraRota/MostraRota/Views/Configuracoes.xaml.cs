using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.JSON;
using MostraRota.ViewModels;
using System;
using System.Collections.Generic;
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
        }

        public async void OnLimpaBDLocal_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(App.usrCorrente.Email))
            {
                DependencyService.Get<IMessage>().ShortAlert("Usuário corrente não definido!");
                return;
            }

            // confirmar limpeza da base de dados local
            var resposta = await DisplayAlert("Limpa BD", "Confirma limpeza da base de dados local?", "Sim", "Não");
            if (resposta == false)
                return;

            // pega lista de usuários cadastrados no BD Local
            List<UsuariosBD> lstUsuarios = UsuariosBD.GetUsuarios();
            foreach (UsuariosBD usr in lstUsuarios)
            {
                // apaga todas as rotas do usuário
                if (UsuariosBD.ApagaUsuario(usr.Email) == false)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Erro Interno: dados não puderam ser apagados");
                    return;
                }
            }

            DependencyService.Get<IMessage>().ShortAlert("BD local limpo com sucesso!");
        }
    }
}
