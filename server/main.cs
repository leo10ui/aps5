class Program
{
    static async Task Main(string[] args)
    {
        var server = new WebSocketServer("localhost", 8080);
        await server.Start();
    }
}
