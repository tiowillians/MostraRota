using System.ComponentModel;

namespace MostraRota.ViewModels
{
    public class SincronizarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                InformaAlteracao("IsBusy");
            }
        }

        public void InformaAlteracao(string propriedade)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propriedade));
        }
    }
}
