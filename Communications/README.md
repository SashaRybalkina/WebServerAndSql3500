```
Author:     Aurora Zuo
Partner:    Sasha Rybalkina
Date:       14-Apr-2023
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Aurora1825 & crazyrussian123456
Repo:       https://github.com/uofu-cs3500-spring23/assignment-nine---web-server---sql-hungry_for_men
Date:       25-Apr-2023 Time (of when submission is ready to be evaluated)
Solution:   WebServer
Copyright:  CS 3500 and Aurora Zuo & Sasha Rybalkina - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

This program contains an abstract for a Networking object and "hides" the "gory details"
of creating connections. It interacts with a "client code" via a callback mechanism.
The code uses the TcpClient object and its various async method such as ReadAsync,
WriteAsync, AcceptTcpClientAsync, etc.

The Networking class also contains three delegates, which will be used in creating a
Networking object. The Networking object will be used in WebServer to form connection.

PS: We commented the logger AwaitMessageAsync out in the Connect method because they break
our code. We tried to debug and asked the TA for help but unfortunately we weren't 
able to get it to work. The provided Communications.dll just totally breaks our code 
and nothing is displayed in the GUI when using this, so we didn't use the provided dll.
The TA told us that it was more like a machine problem instead of our codes' problem. 
(We both are using MacOS).

Apr.25 UPDATES:
We were still unable to use the Communications.dll; thus, we copied the networking class 
from our previous assignment and continued to use it.

# Assignments Specific Topics:

NetworkStream, TCP, Socket, Dependency Injection, Async/Await, Logging, Delegates, protocols

# References:

    1. TCP overview: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes?source=recommendations
    2. Code from ForStudents : SimpleChatClientAndServer
    3. Our own code from A7

# Time Expenditures:

    Total Predicted Hours:       0.5           Total Actual Hours:           0.5

    All the work were done through pair programming, thus the time expenditures for each partner
    are the same.
    
    We used the same Networking class from our A7, so didn't spend much time work on it because 
    the class already works.


# Partnerships:

All the work were done through pair programming (side by side coding).

# Testing:

We tested the Networing programm by opening the server and the ClientGUI, trying to connect and see 
if the clients correctly sends and receives game objects and if the server sends and receives the 
messages correctly.

# Branching:

We did all work for this project in the main branch.
