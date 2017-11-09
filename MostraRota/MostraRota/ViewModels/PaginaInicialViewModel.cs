using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace MostraRota.ViewModels
{
    public class PaginaInicialViewModel : INotifyPropertyChanged
    {
        public double DistTotal { get; set; }
        public string IniciarParar { get; set; }
        public bool MostraPopup { get; set; }
        public string Pergunta { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InformaAlteracao(string propriedade)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }

        public PaginaInicialViewModel()
        {
            DistTotal = 0.0;
            IniciarParar = "Iniciar";
            MostraPopup = false;
        }
    }
}
