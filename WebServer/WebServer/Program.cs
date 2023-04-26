using AS9;
using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Author:    Aurora Zuo 
/// Partner:   Sasha Rybalkina
/// Date:      25-Apr-2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Aurora Zuo, and Sasha Rybalkina - This work not 
///            be copied for use in Academic Coursework.
///            
/// Aurora Zuo and Sasha Rybalkina certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source. All 
/// references used in the completion of the assignments are cited 
/// in our README file.
/// 
/// File Content
///     This class initializes the WebServer, and connects to the port 11001.
/// </summary>
class Program
{
    /// <summary>
    /// Initializes webser, forms networking connection to port 11001.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        WebServer server = new WebServer();
        Networking network = new Networking(NullLogger.Instance, WebServer.OnMessage, WebServer.OnDisconnect, WebServer.OnClientConnect, '\n');
        network.WaitForClients(11001, true);
        Console.ReadLine();
    }
}