using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class MessageController
{
    public async Task HandleMessageEvent(WebSocket webSocket, string message, List<WebSocket> sockets)
    {
        var parts = message.Split('$');
        if (parts.Length != 2)
        {
            Console.WriteLine($"Mensagem no formato errado: {message}");
            return;
        }
        var userName = parts[0];
        var messageText = parts[1];

        await BroadcastMessageAsync(messageText, userName, sockets, webSocket);
    }

    private async Task BroadcastMessageAsync(string textMessage, string senderName, List<WebSocket> sockets, WebSocket senderSocket)
    {
        var buffer = Encoding.UTF8.GetBytes($"{senderName}: {textMessage}");
        var tasks = new List<Task>();

        foreach (var socket in sockets)
        {
            if (socket != senderSocket && socket.State == WebSocketState.Open)
            {
                var arraySegment = new ArraySegment<byte>(buffer);
                tasks.Add(socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None));
            }
        }

        await Task.WhenAll(tasks);
    }
}
