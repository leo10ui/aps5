using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using ChatApp.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatApp;
public partial class WebSocketClient : ObservableObject
{
    private ClientWebSocket _clientWebSocket;
    public WebSocketClient()
    {
    }

    public async Task Connect(string uri)
    {
        _clientWebSocket = new ClientWebSocket();
        await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        Console.WriteLine("Connected to WebSocket server.");
    }

    public async Task SendEvent(string eventType, string eventData)
    {
        var message = $"{eventType}/{eventData}";
        await SendMessage(message);
    }

    private async Task SendMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        int bufferSize = 8192; // Tamanho do buffer que você deseja usar para fragmentos
        int offset = 0;

        while (offset < messageBytes.Length)
        {
            int chunkSize = Math.Min(bufferSize, messageBytes.Length - offset);
            bool endOfMessage = (offset + chunkSize) == messageBytes.Length;

            var buffer = new ArraySegment<byte>(messageBytes, offset, chunkSize);
            await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage, CancellationToken.None);

            offset += chunkSize;
        }

        Console.WriteLine($"Sent message: {message}");
    }

    public async Task Receive(ObservableCollection<Mensagem> mensagens)
    {
        var buffer = new byte[1024 * 512];
        var messageBuilder = new StringBuilder();

        while (_clientWebSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            do
            {
                result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var messageChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                messageBuilder.Append(messageChunk);
            }
            while (!result.EndOfMessage);

            var receivedMessage = messageBuilder.ToString();
            messageBuilder.Clear(); // Clear the StringBuilder for the next message

            Console.WriteLine($"Received message: {receivedMessage}");

            if (!string.IsNullOrEmpty(receivedMessage))
            {
                string[] msgTratada = receivedMessage.Split(new char[] { '/' },2);
                string tipoEvento = msgTratada[0];
                string[] dados = msgTratada[1].Split(new char[] { '$' },2);
                switch (tipoEvento)
                {
                    case "message":
                        var msgTexto = new Mensagem
                        {
                            Conteudo = dados[1],
                            Timestamp = DateTime.Now,
                            Emissor = dados[0]
                        };
                        mensagens.Add(msgTexto);
                        break;
                    case "imagemessage":
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        FileHandling.DecodeBase64ToFile(dados[1], documentsPath, "jpg");
                        if (!string.IsNullOrEmpty(documentsPath))
                        {
                            var msgImagem = new Mensagem
                            {
                                Emissor = dados[0],
                                FilePath = documentsPath + ".jpg",
                                Timestamp = DateTime.Now,
                                Tipo = MensagemTipo.Imagem
                            };
                            mensagens.Add(msgImagem);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public async Task Disconnect()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected.", CancellationToken.None);
        _clientWebSocket.Abort();
        Console.WriteLine("Disconnected from WebSocket server.");
    }
}

/*
public class Program
{
    static async Task Main(string[] args)
    {
        var client = new WebSocketClient();
        await client.Connect("ws://localhost:8080");

        // Enviar evento para o servidor
        await client.SendEvent("message", "Nicolas$Olá servidor!");

        // Aguardar por mensagens recebidas
        var receiveTask = client.Receive();

        // Aguardar o usuário pressionar uma tecla para desconectar
        Console.WriteLine("Pressione qualquer tecla para desconectar.");
        Console.ReadKey();

        // Desconectar do servidor
        await client.Disconnect();

        // Aguardar a conclusão da tarefa de recebimento (para garantir limpeza correta)
        await receiveTask;
    }
}
*/