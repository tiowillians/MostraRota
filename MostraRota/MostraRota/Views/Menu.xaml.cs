using MostraRota.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MostraRota.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        public MenuViewModel viewModel = null;

        public Menu(MasterDetailPrincipal principal)
        {
            InitializeComponent();
            viewModel = new MenuViewModel(this);
            BindingContext = viewModel;
        }
    }
}