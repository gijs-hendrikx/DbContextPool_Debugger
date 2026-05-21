Run this application as a console application from Visual Studio.

## Program.cs
Sets up the db context pool with a limit of 20.

## ContextLeakService
Runs every 20 seconds.
Requests 25 db contexts at once, forcing the pool to create more contexts then the pool holds. Disposes them afterwards.

## MetricsLoggingService
Logs the active_dbcontexts metric and the available db contexts in the pool.
