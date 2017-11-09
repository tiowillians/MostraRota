using MostraRota.BDLocal;
using MostraRota.Interfaces;
using MostraRota.Models;
using MostraRota.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace MostraRota.ViewModels
{
    public class VerRotasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void InformaAlteracao(string propriedade)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }

        public ObservableCollection<RotaModel> Rotas { get; set; }
        private VerRotas parentPage;

        public ICommand ApagarCommand { protected set; get; }
        public ICommand VerCommand { protected set; get; }

        public VerRotasViewModel(VerRotas pai)
        {
            parentPage = pai;

            // apagar rota selecionada
            this.ApagarCommand = new Command(async () =>
            {
                // verifica se existe rota selecionada na lista de rotas
                if (parentPage.RotaSelecionada == null)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Nenhuma rota foi selecionada.");
                    return;
                }

                // confirma eliminação da rota
                var resposta = await parentPage.DisplayAlert("Apaga Rota", "Confirma eliminação da rota selecionada?", "Sim", "Não");
                if (resposta)
                {
                    if (RotasBD.ApagaRota(parentPage.RotaSelecionada.RotaId))
                    {
                        // apaga rota da lista de rotas
                        Rotas.Remove(parentPage.RotaSelecionada);
                        InformaAlteracao("Rotas");
                    }
                    else
                        DependencyService.Get<IMessage>().ShortAlert("Erro na eliminação da rota!");
                }
            });

            // ver rota selecionada no mapa
            this.VerCommand = new Command(async () =>
            {
                // verifica se existe rota selecionada na lista de rotas
                if (parentPage.RotaSelecionada == null)
                {
                    DependencyService.Get<IMessage>().ShortAlert("Nenhuma rota foi selecionada.");
                    return;
                }

                // verifica se existem coordenadas na rota selecionada
                if ((parentPage.RotaSelecionada.Coordenadas == null) ||
                    (parentPage.RotaSelecionada.Coordenadas.Count < 2))
                {
                    DependencyService.Get<IMessage>().ShortAlert("Rota selecionada não possui nenhum trajeto.");
                    return;
                }

                // mostra rota no mapa
                await App.NavegarParaPagina(new VerRotaNoMapa(parentPage.RotaSelecionada.Coordenadas));
            });
        }
    }
}
