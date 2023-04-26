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
    ///     This class demonstrates all the necessary functionality of a web server.
    ///     Upon start, it waits for the browser to connect to port 11001. Then it
    ///     parses the HTTP request and handles the GET requests.
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
        /// Disconnects handler. Browser disconnects.
        /// </summary>
        /// <param name="channel"></param>
        internal static void OnDisconnect(Networking channel)
        {
            Debug.WriteLine($"Goodbye {channel.RemoteAddressPort}");
        }

        /// <summary>
        /// Create the HTTP response header, containing items such as
        /// the "HTTP/1.1 200 OK" line.       
        /// </summary>
        /// <param name="length"> how big a message are we sending</param>
        /// <param name="type"> usually html, but could be css</param>
        /// <returns>returns a string with the response header</returns>
        private static string BuildHTTPResponseHeader(int length, string type = "text/html")
        {
            return $"HTTP/1.1 200 OK\rDate: {DateTime.Now}\rContent-Length: {length}\rContent-Type: text/html\rConnection: Closed";
        }

        /// <summary>
        ///   Create a web page. The body of the returned message is the web page
        ///   "code" itself.
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
        /// Creates the PageNotFound error page. 
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
        /// Creates the Highscores page. Displaying a tables that shows the
        /// players and their scores.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPHighScoresPage()
        {
            Dictionary<string, string> players = DB.ReadPlayerScores();
            string AddToTable = "";

            // Adds the data from database to the table.
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
        /// Creates a page that displays a table containing scores of the input player.
        /// </summary>
        /// <param name="playerName">input player name</param>
        /// <returns></returns>
        private static string BuildHTTPScoresOfPlayer(string playerName)
        {
            Dictionary<string, List<List<string>>> ScoresAndGames = DB.ReadScoresOfPlayers();
            string AddToTable = "";

            // Adds the data of a specific player to the table
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
        /// Creates a page that displays the message when data is sucessfully inserted.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPScoresInsert()
        {
            return "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                        "<h1>Successfully inserted data :)</h1>" +
                    "</body>" +
                    "</html>";
        }

        /// <summary>
        /// Extra function. Creates a page that displays a table containing the
        /// total time of the players.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPTotalTime()
        {
            Dictionary<string, string> players = DB.ReadTimesSurvived();
            string AddToTable = "";
            foreach (string player in players.Keys)
            {
                AddToTable += $@"<tr><td>{player}</td><td>{players[player]}</td></tr>";
            }
            return "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                        "<h1>Total time that players lasted (in milliseconds)</h1>" +
                         $@"<table>" +
                            "<tr>" +
                            "<th>Player</th>" +
                            "<th>Time Survived</th>" +
                            "</tr>" +
                            AddToTable +
                        "</table>" +
                    "</body>" +
                    "</html>";
        }    

        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success or fail
        /// </summary>
        /// <returns> the HTTP response header followed by some informative information</returns>
        private static string CreateDBTablesPage()
        {
            string SuccessPage =
                "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                        SendCSSResponse() +
                    "</head>" +
                    "<body>" +
                        "<h1>Data Created Successfully :)</h1>" +
                    "</body>" +
                    "</html>";

            string FailPage =
                "<!DOCTYPE html>" +
                    "<html>" +
                    "<body>" +
                        "<h1>Error. Tables already exist.</h1>" +
                    "</body>" +
                    "</html>";

            // calling the helper method from the database class to create
            // six tables. If the tables are successfully created, returns
            // the success page; otherwise the fail page.
            if (DB.TablesCreated())
            {
                return SuccessPage;
            }
            else
            {
                return FailPage;
            }
        }

        /// <summary>
        /// Responsible for handling incoming messages from the client and
        /// determining the appropriate response.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        internal static void OnMessage(Networking channel, string message)
        {
            string body = "";
            string header = "";

            // Retreives the data from the message and inserts the data to the
            // SQL database.
            if (message.Contains("/scores/") && message.ToCharArray().Count(c => c == '/') == 7)
            {
                string[] splitMessage = message.Split("/");

                string name = splitMessage[2];
                string highmass = splitMessage[3];
                string highrank = splitMessage[4];
                string starttime = splitMessage[5];
                string endtime = splitMessage[6].Substring(0, splitMessage[6].Length-5);

                // builds the connection with the SQL server
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

                con.Open();

                using SqlCommand insertTotalTime = new SqlCommand($@"INSERT INTO TotalTime VALUES ('{name}', '{long.Parse(endtime) - long.Parse(starttime)}')", con);
                using SqlDataReader reader4 = insertTotalTime.ExecuteReader();

                con.Close();

                body = BuildHTTPScoresInsert();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }

            // handles the request and displays the page that shows the data
            // of the input player
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

            // shows the highscores page
            else if (message.Contains("/highscores"))
            {
                body = BuildHTTPHighScoresPage();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }

            // handles the request for fancy, shows the totaltime page
            else if (message.Contains("/fancy"))
            {
                body = BuildHTTPTotalTime();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }

            // checks if the incoming message is a GET request for
            // the root or index page of the web server
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

            // if the request contains '/favicon', sends the response
            // header and body to client.
            else if (message.Contains("/favicon"))
            {
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }

            // creates the tables in the database and displays
            // corresponding page.
            else if (message.Contains("/create"))
            {
                body = CreateDBTablesPage();
                header = BuildHTTPResponseHeader(body.Length);
                channel.Send(header);
                channel.Send("");
                channel.Send(body);
            }

            // if the request is not found, displays a PageNotFound
            // error page.
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