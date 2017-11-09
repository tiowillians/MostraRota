using MostraRota.Views;
using System.ComponentModel;

namespace MostraRota.ViewModels
{
    public class ConfiguracoesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void InformaAlteracao(string propriedade)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }

        public string URL { get; set; }
        public bool GravaOn { get; set; }
        public bool WifiOn { get; set; }

        public ConfiguracoesViewModel()
        {
            URL = App.usrCorrente.WSUrl;
            GravaOn = (App.usrCorrente.Grava == 1);
            WifiOn = (App.usrCorrente.WiFi == 1);
        }
    }
}

