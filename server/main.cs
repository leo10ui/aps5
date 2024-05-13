
// class MainProgram
// {
//     static void Main(string[] args)
//     {
//         ApiServer.StartServer();
//     }
// }

using MongoDB.Driver;
using MongoDB.Bson;

var connectionString = "mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+2.2.5";
if (connectionString == null)
{
    Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
    Environment.Exit(0);
}

var client = new MongoClient(connectionString);

var collection = client.GetDatabase("teste").GetCollection<BsonDocument>("posts");

var filter = Builders<BsonDocument>.Filter.Eq("title", "teste");

var document = collection.Find(filter).First();

Console.WriteLine(document);
