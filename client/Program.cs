using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        StartClient();
    }

    static void StartClient()
    {
        TcpClient clientSocket = new TcpClient();
        clientSocket.Connect("127.0.0.1", 8888);
        Console.WriteLine("Client Socket Program - Server Connected ...");

        // Solicitar o nome do usuário
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();

        // Enviar o nome do usuário para o servidor
        NetworkStream networkStream = clientSocket.GetStream();
        byte[] nameBytes = Encoding.ASCII.GetBytes(name);
        networkStream.Write(nameBytes, 0, nameBytes.Length);

        // Iniciar uma nova thread para receber mensagens do servidor
        var receiveThread = new System.Threading.Thread(() => ReceiveMessages(clientSocket));
        receiveThread.Start();

        // Solicitar entrada do usuário e enviar mensagens para o servidor
        Console.WriteLine("Enter the string to be transmitted : ");

        while (true)
        {
            Console.Write($"{name}: ");
            string message = Console.ReadLine();

            // Adicionar o caractere delimitador "$" ao final da mensagem
            message += "$";

            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
        }
    }

    // Método para receber mensagens do servidor
    static void ReceiveMessages(object clientSocketObj)
    {
        TcpClient clientSocket = (TcpClient)clientSocketObj;
        NetworkStream networkStream = clientSocket.GetStream();

        while (true)
        {
            try
            {
                // Ler os dados do servidor
                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                int bytesRead = networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                string dataFromServer = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead).TrimEnd('$');
                Console.WriteLine(dataFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" >> " + ex.ToString());
                break;
            }
        }
    }
}
