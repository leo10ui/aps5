using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketServer
{
    private readonly HttpListener _listener;
    public List<WebSocket> ConnectedSockets { get; } = new List<WebSocket>();
    private readonly EventsController eventsController = new EventsController();

    public WebSocketServer(string ipAddress, int port)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://{ipAddress}:{port}/");
    }

    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine("Servidor Iniciado.");

        while (true)
        {
            var context = await _listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                ProcessWebSocketRequest(context);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        var webSocketContext = await context.AcceptWebSocketAsync(null);
        var webSocket = webSocketContext.WebSocket;

        try
        {
            ConnectedSockets.Add(webSocket);
            await HandleWebSocketConnection(webSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 512];
        var messageBuilder = new StringBuilder();

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var messageChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                messageBuilder.Append(messageChunk);
            }
            while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = messageBuilder.ToString();
                messageBuilder.Clear(); // Clear the StringBuilder for the next message
                                        // Console.WriteLine($"Evento Recebido: {message}");

                await eventsController.HandleEvent(webSocket, message, ConnectedSockets);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                ConnectedSockets.Remove(webSocket);
            }

            Array.Clear(buffer, 0, buffer.Length);
        }
    }
}
