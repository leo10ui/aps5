using System.Collections.ObjectModel;
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
        _clientWebSocket = new ClientWebSocket();
    }

    public async Task Connect(string uri)
    {
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
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Sent message: {message}");
    }

    public async Task Receive(ObservableCollection<Mensagem> mensagens)
    {
        var buffer = new byte[1024];
        while (_clientWebSocket.State == WebSocketState.Open)
        {
            var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {receivedMessage}");

            if (receivedMessage != null)
            {
                string[] msgTratada = receivedMessage.Split(':');

                var mensagem = new Mensagem
                {
                    Conteudo = msgTratada[1],
                    Timestamp = DateTime.Now,
                    Emissor = msgTratada[0]
                };
                mensagens.Add(mensagem);
            }
        }
    }

    public async Task Disconnect()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected.", CancellationToken.None);
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