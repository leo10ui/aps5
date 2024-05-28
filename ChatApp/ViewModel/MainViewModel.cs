using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ChatApp.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel() => Mensagens = new ObservableCollection<Mensagem>();

    [ObservableProperty]
    ObservableCollection<Mensagem> mensagens;

    private WebSocketClient client = new WebSocketClient();

    [ObservableProperty]
    string nome,entryNome,titulo = "Ecochat",btnTexto = "Entrar",mensagemTxt,anexoTxt,testando = "É isso";

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

            EnviarEvento("message",mensagem.Emissor, mensagem.Conteudo);
            
        };
    }

    [RelayCommand]
    private void EnviarImagem(string filePath)
    {
        if(filePath != null)
        {
            var mensagem = new Mensagem
            {
                Emissor = Nome,
                FilePath = filePath,
                Timestamp = DateTime.Now,
                Tipo = MensagemTipo.Imagem
            };

            Mensagens.Add(mensagem);
        }
    }

    [RelayCommand]
    private void EnviarDocumento(string filePath)
    {
        if (filePath != null)
        {
            var mensagem = new Mensagem
            {
                FilePath = filePath,
                Timestamp = DateTime.Now,
                Tipo = MensagemTipo.Documento
            };

            Mensagens.Add(mensagem);
        }
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
            // Ação entrar
            Conectar();
            Nome = EntryNome;
            Titulo = "Entrou no chat como: " + Nome;
            IsEntryNome = false;
            IsBtnEnviar = true;
            IsEditor = true;
            BtnTexto = "Sair";

            var mensagem = new Mensagem
            {
                Conteudo = Nome + " entrou do chat",
                Timestamp = DateTime.Now,
                Emissor = String.Empty
            };

            EnviarEvento("message","-", mensagem.Conteudo);
            Mensagens.Add(mensagem);
        }
        else
        {
            // Ação sair
            Desconectar();
            var mensagem = new Mensagem
            {
                Conteudo = Nome + " saiu do chat",
                Timestamp = DateTime.Now,
                Emissor = ""
            };

            EnviarEvento("message","-", mensagem.Conteudo);
            Mensagens.Add(mensagem);

            EntryNome = string.Empty;
            Nome = string.Empty;
            Titulo = "APS 5 Semestre";
            IsEntryNome = true;
            IsBtnEnviar = false;
            IsEditor = false;
            MensagemTxt = string.Empty;
            AnexoTxt= string.Empty;
            BtnTexto = "Entrar";
        }
    }

    // Lógica para anexo de arquivos
    [RelayCommand]
    public async Task<FileResult> AnexarArquivo(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                AnexoTxt = result.FileName;
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    EnviarImagem(result.FullPath);
                    AnexoTxt = string.Empty;
                    EnviarEvento("imagemessage", Nome,result.FullPath);
                }
            }

            return result;
        }
        catch (Exception ex)
        { }

        return null;
    }

    private async void Conectar()
    {
        await client.Connect("ws://localhost:8080");
        await client.Receive(mensagens);
    }
    private async void Desconectar()
    {
        await client.Disconnect();
    }

    private async void EnviarEvento(string tipoEvento, string emissor, string info)
    {
        string dados = "";
        try
        {
            switch (tipoEvento)
            {
                case "message":
                    dados = emissor + "$" + info;
                    break;
                case "imagemessage":
                    dados = emissor + "$" + FileHandling.ConvertFileToBase64(info);
                    break;
                case "documentmessage":
                    string extension = Path.GetExtension(info).ToLower();
                    dados = emissor + "$" + extension.Remove(extension.IndexOf('.')) + "!" + FileHandling.ConvertFileToBase64(info);
                    break;

                default:
                    Console.WriteLine($"Evento desconhecido: {tipoEvento}");
                    break;
            }
            await client.SendEvent(tipoEvento, dados);
        } catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex);
        }
    }

}

