using AS9;

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
///     This class contains the main method of the database program.
/// </summary>
class Program
{
    private static DataBase DB;

    /// <summary>
    /// Initializes the database. 
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        DB = new DataBase();
    }
}