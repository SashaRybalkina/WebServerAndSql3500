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
        /// 
        /// </summary>
        /// <param name="channel"></param>
        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
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
                        "<h2>Supported Requests:\r</h2>"+
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
        private static string BuildPageNotFound()
        {
            return "<!DOCTYPE html>" +
                    "<html>" +
                    "<body>" +
                        "<h1>Page not found :(</h1>" +
                        "<p>The link you entered isn't supported by the browser</p>" +
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
                            "</tr>" +
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
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
        /// </summary>
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            //CREATE
            //INSERT
            //x6 for all tables

            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<DataBase>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("DataSecrets");

            string connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["DataSource"],
                InitialCatalog = SelectedSecrets["InitialCatalog"],
                UserID = SelectedSecrets["UserID"],
                Password = SelectedSecrets["Password"],
                Encrypt = false
            }.ConnectionString;

            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand insertMass = new SqlCommand("CREATE TABLE NewAllGames", con);
            using SqlDataReader reader1 = insertMass.ExecuteReader();

            con.Close();


            return "";
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
            string body = "";
            string header = "";

            if (message.Contains("/scores/") && message.ToCharArray().Count(c => c == '/') == 7)
            {
                string[] splitMessage = message.Split("/");

                string name = splitMessage[2];
                string highmass = splitMessage[3];
                string highrank = splitMessage[4];
                string starttime = splitMessage[5];
                string endtime = splitMessage[6].Substring(0, splitMessage[6].Length-5);

                var builder = new ConfigurationBuilder();

                builder.AddUserSecrets<DataBase>();
                IConfigurationRoot Configuration = builder.Build();
                var SelectedSecrets = Configuration.GetSection("DataSecrets");

                string connectionString = new SqlConnectionStringBuilder()
                {
                    DataSource = SelectedSecrets["DataSource"],
                    InitialCatalog = SelectedSecrets["InitialCatalog"],
                    UserID = SelectedSecrets["UserID"],
                    Password = SelectedSecrets["Password"],
                    Encrypt = false
                }.ConnectionString;

                using SqlConnection con = new SqlConnection(connectionString);

                con.Open();

                using SqlCommand insertMass = new SqlCommand($@"INSERT INTO HighMass VALUES ('{name}', '{highmass}')", con);
                using SqlDataReader reader1 = insertMass.ExecuteReader();

                con.Close();

                con.Open();

                using SqlCommand insertRank = new SqlCommand($@"INSERT INTO HighRank VALUES ('{name}', '{highrank}')", con);
                using SqlDataReader reader2 = insertRank.ExecuteReader();

                con.Close();

                con.Open();

                using SqlCommand insertTime = new SqlCommand($@"INSERT INTO Time VALUES ('{name}', '{starttime}', '{endtime}')", con);
                using SqlDataReader reader3 = insertTime.ExecuteReader();

                con.Close();

            }
            else if (message.Contains("/scores/"))
            {
                string player = message.Remove(0, 12);
                int toRemove = player.Length - 10;
                player = player.Substring(0, toRemove);
                body = BuildHTTPScoresOfPlayer(player);
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else if (message.Contains("/highscores"))
            {
                body = BuildHTTPHighScoresPage();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            
            else if (message.Contains("GET / HTTP/1.1\r")
                  || message.Contains("GET /index HTTP/1.1\r")
                  || message.Contains("GET /index.html HTTP/1.1\r"))
            {
                body = BuildHTTPBody();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else if (message.Contains("/favicon"))
            {
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else if (message.Contains("/create"))
            {
                body = CreateDBTablesPage();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }
            else
            {
                body = BuildPageNotFound();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
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
                        "h2 {" +
                            "color: deeppink;" +
                            "text-align: center;" +
                        "}" +
                         "p {" +
                            "color: deeppink;" +
                            "text-align: center;" +
                        "}" +
                        "a {" +
                            "color: deeppink;" +
                            "text-align: center;" +
                            "display: block;" +
                        "}" +
                    "</style>";
        }
    }
}