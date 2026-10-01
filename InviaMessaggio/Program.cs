using RabbitMQ.Client;
using System.Text;

var connFact = new ConnectionFactory
{
    HostName = "localhost",
};
using var conn = await connFact.CreateConnectionAsync();
using var canale = await conn.CreateChannelAsync();

string coda = "InviaRicevi";
await canale.QueueDeclareAsync(
    coda,
    false,
    false,
    false,
    null
    );

Console.Write("Messaggio da Inviare: ");
string messaggio = Console.ReadLine() ?? string.Empty;

var contenuto = Encoding.UTF8.GetBytes(messaggio);
await canale.BasicPublishAsync(
    string.Empty,
    coda, 
    contenuto
    );

Console.WriteLine("Messagio Inviato.");