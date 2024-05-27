using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class MessageController
{
    public async Task SendMessageEvent(WebSocket webSocket, string message, List<WebSocket> sockets)
    {   //exemplo
        //"message/username$textodamensagem"
        var parts = message.Split(new char[] { '$' }, 2);
        if (parts.Length != 2)
        {
            Console.WriteLine($"Mensagem no formato errado: {message}");
            return;
        }
        var userName = parts[0];
        var messageText = parts[1];

        messageText = $"message/{userName}${messageText}";
        await BroadcastMessageAsync(messageText, sockets, webSocket);
    }

    public async Task SendImageEvent(List<WebSocket> sockets, string dataEvent, WebSocket senderSocket)
    {
        // "documentmessage/nomedocara$base64"
        var partsEvent = dataEvent.Split(new char[] { '$' }, 2);
        var userName = partsEvent[0];
        var eventDataDoc = partsEvent[1];
        var messageText = $"imagemessage/{userName}${eventDataDoc}";
        await BroadcastMessageAsync(messageText, sockets, senderSocket);
    }

    public async Task SendDocumentEvent(List<WebSocket> sockets, string dataEvent, WebSocket senderSocket)
    {
        //exemplo
        // "documentmessage/nomedocara$pdf!base64"
        var partsEvent = dataEvent.Split(new char[] { '$' }, 2);
        var userName = partsEvent[0];
        var eventDataDoc = partsEvent[1];
        var eventDataDocSplit = eventDataDoc.Split(new char[] { '!' }, 2);
        var documentType = eventDataDocSplit[0];
        var documentData = eventDataDocSplit[1];
        var messageText = $"documentmessage/{userName}${documentType}!{documentData}";
        await BroadcastMessageAsync(messageText, sockets, senderSocket);
    }

    private async Task BroadcastMessageAsync(string textMessage, List<WebSocket> sockets, WebSocket senderSocket)
    {
        var buffer = Encoding.UTF8.GetBytes($"{textMessage}");
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
