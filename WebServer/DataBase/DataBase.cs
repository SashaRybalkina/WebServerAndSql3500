namespace AS9;

using System.Numerics;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Communications;
using System.Collections.Generic;
using System.Collections;

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
///     This class demonstrates all the necessary functionality of a database.
///     It connects to the SQL database using the user secrets and retreives the
///     data from the tables such as highscores, scores, time survived. It also
///     contains the helper method for creating six tables and inserting data into them.
/// </summary>
public class DataBase
{
    /// <summary>
    /// The information necessary for the program to connect to the Database
    /// </summary>
    public static readonly string connectionString;

    /// <summary>
    /// Upon construction of this static class, build the connection string
    /// </summary>
    static DataBase()
    {
        var builder = new ConfigurationBuilder();

        builder.AddUserSecrets<DataBase>();
        IConfigurationRoot Configuration = builder.Build();
        var SelectedSecrets = Configuration.GetSection("DataSecrets");

        connectionString = new SqlConnectionStringBuilder()
        {
            DataSource      = SelectedSecrets["DataSource"],
            InitialCatalog  = SelectedSecrets["InitialCatalog"],
            UserID          = SelectedSecrets["UserID"],
            Password        = SelectedSecrets["Password"],
            Encrypt         = false
        }.ConnectionString;
    }

    /// <summary>
    /// Reads the player names and scores from the "HighScores" table
    /// in the database and returns a dictionary of the results. Prints the
    /// player names and scores from the database.
    /// </summary>
    /// <returns>empty dictionary is returned if error occurs during connection</returns>
    public Dictionary<string, string> ReadPlayerScores()
    {
        Dictionary<string, string> scores = new();

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("SELECT Player, Score FROM HighScores", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                scores.Add(reader["Player"].ToString(), reader["Score"].ToString());
                Console.WriteLine($"{reader["Player"]} - {reader["Score"]}");
            }
        }

        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        return scores;
    }

    /// <summary>
    /// Reads and retrieves the times survived data for all players from the database.
    /// </summary>
    /// <returns>a dictionary of players and their times survived</returns>
    public Dictionary<string, string> ReadTimesSurvived()
    {
        Dictionary<string, string> times = new();

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("SELECT Player, Time FROM TotalTime", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                times.Add(reader["Player"].ToString(), reader["Time"].ToString());
                Console.WriteLine($"{reader["Player"]} - {reader["Time"]}");
            }
        }

        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        return times;
    }

    /// <summary>
    /// Reads the scores of players from the database and returns them as a dictionary.
    /// </summary>
    /// <returns>a dictionary of player scores.</returns>
    public Dictionary<string, List<List<string>>> ReadScoresOfPlayers()
    {
        Dictionary<string, List<List<string>>> scores = new();

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("SELECT Player, Score, GameID FROM AllGames", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string ?player = reader["Player"].ToString();
                string ?score = reader["Score"].ToString();
                string ?gameID = reader["GameID"].ToString();

                List<string> ScoreAndGame = new();
                ScoreAndGame.Add(score);
                ScoreAndGame.Add(gameID);

                if (scores.ContainsKey(player))
                {
                    scores[player].Add(ScoreAndGame);
                }
                else
                {
                    List<List<string>> ScoresAndGames = new();
                    ScoresAndGames.Add(ScoreAndGame);
                    scores.Add(player, ScoresAndGames);
                } 
            }
        }

        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        return scores;
    }

    /// <summary>
    /// Helper method that creates the tables and returns true if the tables were created
    /// successfully. If the tables already exist, returns false.
    /// </summary>
    /// <returns></returns>
    public bool TablesCreated()
    {
        // Connects to the database
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

        // creates the tables
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

        // inserts randomized data into the tables
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
}