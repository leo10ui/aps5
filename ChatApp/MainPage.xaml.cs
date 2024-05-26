using ChatApp.ViewModel;
using ChatApp;

namespace ChatApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        async void NavegarChat(object sender, EventArgs args)
        {
            await Navigation.PopToRootAsync();
        }

    }

}
