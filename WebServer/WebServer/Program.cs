using AS9;
using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

WebServer server = new WebServer();
Networking n = new Networking(NullLogger.Instance, WebServer.OnMessage, WebServer.OnDisconnect, WebServer.OnClientConnect, '\n');
//n.AwaitMessagesAsync();