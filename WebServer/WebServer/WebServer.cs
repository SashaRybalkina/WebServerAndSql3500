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
        /// 
        /// </summary>
        /// <param name="playerName"></param>
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
        /// 
        /// </summary>
        /// <param name="playerName"></param>
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
        /// Helper method that creates the tables and returns true if the tables were created
        /// successfully. If the tables already exist, returns false.
        /// </summary>
        /// <returns></returns>
        private static bool TablesCreated()
        {

            var builder = new ConfigurationBuilder();

            string[] players = { "Jessica", "Sasha", "Aurora", "TheManOfMyDreams", "CatLover56" };

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

            try
            {
                con.Open();
                using SqlCommand CreateAllGames = new SqlCommand("CREATE TABLE AllGames (Player varchar(50), Score varchar(50), GameID varchar(50))", con);
                using SqlDataReader reader1 = CreateAllGames.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand CreateHighMass = new SqlCommand("CREATE TABLE HighMass (Player varchar(50), Mass varchar(50))", con);
                using SqlDataReader reader2 = CreateHighMass.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand CreateHighRank = new SqlCommand("CREATE TABLE HighRank (Player varchar(50), Rank varchar(50))", con);
                using SqlDataReader reader3 = CreateHighRank.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand CreateHighScores = new SqlCommand("CREATE TABLE HighScores (Player varchar(50), Score varchar(50))", con);
                using SqlDataReader reader4 = CreateHighScores.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand CreateTime = new SqlCommand("CREATE TABLE Time (Player varchar(50), StartTime varchar(50), EndTime varchar(50))", con);
                using SqlDataReader reader5 = CreateTime.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand CreateTotalTime = new SqlCommand("CREATE TABLE TotalTime (Player varchar(50), Time varchar(50))", con);
                using SqlDataReader reader6 = CreateTotalTime.ExecuteReader();
                con.Close();
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (string player in players)
            {
                Random random = new Random();
                string GameID = random.Next(0, 50).ToString();
                string Mass = random.Next(50, 5000).ToString();
                string Rank = random.Next(1, 100).ToString();
                string Score = random.Next(0, 5000).ToString();
                int StartTime = random.Next(100000, 1000000);
                int EndTime = random.Next(100000000, 1000000000);
                string TotalTime = (EndTime - StartTime).ToString();

                con.Open();
                using SqlCommand InsertAllGames = new SqlCommand($@"INSERT INTO AllGames VALUES ('{player}', '{Score}', '{GameID}')", con);
                using SqlDataReader reader01 = InsertAllGames.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand InsertHighMass = new SqlCommand($@"INSERT INTO HighMass VALUES ('{player}', '{Mass}')", con);
                using SqlDataReader reader02 = InsertHighMass.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand InsertHighRank = new SqlCommand($@"INSERT INTO HighRank VALUES ('{player}', '{Rank}')", con);
                using SqlDataReader reader03 = InsertHighRank.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand InsertHighScores = new SqlCommand($@"INSERT INTO HighScores VALUES ('{player}', '{Score}')", con);
                using SqlDataReader reader04 = InsertHighScores.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand InsertTime = new SqlCommand($@"INSERT INTO Time VALUES ('{player}', '{StartTime.ToString()}', '{EndTime.ToString()}')", con);
                using SqlDataReader reader05 = InsertTime.ExecuteReader();
                con.Close();

                con.Open();
                using SqlCommand InsertTotalTime = new SqlCommand($@"INSERT INTO TotalTime VALUES ('{player}', '{TotalTime}')", con);
                using SqlDataReader reader06 = InsertTotalTime.ExecuteReader();
                con.Close();
            }
            return true;
        }

        /// <summary>
        ///    (1) Instruct the DB to seed itself (build tables, add data)
        ///    (2) Report to the web browser on the success
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

            if (TablesCreated())
            {
                return SuccessPage;
            }
            else
            {
                return FailPage;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
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

            else if (message.Contains("/fancy"))
            {
                body = BuildHTTPTotalTime();
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