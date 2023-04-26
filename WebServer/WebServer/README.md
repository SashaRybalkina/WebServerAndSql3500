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

This class demonstrates all the necessary functionality of a web server. It contains the methods
for handling Connecting/Disconnecting messages; methods for building various web pages such as
highscores, totaltime, etc. It also handles the GET requests and returns web pages accordingly.

For our extra features, we created a '/fancy' request. Upon entering the request, the browser returns
a web page showing a table containing all the players' names and their time survived.

The '/create' request creates six tables in the SQL database, which are AllGames, HighMass, HighRank,
HighScores, Time, and TotalTime. If the tables are successfully created, the browser returns a success
page, if the tables already exist, it will return an error page. The helper method for creating tables
and inserting data is in the DataBase class.

NOTE: 
Both of us are using MacOS; thus, we changed our all the '\r\n' to '\r', otherwise it will break our code.
For our program to run on other machines like Windows, you will have to change all the '\r' back to '\r\n'.

# Assignments Specific Topics:

SQL, SSMS, Database, Tables, User Secrets, HTTP, HTML, CSS.

# References:

    1. HTTP Responses: https://www.tutorialspoint.com/http/http_responses.htm 
    2. HTML5: https://www.sitepoint.com/a-basic-html5-template/
    3. CSS: https://www.w3schools.com/html/html_css.asp

# Time Expenditures:

    Total Predicted Hours:       12          Total Actual Hours:           12

    All the work were done through pair programming, thus the time expenditures for each partner
    are the same.

# Partnerships:

All the work were done through pair programming (side by side coding).

# Testing:

We tested the WebServer programm by connecting to our SQL server, opening the browser and trying to 
connect. We entered different GET requests to check if the web pages and data are displayed correctly. 
We also checked our database to see if the tables are successfully generated and data are inserted. 

# Branching:

We did all work for this project in the main branch.
