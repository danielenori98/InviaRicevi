using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Oracle.ManagedDataAccess.Client;

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

string oracleConnectionString = "User Id=SYSTEM;Password=Esercizi#123;Data Source=localhost:1521/FREEPDB1;";

var consumatore = new AsyncEventingBasicConsumer(canale);

consumatore.ReceivedAsync += async (model, ea) =>
{
    var messaggio = Encoding.UTF8.GetString(ea.Body.ToArray());

    Console.WriteLine($"Messaggio:{messaggio}");

    try
    {
        using var oracleConn = new OracleConnection(oracleConnectionString);
        await oracleConn.OpenAsync();

        string sql = "INSERT INTO REGISTRO_MESSAGGI (TESTO) VALUES (:testo)";

        using var cmd = new OracleCommand(sql, oracleConn);
        cmd.Parameters.Add(new OracleParameter("testo", messaggio));

        await cmd.ExecuteNonQueryAsync();

        Console.WriteLine("Messaggio salvato su DB");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Errore DB: {ex.Message}");
    }

    await Task.CompletedTask;
};

await canale.BasicConsumeAsync(
    coda,
    true,
    consumatore
    );

Console.WriteLine("Ascolto Messaggi, Premere un Tasto per Uscire.");
Console.ReadLine();