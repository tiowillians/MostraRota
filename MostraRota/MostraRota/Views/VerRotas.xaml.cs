using MostraRota.BDLocal;
using MostraRota.CustomControls;
using MostraRota.Models;
using MostraRota.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerRotas : ContentPage
    {
        private VerRotasViewModel viewModel;
        public RotaModel RotaSelecionada { get; set; }

        public VerRotas()
        {
            InitializeComponent();
            viewModel = new VerRotasViewModel(this);
            this.BindingContext = viewModel;

            // inicialmente nenhuma rota foi selecionada
            RotaSelecionada = null;

            // mostrar todas as rotas
            MostraRotas();
        }

        // usuário clicou sobre a lista de rotas
        async void Rota_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // seleciona item
            if (RotaSelecionada == null)
                RotaSelecionada = (RotaModel)e.Item;
            else
            {
                // verifica se usuário clicou sobre a rota que já estava selecionada
                RotaModel item = (RotaModel)e.Item;
                if (item == RotaSelecionada)
                {
                    // "desseleciona" a rota
                    RotaSelecionada = null;
                    listview_Rotas.SelectedItem = null;
                }
                else
                    // seleciona o item
                    RotaSelecionada = item;
            }
        }

        public void MostraRotas()
        {
            viewModel.Rotas = new ObservableCollection<RotaModel>();

            List<RotasBD> lista = RotasBD.GetRotas(App.usrCorrente.Email);
            if (lista == null)
                return;

            RotaModel nova;
            MyPosition pos;
            List<MyPosition> posicoes;
            List<CoordenadasBD> bdCoord;
            foreach (RotasBD rota in lista)
            {
                posicoes = new List<MyPosition>();
                bdCoord = CoordenadasBD.GetCoordenadas(rota.Id);
                foreach(CoordenadasBD coord in bdCoord)
                {
                    pos = new MyPosition(coord.Latitude, coord.Longitude, coord.DataHora);
                    posicoes.Add(pos);
                }

                nova = new RotaModel(rota.Id, rota.DtHrInicial, rota.DtHrFinal,
                                     rota.Distancia, posicoes);
                viewModel.Rotas.Add(nova);
            }

            viewModel.InformaAlteracao("Rotas");
        }
    }
}