using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

class ApiServer
{
    public static List<ClientInfo> clientList = new List<ClientInfo>();
    public static List<ClientsMessages> messagesList = new List<ClientsMessages>();


    public static void StartServer()
    {
        TcpListener serverSocket = new TcpListener(IPAddress.Any, 8888);
        serverSocket.Start();
        Console.WriteLine("🚀 Server listening on port 8000");

        while (true)
        {
            TcpClient clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Client connected");

            HandleClient client = new HandleClient();
            client.StartClient(clientSocket);
            NetworkStream networkStream = clientSocket.GetStream();
            // foreach (ClientsMessages el in messagesList)
            // {
            //     Console.WriteLine(el.ClientName);
            //     Console.WriteLine(el.TimeStamp);
            //     Console.WriteLine(el.ClientMessage);
            //     byte[] sendBytes = Encoding.ASCII.GetBytes($"{el.TimeStamp} - {el.ClientName}: {el.ClientMessage}");
            //     networkStream.Write(sendBytes, 0, sendBytes.Length);
            //     networkStream.Flush();
            // }
        }
    }

    // public static void BroadcastMessage(string message, string sender)
    // {
    //     foreach (ClientInfo clientInfo in clientList.ToArray())
    //     {
    //         try
    //         {
    //             if (clientInfo.ClientName != sender)
    //             {
    //                 TcpClient clientSocket = clientInfo.ClientSocket;

    //                 if (clientSocket != null && clientSocket.Connected)
    //                 {
    //                     NetworkStream networkStream = clientSocket.GetStream();

    //                     // Construir a mensagem formatada com o nome de usuário do remetente
    //                     string formattedMessage = $"{sender}: {message}";

    //                     byte[] sendBytes = Encoding.ASCII.GetBytes(formattedMessage);
    //                     networkStream.Write(sendBytes, 0, sendBytes.Length);
    //                     networkStream.Flush();
    //                 }
    //                 else
    //                 {
    //                     Console.WriteLine("Error broadcasting message to a client: Client socket is not connected.");
    //                 }
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine("Error broadcasting message to a client: " + ex.Message);
    //         }
    //     }
    // }
}

public class HandleClient
{
    // TcpClient clientSocket;
    // string clientName;
    public Message message = new Message();
    public void StartClient(TcpClient inClientSocket)
    {

        message.clientSocket = inClientSocket;
        Thread ctThread = new Thread(message.HandleMessage);
        ctThread.Start();
    }
}

public class ClientInfo
{
    public TcpClient ClientSocket { get; set; }
    public string ClientName { get; set; }

    public ClientInfo()
    {
        ClientSocket = null;
        ClientName = string.Empty;
    }

}
public class ClientsMessages
{
    public String ClientName { get; set; }
    public String ClientMessage { get; set; }
    public DateTime TimeStamp { get; set; }
    public ClientsMessages()
    {
        ClientName = string.Empty;
        ClientMessage = string.Empty;
        TimeStamp = DateTime.Now;
    }
}