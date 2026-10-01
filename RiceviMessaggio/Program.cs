using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

var consumatore = new AsyncEventingBasicConsumer(canale);

consumatore.ReceivedAsync += async (model, ea) =>
{
    var messaggio = Encoding.UTF8.GetString(ea.Body.ToArray());

    Console.WriteLine($"Messaggio:{messaggio}");

    await Task.CompletedTask;
};

await canale.BasicConsumeAsync(
    coda,
    true,
    consumatore
    );

Console.WriteLine("Ascolto Messaggi, Premere un Tasto per Uscire.");
Console.ReadLine();