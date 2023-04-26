```
Author:     Aurora Zuo
Partner:    Sasha Rybalkina
Date:       25-Apr-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Aurora1825
Repo:       https://github.com/uofu-cs3500-spring23/assignment-nine---web-server---sql-hungry_for_men
Date:       26-Apr-2023 Time (of when submission is ready to be evaluated)
Solution:   WebServer
Copyright:  CS 3500 and Aurora Zuo & Sasha Rybalkina - This work may not be copied for use in Academic Coursework.
```

# Overview of the WebServer

Database Table:
We created six tables, which are AllGames, HighMass, HighRank, HighScores, Time, and TotalTime.
The AllGames contains the players' names, their highest scores, and IDs. The HighMass records the
largest masses; HighRank stores the players' highest ranks; HighScores stores the players' highest
scores. Time stores the players' game start time and end time in milliseconds. TotalTime stores
the players' total time survived in the game by subtracting the start time from the end time.

DataBase Program:
The DataBase program demonstrates all the necessary functionalities of a database. It connects to
the SQL database using the user secrets. It updates and retreives the data from the tables such as
high scores, scores, times survived. It contains helper method for inserting the necessary data into
the tables according to the scores followed by five slashes request. It also contains the helper
method for creating six tables and inserting data into them using SSMS.

WebServer Program: 
The WebServer project will allow everyone to use their web browser to see information about how players 
are doing when playing Agario on our client. Upon start, web server allows the user to connect to the
browser and port 11001. It returns a Welcome page containing some useful web page links for the user
to navigate. 

It contains the methods for handling Connecting/Disconnecting messages; methods for building various
web pages such as highscores, totaltime, etc. It also handles the GET requests and returns web pages
accordingly. For example, when the user enters '/highscores', the browser will return a webpage containing
the highscores data in a table send from the HighScores table in the SQL database. 

For our extra features, we created a '/fancy' request. Upon entering the request, the browser returns
a web page showing a table containing all the players' names and their time survived.

The '/create' request creates six tables in the SQL database, which are AllGames, HighMass, HighRank,
HighScores, Time, and TotalTime. If the tables are successfully created, the browser returns a success
page, if the tables already exist, it will return an error page. The helper method for creating tables
and inserting data is in the DataBase class.

Client:
Here's our repo for Agario client: 
https://github.com/uofu-cs3500-spring23/assignment8agario-dominators_of_worlds

Note: 
Both of us are using MacOS; thus, we changed our all the '\r\n' to '\r', otherwise it will break our code.
For our program to run on other machines such as Windows, you will have to change all the '\r' to '\r\n' 
accordingly.

# Extent of work:

Our WebServer project has the ability of:

1. Connects to the port (11001) and the browser, returning a basic Welcome page upon start with welcome
message and some useful links.
2. Returns a web page with a chart of highscores.
3. Takes in request to insert highscores into the table in the database.
4. Returns a web page with a chart of scores of a specific player. The request can be like '/scores/Jessica'.
5. Returns a web page with a chart of time survived for all players. The request is '/fancy'.
6. The Agario client can contact the database and store information therein.

# Time Expenditures:

    Total Predicted Hours:          16           Total Actual Hours:         15
    Auroroa Total Predicted Hours:  16           Total Actual Hours:         15
    Sasha Total Predicted Hours:    16           Total Actual Hours:         15

    Time spent effectively: ~11 hours            Time spent debugging: ~2 hours
    Time spent learning tools and techniques: ~2 hours

    All the work were done through side by side pair programming, thus the time expenditures
    for each person are the same.

# Partnership

All the work were done through side by side pair programming, we constantly switched positions
as driver/navigator, we communicated a lot to make sure that each of us were on the same track.
Details for pair programming practice for each project can be found in the project level READMEs.


# Branching

All the work were done through the main branch. Since the partners worked side by side on the
whole project.


# Testing

For testing, we opened the SQL server and ran the webserver to open the browser. We tested the simple 
functionalities such as opening localhost:11001, sending random request see if the PageNotFound page 
was returned or not. Accessing highscores page, scores of specific players, and our fancy page to 
test if the required tables were displayed correctly and information was showing up successfully or not.

The details of testing for each project can be found in the project level READMEs.
