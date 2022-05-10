# BettingAPI
Application that retrieves betting data from an online XML source and saves it to a local database. The data is refreshed every 60 seconds to show active Events, Matches and their Bets and Odds.
## The Web API consists of two endpoints.
* **First endpoint** - BettingAPI/matches - Retrieve all matches starting in the next 24 hours along with all their active Bets and Odds if they have such
* **Second endpoint** - BettingAPI/matches/{matchXmlId} - Retrieve a specific match according to given Id along with all of its active and past Bets and Odds
 
## Technologies
* ASP.NET Core
* Entity Framework Core
* MS SQL Server

## Notes
* Before starting the application execute the BettingDbScript.sql file found in the repository to create the needed database on your local machine

## Database Diagram
![Alt text](https://github.com/AngelYankov/BettingAPI/-/raw/main/DbDiagram.png)
