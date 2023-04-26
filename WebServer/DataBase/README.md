```
Author:     Aurora Zuo
Partner:    Sasha Rybalkina
Date:       25-Apr-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Aurora1825 & crazyrussian123456
Repo:       https://github.com/uofu-cs3500-spring23/assignment-nine---web-server---sql-hungry_for_men
Date:       26-Apr-2023 Time (of when submission is ready to be evaluated)
Solution:   WebServer
Copyright:  CS 3500 and Aurora Zuo & Sasha Rybalkina - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

This program demonstrates all the necessary functionalities of a database. It connects to the SQL database 
using the user secrets and retreives the data from the tables such as high scores, scores, times survived.
It contains helper method for inserting the necessary data into the tables according to the scores followed
by five slashes request. It also contains the helper method for creating six tables and inserting data into them
using SSMS.

Tables: 
We created six tables, which are AllGames, HighMass, HighRank, HighScores, Time, and TotalTime.
The AllGames contains the players' names, their highest scores, and IDs. The HighMass records the
largest masses; HighRank stores the players' highest ranks; HighScores stores the players' highest
scores. Time stores the players' game start time and end time in milliseconds. TotalTime stores
the players' total time survived in the game by subtracting the start time from the end time.

NOTE: 
Both of us are using MacOS; thus, we changed our all the '\r\n' to '\r', otherwise it will break our code.
For our program to run on other machines like Windows, you will have to change all the '\r' back to '\r\n'.

# Assignments Specific Topics:

SQL, SSMS, Database, Tables, User Secrets.

# References:

    1. Code from Lab14

# Time Expenditures:

    Total Predicted Hours:       2           Total Actual Hours:           2

    All the work were done through pair programming, thus the time expenditures for each partner
    are the same.

# Partnerships:

All the work were done through pair programming (side by side coding).

# Testing:

We tested the DataBase programm by connecting our SQL server, opening the browser and trying to 
connect to our web server. Check to see if the tables are correctly created and data are correctly 
displayed and inserted.

# Branching:

We did all work for this project in the main branch.
