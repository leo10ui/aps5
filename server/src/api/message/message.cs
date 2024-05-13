using System.Net.Sockets;
using System.Text;

public class Message
{
    string clientName = "";
    public TcpClient clientSocket;

    public void HandleMessage()
    {
        NetworkStream networkStream = clientSocket.GetStream();

        byte[] nameBytes = new byte[clientSocket.ReceiveBufferSize];
        int nameBytesLength = networkStream.Read(nameBytes, 0, nameBytes.Length);
        clientName = Encoding.ASCII.GetString(nameBytes, 0, nameBytesLength);
        Console.WriteLine(clientName);

        ApiServer.clientList.Add(new ClientInfo { ClientSocket = clientSocket, ClientName = clientName });

        while (true)
        {
            try
            {
                if (networkStream.DataAvailable)
                {
                    byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    string dataFromClient = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead).TrimEnd('$');
                    Console.WriteLine(dataFromClient);
                    BroadcastMessage(dataFromClient, clientName);
                    ApiServer.messagesList.Add(new ClientsMessages { ClientName = clientName, ClientMessage = dataFromClient });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> Error handling client connection: " + ex.Message);
                break;
            }
        }
    }
    public static void BroadcastMessage(string message, string sender)
    {
        foreach (ClientInfo clientInfo in ApiServer.clientList.ToArray())
        {
            try
            {
                if (clientInfo.ClientName != sender)
                {
                    TcpClient clientSocket = clientInfo.ClientSocket;

                    if (clientSocket != null && clientSocket.Connected)
                    {
                        NetworkStream networkStream = clientSocket.GetStream();

                        string formattedMessage = $"{sender}: {message}";

                        byte[] sendBytes = Encoding.ASCII.GetBytes(formattedMessage);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                    }
                    else
                    {
                        Console.WriteLine("Error broadcasting message to a client: Client socket is not connected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error broadcasting message to a client: " + ex.Message);
            }
        }
    }
}