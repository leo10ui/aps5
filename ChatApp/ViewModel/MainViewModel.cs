using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;

namespace ChatApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel() => Mensagens = new ObservableCollection<Mensagem>();

        [ObservableProperty]
        ObservableCollection<Mensagem> mensagens;

        [ObservableProperty]
        string nome,entryNome,titulo = "APS 5 Semestre",btnTexto = "Salvar",mensagemTxt;

        [ObservableProperty]
        bool isEntryNome = true, isEditor = false, isBtnEnviar = false;

        [ObservableProperty]
        private CollectionView collectionView;

        [RelayCommand]
        private void EnviarMensagem()
        {
            if(MensagemTxt != null)
            {
                var mensagem = new Mensagem
                {
                    Conteudo = MensagemTxt,
                    Timestamp = DateTime.Now,
                    Emissor = Nome
                };

                Mensagens.Add(mensagem);
                MensagemTxt = string.Empty;
                
            };
        }

        [RelayCommand]
        private void Limpar()
        {
            if (mensagens.Count > 0)
            {
                mensagens.Clear();
            }
        }

        [RelayCommand]
        void SalvarNome()
        {
            if (EntryNome != null && IsEntryNome)
            {
                Nome = EntryNome;
                Titulo = "Entrou no chat como: " + Nome;
                IsEntryNome = false;
                IsBtnEnviar = true;
                IsEditor = true;
                BtnTexto = "Sair";
            }
            else
            { 
                EntryNome = string.Empty;
                Nome = string.Empty;
                Titulo = "APS 5 Semestre";
                IsEntryNome = true;
                IsBtnEnviar = false;
                IsEditor = false;
                MensagemTxt = string.Empty;
                BtnTexto = "Salvar";
            }
        }
    }
}
