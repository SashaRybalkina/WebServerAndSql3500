using AS9;
using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

class Program
{
    public static void Main(string[] args)
    {
        WebServer server = new WebServer();
        Networking network = new Networking(NullLogger.Instance, WebServer.OnMessage, WebServer.OnDisconnect, WebServer.OnClientConnect, '\n');
        network.WaitForClients(11001, true);
        Console.ReadLine();
    }
}