namespace WebServer;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
            DataSource = "S2023_u1362376,14330",
            InitialCatalog = "cs3500",
            UserID = "S2023_u1362376",
            Password = SelectedSecrets["Password"],
            Encrypt = false
        }.ConnectionString;
    }

    /// <summary>
    ///  Test several connections and print the output to the console
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        Console.WriteLine(connectionString);
        Console.WriteLine("\n---------- Read All Patrons ---------------");
        AllPatrons();

        Console.WriteLine("\n---------- Add Patrons ---------------");
        AddPatrons();

        Console.WriteLine("\n---------- Read All Phone Numbers ---------------");
        AllPhones();

        Console.WriteLine("\n---------- JOIN Patrons and Phone Numbers ---------------");
        PatronsPhones();
    }


    /// <summary>
    /// Open a connection to the SQL server and query for all patrons.
    /// 
    /// Important points in code below:
    /// (1) creation of the ConnectionString and opening of the connection
    /// (2) the use of the using statements
    /// (3) how to write a direct SQL query and send it to the server
    /// (4) how to retrieve the data
    /// </summary>
    static void AllPatrons()
    {
        Console.WriteLine("Getting Connection ...");

        try
        {
            //create instance of database connection
            using SqlConnection con = new(connectionString);

            //
            // Open the SqlConnection.
            //
            con.Open();

            //
            // This code uses an SqlCommand based on the SqlConnection.
            //
            using SqlCommand command = new SqlCommand("SELECT * FROM Patrons", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("{0} {1}",
                    reader.GetInt32(0), reader.GetString(1));
            }

            Console.WriteLine($"Successful SQL connection");
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection: {exception.Message}");
        }
    }

    /// <summary>
    /// Try to add a row to the database table
    /// Note:
    ///   (1) Fails because the user does not have permission to do so!
    /// </summary>
    static void AddPatrons()
    {
        Console.WriteLine("Can we add a row?");

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("INSERT INTO Patrons VALUES (6,'Dav')", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("{0} {1}",
                    reader.GetInt32(0), reader.GetString(1));
            }
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }
    }

    /// <summary>
    /// Query all Phone numbers in table Phones
    /// 
    /// Notice:
    /// 
    /// (1) use of "dictionary" access
    /// (2) Select [named columns] syntax 
    /// </summary>
    public static void AllPhones()
    {
        Console.WriteLine("What phone numbers exist?");

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("SELECT CardNum, Phone FROM Phones", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["CardNum"]} - {reader["Phone"]}");
            }
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }
    }

    /// <summary>
    ///  JOIN the Phone number table with the Patrons table 
    ///  print all the phone numbers associated with each patron
    ///  
    ///  Notice: Explicit JOIN
    /// </summary>
    public static void PatronsPhones()
    {
        Console.WriteLine("What phone numbers exist?");

        try
        {
            using SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            using SqlCommand command = new SqlCommand("SELECT * FROM Phones JOIN Patrons ON Phones.CardNum = PAtrons.CardNum", con);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                {
                    Console.WriteLine($"{reader["Name"].ToString()?.Trim()} ({reader["CardNum"]}) - {reader["Phone"]}");
                }
            }
        }
        catch (SqlException exception)
        {
            Console.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }
    }
}