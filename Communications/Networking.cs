using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

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
///		This program contains an abstract for a Networking object and "hides" the
///		"gory details" of creating connections. It interacts with a "client code"
///		via a callback mechanism. The code uses the TcpClient object and its
///		various async method.
///		The Networking object will be used in ClientGUI.
/// </summary>
namespace Communications
{
    public class Networking
    {
        //define three basic delegate for connection, message hadnler and disconnection.
        public delegate void ReportMessageArrived(Networking channel, string message);
        private ReportMessageArrived handleMessage;

        public delegate void ReportDisconnect(Networking channel);
        private ReportDisconnect handleDisconnect;

        public delegate void ReportConnectionEstablished(Networking channel);
        private ReportConnectionEstablished handleConnection;

        private readonly char endTerminator;
        CancellationTokenSource waitForCancellation = new();

        // define a tcpClient
        public TcpClient tcpClient { get; set; }
        //define a logger
        public ILogger logger;
        // let ID empty if there is nothing happened
        private string id = string.Empty;
        /// <summary>
        /// if user do not set ID, we say that is RemoteEndPoint
        /// if user set ID, we say that is user Input.
        /// </summary>
        public string ID
        {
            // set user ID
            get
            {
                if (id.Length == 0)
                    id = RemoteAddressPort;

                return id;
            }
            set { id = value; }
        }
        public string RemoteAddressPort
        {
            //default ID
            get
            {
                if (tcpClient.Connected)
                {
                    return $"{this.tcpClient.Client.RemoteEndPoint}";
                }
                else
                    return "";

            }
        }

        /// <summary>
        /// Constructor that initiates the delegates and other fields
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="onMessage"></param>
        /// <param name="onDisconnect"></param>
        /// <param name="onConnect"></param>
        /// <param name="terminationCharacter"></param>
        public Networking(ILogger logger, ReportMessageArrived onMessage, ReportDisconnect onDisconnect,
            ReportConnectionEstablished onConnect, char terminationCharacter)
        {
            // three delegates stored over here 
            this.handleMessage = onMessage;
            this.handleDisconnect = onDisconnect;
            this.handleConnection = onConnect;
            // store end character over here.
            endTerminator = terminationCharacter;
            // store Ilooger over here.
            this.logger = logger;
            tcpClient = new TcpClient();
        }
        /// <summary>
        /// let user connect with server. and the logic in this method
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Connect(string host, int port)
        {
            // assgin our port and host to server for connection, and do connecction operation
            // then await for message, if there has any exception we throw exception(it will be catch in somewhere)
            try
            {
                tcpClient = new TcpClient(host, port);
                handleConnection(this);
                if (tcpClient.Connected)
                {
                    //logger.LogInformation($"there one connection going to connected and ID is {this.tcpClient.Client.RemoteEndPoint}");
                    //this.AwaitMessagesAsync(true);
                }

            }
            catch
            {
                throw new ArgumentException("TcpClient constructor an exception occurs");
            }
        }
        /// <summary>
        /// await for message once connection established
        /// </summary>
        /// <param name="infinite"></param>
        public async void AwaitMessagesAsync(bool infinite = true)
        {
            // stringbuilder store full message and buffer store paritial message.
            try
            {
                StringBuilder dataBacklog = new StringBuilder();
                byte[] buffer = new byte[4096];
                NetworkStream stream = tcpClient.GetStream();
                // if stream is null we just return.
                if (stream == null)
                {
                    return;
                }
                // if full message do one message operation otherwise do multiple times. if there has any exception
                //throw that out
                do
                {
                    int total = await stream.ReadAsync(buffer);
                    string current_data = Encoding.UTF8.GetString(buffer, 0, total);
                    dataBacklog.Append(current_data);
                    logger.LogInformation($"  Received {total} new bytes for a total of {dataBacklog.Length}.");
                    this.checkForMessage(dataBacklog);


                } while (infinite);


            }
            catch (Exception ex)
            {
                logger.LogInformation($"Exception wait message{ex}");
                handleDisconnect(this);
            }
        }
        /// <summary>
        /// check received message from server.
        /// </summary>
        /// <param name="fullMessageHolder"></param>
        private void checkForMessage(StringBuilder fullMessageHolder)
        {

            // if messages is single message or full message, it should put 
            // into message callback 
            string allData = fullMessageHolder.ToString();
            int terminator_position = allData.IndexOf(endTerminator);
            while (terminator_position >= 0)
            {
                string message = allData.Substring(0, terminator_position);
                fullMessageHolder.Remove(0, terminator_position + 1);
                handleMessage(this, message);
                allData = fullMessageHolder.ToString();
                terminator_position = allData.IndexOf(endTerminator);
            }
        }
        /// <summary>
        /// this method is used by server and wait clients connected with server
        /// </summary>
        /// <param name="port"></param>
        /// <param name="infinite"></param>
        public async void WaitForClients(int port, bool infinite)
        {
            TcpListener network_listener = new TcpListener(IPAddress.Any, port);
            network_listener.Start();
            // do while here and everything is this while loop
            do
            {
                try
                {
                    // once server received client but connection into new network and await client's message,
                    // if server shuwdown, listener will stop listen any clients
                    TcpClient connection = await network_listener.AcceptTcpClientAsync(waitForCancellation.Token);
                    logger.LogInformation($"\n ** New Connection ** Accepted From " +
                        $"{connection.Client.RemoteEndPoint} to {connection.Client.LocalEndPoint}\n");
                    Networking network = new Networking(logger, this.handleMessage,
                        this.handleDisconnect, this.handleConnection, this.endTerminator);

                    network.tcpClient = connection;
                    handleConnection(network);
                    network.AwaitMessagesAsync(true);

                }
                catch
                {
                    network_listener.Stop();
                    infinite = false;
                }
            }
            while (infinite);

        }
        /// <summary>
        /// invoke cancel method
        /// </summary>
        public void StopWaitingForClients()
        {
            waitForCancellation.Cancel();
        }
        /// <summary>
        /// step for client.
        /// </summary>
        public void Disconnect()
        {
            tcpClient.Close();
        }
        /// <summary>
        /// send message from server to client or client to server.
        /// </summary>
        /// <param name="text"></param>
        public async void Send(string text)
        {
            // add endterminator for every message before they send messsage
            try
            {
                if (text.Contains('\n'))
                    throw new Exception();
                text += '\n';
                await this.tcpClient.GetStream().WriteAsync(Encoding.UTF8.GetBytes(text));
            }
            catch
            {
                logger.LogInformation($"Failed to send {text}");
                handleDisconnect(this);
            }
        }
    }
}