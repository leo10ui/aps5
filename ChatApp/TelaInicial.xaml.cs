using ChatApp.ViewModel;

namespace ChatApp
{
    public partial class TelaInicial : ContentPage
    {
        public TelaInicial()
        {
            InitializeComponent();
        }

        async void NavegarChat(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new AppShell());

        }

    }
}
