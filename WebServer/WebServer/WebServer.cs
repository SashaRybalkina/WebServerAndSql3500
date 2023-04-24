using Communications;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace AS9
{
    /// <summary>
    /// Author:   H. James de St. Germain
    /// Date:     Spring 2020
    /// Updated:  Spring 2023
    /// 
    /// Code for a simple web server
    /// </summary>
    class WebServer
    {
        /// <summary>
        /// keep track of how many requests have come in.  Just used
        /// for display purposes.
        /// </summary>

        private static DataBase DB = new DataBase();

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// Print an information message
        /// </summary>
        /// <param name="channel"> the Networking connection</param>

        internal static void OnClientConnect(Networking channel)
        {
            Console.WriteLine("Recieved Connection");
        }

        /// <summary>
        /// Create the HTTP response header, containing items such as
        /// the "HTTP/1.1 200 OK" line.
        /// 
        /// See: https://www.tutorialspoint.com/http/http_responses.htm
        /// 
        /// Warning, don't forget that there have to be new lines at the
        /// end of this message!
        /// </summary>
        /// <param name="length"> how big a message are we sending</param>
        /// <param name="type"> usually html, but could be css</param>
        /// <returns>returns a string with the response header</returns>
        private static string BuildHTTPResponseHeader(int length, string type = "text/html")
        {
            return $"HTTP/1.1 200 OK\rDate: {DateTime.Now}\rContent-Length: {length}\rContent-Type: text/html\rConnection: Closed";
        }


        /// <summary>
        ///   Create a web page!  The body of the returned message is the web page
        ///   "code" itself. Usually this would start with the doctype tag followed by the HTML element.  Take a look at:
        ///   https://www.sitepoint.com/a-basic-html5-template/
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuildHTTPBody()
        {
            return "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                        "<h1>Welcome to the Home Page! :)</h1>" +
                        "<p>Supported Requests:\r</p>"+
                        "<a href=“http://localhost:11001”\r> localhost:11001 </a>" +
                        "<p></p>" +
                        "<a href=“http://localhost:11001/”\r> localhost:11001/ </a>" +
                        "<p></p>" +
                        "<a href=“http://localhost:11001/index”\r> localhost:11001/index </a>" +
                        "<p></p>" +
                        "<a href=“http://localhost:11001/index.html”> localhost:11001/index.html a>" +
                    "</body>" +
                    "</html>";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPHighScoresPage()
        {
            Dictionary<string, string> players = DB.ReadPlayerScores();
            string AddToTable = "";
            foreach(string player in players.Keys)
            {
                AddToTable += $@"<tr><td>{player}</td><td>{players[player]}</td></tr>";
            }
            return  "<!DOCTYPE html>"+
                    "<html>"+
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                    "<h1> Highscores </h1> " +
                        $@"<table>"+
                            "<tr>"+
                            "<th>Player</th>"+
                            "<th>Score</th>"+
                            "</tr>"+
                            AddToTable+
                        "</table>"+
                    "</body>"+
                    "</html>";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        private static string BuildHTTPScoresOfPlayer(string playerName)
        {
            Dictionary<string, List<List<string>>> ScoresAndGames = DB.ReadScoresOfPlayers();
            string AddToTable = "";
            foreach (List<string> ScoreAndGame in ScoresAndGames[playerName])
            {
                string score = ScoreAndGame[0];
                string gameID = ScoreAndGame[1];
                AddToTable += $@"<tr><td>{score}</td><td>{gameID}</td><tr>";
            }
            return "<!DOCTYPE html>"+
                    "<html>"+
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                        $@"<h1>{playerName}'s Scores</h1>"+
                        $@"<table>"+
                            "<tr>" +
                            "<th>Score</th>" +
                            "<th>Game</th>" +
                            "" +
                            AddToTable +
                        "</table>"+
                    "</body>"+
                    "</html>";
        }

        /// <summary>
        /// Create a response message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new line]
        ///   HTTP Body
        ///  
        ///  The Header must follow the header protocol.
        ///  The body should follow the HTML doc protocol.
        /// </summary>
        /// <returns> the complete HTTP response</returns>
        private static string BuildMainPage()
        {
            string message = BuildHTTPBody();
            string header = BuildHTTPResponseHeader(message.Length);

            return header + message;
        }

        /// <summary>
        ///   <para>
        ///     When a request comes in (from a browser) this method will
        ///     be called by the Networking code.  Each line of the HTTP request
        ///     will come as a separate message.  The "line" we are interested in
        ///     is a PUT or GET request.  
        ///   </para>
        ///   <para>
        ///     The following messages are actionable:
        ///   </para>
        ///   <para>
        ///      get highscore - respond with a highscore page
        ///   </para>
        ///   <para>
        ///      get favicon - don't do anything (we don't support this)
        ///   </para>
        ///   <para>
        ///      get scores/name - along with a name, respond with a list of scores for the particular user
        ///   <para>
        ///      get scores/name/highmass/highrank/startime/endtime - insert the appropriate data
        ///      into the database.
        ///   </para>
        ///   </para>
        ///   <para>
        ///     create - contact the DB and create the required tables and seed them with some dummy data
        ///   </para>
        ///   <para>
        ///     get index (or "", or "/") - send a happy home page back
        ///   </para>
        ///   <para>
        ///     get css/styles.css?v=1.0  - send your sites css file data back
        ///   </para>
        ///   <para>
        ///     otherwise send a page not found error
        ///   </para>
        ///   <para>
        ///     Warning: when you send a response, the web browser is going to expect the message to
        ///     be line by line (new line separated) but we use new line as a special character in our
        ///     networking object.  Thus, you have to send _every line of your response_ as a new Send message.
        ///   </para>
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        internal static void OnMessage(Networking channel, string message)
        {
            if (message.Contains("/scores/"))
            {
                string player = message.Remove(0, 12);
                int toRemove = player.Length - 10;
                player = player.Substring(0, toRemove);
                string body = BuildHTTPScoresOfPlayer(player);
                string header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else if (message.Contains("/highscores"))
            {
                string body = BuildHTTPHighScoresPage();
                string header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else if (message.Contains("localhost:11001")
                  || message.Contains("localhost:11001/")
                  || message.Contains("localhost:11001/index.html")
                  || message.Contains("localhost:11001/index"))
            {
                string body = BuildHTTPBody();
                string header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
                channel.Disconnect();
            }
        }

        /// <summary>
        /// Handle some CSS to make our pages beautiful
        /// </summary>
        /// <returns>HTTP Response Header with CSS file contents added</returns>
        private static string SendCSSResponse()
        {
            return  "<style>" +
                        "body {background-color: darkorchid;}" +
                        "table, th, td {" +
                            "border: 2px solid deeppink;" +
                            "width: 50%;" +
                            "color: deeppink;" +
                            "background-color: purple;" +
                        "}" +
                        "table {" +
                            "width: 100%;" +
                        "}" +
                        "td {" +
                            "text-align: center;" +
                        "}" +
                        "h1 {" +
                            "color: hotpink;" +
                            "text-align: center;" +
                        "}" +
                    "</style>";

        }


        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            throw new NotImplementedException("create the database tables by 'talking' with the DB server and then return an informative web page");
        }

        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
        }
    }
}