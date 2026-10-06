using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Oracle.ManagedDataAccess.Client;
using IBM.Data.Db2;

var connFact = new ConnectionFactory
{
    HostName = "localhost",
};
using var conn = await connFact.CreateConnectionAsync();
using var canale = await conn.CreateChannelAsync();

string coda = "InviaRicevi";
await canale.QueueDeclareAsync(
    coda,
    true,
    false,
    false,
    null
    );

string oracleConnectionString = "User Id=SYSTEM;Password=Esercizi_123;Data Source=localhost:1521/FREEPDB1;";
string db2ConnectionString = "Server=localhost:50000;Database=mydb;UID=db2admin;PWD=Esercizi_123;";

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

    try
    {
        using var db2Conn = new DB2Connection(db2ConnectionString);
        await db2Conn.OpenAsync();

        string sqlDb2 = "INSERT INTO REGISTRO_MESSAGGI (TESTO) VALUES (?)";

        using var cmdDb2 = new DB2Command(sqlDb2, db2Conn);
        cmdDb2.Parameters.Add(new DB2Parameter("@TESTO", DB2Type.VarChar)).Value = messaggio;

        await cmdDb2.ExecuteNonQueryAsync();

        Console.WriteLine("Messaggio salvato su IBM Db2");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Errore IBM Db2: {ex.Message}");
    }

    await Task.CompletedTask;
};

await canale.BasicConsumeAsync(
    coda,
    true,
    consumatore
    );

Console.WriteLine("Ascolto Messaggi, Premere Invio per Uscire.");
Console.ReadLine();