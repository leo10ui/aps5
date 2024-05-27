using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson.Serialization;
using System.Text;
using System.Diagnostics.Tracing;

public class EventsController
{
    private MessageController _messageController = new MessageController();

    public async Task HandleEvent(WebSocket webSocket, string message, List<WebSocket> sockets)
    {

        var parts = message.Split(new char[] { '/' }, 2);
        if (parts.Length != 2)
        {
            Console.WriteLine($"Mensagem no formato errado: {message}");
            return;
        }

        var eventType = parts[0];
        var eventData = parts[1];
        switch (eventType)
        {
            case "message":
                await _messageController.SendMessageEvent(webSocket, eventData, sockets);
                break;
            case "imagemessage":
                await _messageController.SendImageEvent(sockets, eventData, webSocket);
                break;
            case "documentmessage":
                await _messageController.SendDocumentEvent(sockets, eventData, webSocket);
                break;
            default:
                Console.WriteLine($"Evento desconhecido: {eventType}");
                break;
        }
    }

}
