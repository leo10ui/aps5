using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class EventsController
{
    private MessageController _messageController = new MessageController();

    public async Task HandleEvent(WebSocket webSocket, string message, List<WebSocket> sockets)
    {
        var parts = message.Split('/');
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
                await _messageController.HandleMessageEvent(webSocket, eventData, sockets);
                break;
            default:
                Console.WriteLine($"Evento desconhecido: {eventType}");
                break;
        }
    }
}
