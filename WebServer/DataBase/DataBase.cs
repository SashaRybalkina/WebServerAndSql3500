namespace AS9;

using System.Numerics;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Communications;
using System.Collections.Generic;

/// <summary>
/// Author:  H. James de St. Germain
/// Date:    Spring 2020
/// Updated: Spring 2022
///          Spring 2023
/// 
/// Coding examples for connecting to and querying an SQL Database
/// 
/// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows
/// 
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

    public void AddPlayers()
    {
        Console.WriteLine("Can we add a row?");

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("INSERT INTO Players VALUES ('Jessica', 6)", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("{0} {1}",
                    reader.GetInt32(0), reader.GetString(1));
            }
            con.Close();
        }
        
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }
    }

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
}