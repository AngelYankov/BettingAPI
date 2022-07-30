using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models.Active;
using BettingAPI.Services.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace BettingAPI.Services
{
    public class BettingService : IBettingService
    {
        private readonly string connectionString;
        private readonly IDeserializeService deserializeService;

        public BettingService(IConfiguration configuration, IDeserializeService deserializeService)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
            this.deserializeService = deserializeService;
        }

        /// <summary>
        /// Saves all data from the XML
        /// </summary>
        public void Save()
        {
            var document = this.deserializeService.TransformXml();

            var allSports = MapSports(document);
            AddSports(allSports);
            
            var allEvents = MapEvents(document);
            AddEvents(allEvents);
            
            var allMatches = MapMatches(document);
            AddMatches(allMatches);
            
            var allBets = MapBets(document);
            AddBets(allBets);
            
            var allOdds = MapOdds(document);
            AddOdds(allOdds);
        }

        /// <summary>
        /// Maps all Sport nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Sport objects mapped from XML document</returns>
        private IEnumerable<SportDTO> MapSports(XmlDocument document)
        {
            var allSports = new List<Sport>();
            var sports = document.SelectNodes(Constants.SportNodes);
            for (int i = 0; i < sports.Count; i++)
            {
                var sport = new Sport()
                {
                    Id = Int32.Parse(sports[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    Name = sports[i].SelectSingleNode(Constants.NameAttribute).InnerText,
                    IsActive = true
                };

                allSports.Add(sport);
            }

            return allSports.Select(s => new SportDTO(s));
        }

        /// <summary>
        /// Maps all Event nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Event objects mapped from XML document</returns>
        private IEnumerable<EventDTO> MapEvents(XmlDocument document)
        {
            var allEvents = new List<Event>();
            var events = document.SelectNodes(Constants.EventNodes);
            for (int i = 0; i < events.Count; i++)
            {
                var eventHistory = new Event()
                {
                    Id = Int32.Parse(events[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    CategoryID = Int32.Parse(events[i].SelectSingleNode(Constants.CategoryAttribute).InnerText),
                    Name = events[i].SelectSingleNode(Constants.NameAttribute).InnerText,
                    IsLive = events[i].SelectSingleNode(Constants.IsLiveAttribute).InnerText == "true",
                    SportId = Int32.Parse(events[i].SelectSingleNode(Constants.AtChar + Constants.SportIdAttribute).InnerText),
                    IsActive = true
                };

                allEvents.Add(eventHistory);
            }

            return allEvents.Select(e => new EventDTO(e));
        }

        /// <summary>
        /// Maps all Match nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Match objects mapped from XML document</returns>
        private IEnumerable<MatchDTO> MapMatches(XmlDocument document)
        {
            var allMatches = new List<Match>();
            var matches = document.SelectNodes(Constants.MatchNodes);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = new Match()
                {
                    Id = Int32.Parse(matches[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    Name = matches[i].SelectSingleNode(Constants.NameAttribute).InnerText,
                    StartDate = DateTime.Parse(matches[i].SelectSingleNode(Constants.StartDateAttribute).InnerText),
                    MatchType = Enum.Parse<MatchType>(matches[i].SelectSingleNode(Constants.AtChar + Constants.MatchTypeAttribute).InnerText),
                    EventId = Int32.Parse(matches[i].SelectSingleNode(Constants.AtChar + Constants.EventIdAttribute).InnerText),
                    IsActive = true
                };
                allMatches.Add(match);
            }

            return allMatches.Select(m => new MatchDTO(m));
        }

        /// <summary>
        /// Maps all Bet nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Bet objects mapped from XML document</returns>
        private IEnumerable<BetDTO> MapBets(XmlDocument document)
        {
            var allBets = new List<Bet>();
            var bets = document.SelectNodes(Constants.BetNodes);
            for (int i = 0; i < bets.Count; i++)
            {
                var bet = new Bet()
                {
                    Id = Int32.Parse(bets[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    Name = bets[i].SelectSingleNode(Constants.NameAttribute).InnerText,
                    IsLive = bets[i].SelectSingleNode(Constants.IsLiveAttribute).InnerText == "true",
                    MatchId = Int32.Parse(bets[i].SelectSingleNode(Constants.AtChar + Constants.MatchIdAttribute).InnerText),
                    MatchType = Enum.Parse<MatchType>(bets[i].SelectSingleNode(Constants.AtChar + Constants.MatchTypeAttribute).InnerText),
                    MatchStartDate = DateTime.Parse(bets[i].SelectSingleNode(Constants.AtChar + Constants.MatchStartDateAttribute).InnerText),
                    IsActive = true
                };

                allBets.Add(bet);
            }

            return allBets.Select(b => new BetDTO(b));
        }

        /// <summary>
        /// Maps all Odd nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Odd objects mapped from XML document</returns>
        private IEnumerable<OddDTO> MapOdds(XmlDocument document)
        {
            var allOdds = new List<Odd>();
            var odds = document.SelectNodes(Constants.OddNodes);
            for (int i = 0; i < odds.Count; i++)
            {
                var odd = new Odd()
                {
                    Id = Int32.Parse(odds[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    Name = odds[i].SelectSingleNode(Constants.NameAttribute).InnerText,
                    Value = Convert.ToDecimal(odds[i].SelectSingleNode(Constants.ValueAttribute).InnerText),
                    SpecialValueBet = odds[i].SelectSingleNode(Constants.SpecialBetValueAttribute)?.InnerText,
                    BetId = Int32.Parse(odds[i].SelectSingleNode(Constants.AtChar + Constants.BetIdAttribute).InnerText),
                    IsActive = true
                };

                allOdds.Add(odd);
            }

            return allOdds.Select(o => new OddDTO(o));
        }

        /// <summary>
        /// Adds, Updates and Deletes Sport objects from Sport database table according to current XML document
        /// </summary>
        /// <param name="allSports">All Sport objects from current XML document</param>
        private void AddSports(IEnumerable<SportDTO> allSports)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = connectionString })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpSportsTable(
                            [Id] [INT],
                            [Name] [TEXT],
                            [IsActive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpSportsTable";
                            bulkCopy.WriteToServer(allSports.ToDataTable());
                        }

                        command.CommandText = @"
                            MERGE INTO dbo.Sports AS TARGET
                            USING dbo.#TmpSportsTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id
                            WHEN MATCHED THEN
                                UPDATE SET TARGET.Name = SOURCE.Name
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, IsActive)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsActive)
                            WHEN NOT MATCHED BY SOURCE THEN
                                UPDATE SET TARGET.IsActive = 0;
                            DROP TABLE #TmpSportsTable";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds, Updates and Deletes Event objects from Event database table according to current XML document
        /// </summary>
        /// <param name="allEvents">All Event objects from current XML document</param>
        private void AddEvents(IEnumerable<EventDTO> allEvents)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = connectionString })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpEventTable(
                            [Id] [INT],
                            [Name] [TEXT],
                            [IsLive] [BIT],
                            [CategoryId] [INT],
                            [SportId] [INT],
                            [IsActive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpEventTable";
                            bulkCopy.WriteToServer(allEvents.ToDataTable());
                        }

                        command.CommandText = @"
                            MERGE INTO dbo.Events AS TARGET
                            USING dbo.#TmpEventTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id
                            WHEN MATCHED THEN
                                UPDATE SET TARGET.IsLive = SOURCE.IsLive
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, IsLive, CategoryId, SportId, IsActive)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.CategoryId, SOURCE.SportId, SOURCE.IsActive)
                            WHEN NOT MATCHED BY SOURCE THEN
                                UPDATE SET TARGET.IsActive = 0;
                            DROP TABLE #TmpEventTable";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds, Updates and Deletes Match objects from Match database table according to current XML document
        /// Adds new MatchChangeLog objects to MatchChangeLogs database table when records are updated or deleted in Match database table
        /// </summary>
        /// <param name="allMatches">All Match objects from current XML document</param>
        private void AddMatches(IEnumerable<MatchDTO> allMatches)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = connectionString })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpMatchTable(
                            [Id] [INT],
                            [Name] [TEXT],
                            [StartDate] [DATETIME],
                            [MatchType] [INT],
                            [EventId] [INT],
                            [IsActive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpMatchTable";
                            bulkCopy.WriteToServer(allMatches.ToDataTable());
                        }

                        command.CommandText = @"
                            INSERT INTO dbo.MatchChangeLogs
                                SELECT 
                                    MatchXmlId,
                                    Name,
                                    StartDate,
                                    MatchType,
                                    EventId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Matches AS TARGET
                                    USING dbo.#TmpMatchTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND (
                                       TARGET.MatchType != SOURCE.MatchType OR
                                       TARGET.StartDate != SOURCE.StartDate )
                                    WHEN MATCHED THEN
                                        UPDATE SET TARGET.MatchType = SOURCE.MatchType,
                                                   TARGET.StartDate = SOURCE.StartDate
                                    OUTPUT $action as ActionType, INSERTED.Id as MatchXmlId, INSERTED.Name, INSERTED.StartDate, INSERTED.MatchType, INSERTED.EventId, GETDATE() as DateCreated, 'UPDATE' as ActionToTake
                                ) AllChanges (ActionType, MatchXmlId, Name, StartDate, MatchType, EventId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.MatchChangeLogs
                                SELECT 
                                    MatchXmlId,
                                    Name,
                                    StartDate,
                                    MatchType,
                                    EventId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Matches AS TARGET
                                    USING dbo.#TmpMatchTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id 
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, StartDate, MatchType, EventId, IsActive)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.StartDate, SOURCE.MatchType, SOURCE.EventId, SOURCE.IsActive)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                         UPDATE SET TARGET.IsActive = 0
                                    OUTPUT $action as ActionType, INSERTED.Id as MatchXmlId, INSERTED.Name, INSERTED.StartDate, INSERTED.MatchType, INSERTED.EventId, GETDATE() as DateCreated, 'DELETE' as ActionToTake
                                ) AllChanges (ActionType, MatchXmlId, Name, StartDate, MatchType, EventId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'
                                DROP TABLE #TmpMatchTable";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds, Updates and Deletes Bet objects from Bet database table according to current XML document
        /// Adds new BetChangeLog objects to BetChangeLogs database table when records are updated or deleted in Bet database table
        /// </summary>
        /// <param name="allBets">All Bet objects from current XML document</param>
        private void AddBets(IEnumerable<BetDTO> allBets)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = connectionString })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpBetTable(
                            [Id] [INT],
                            [IsLive] [BIT],
                            [MatchId] [INT],
                            [Name] [TEXT],
                            [MatchType] [INT],
                            [MatchStartDate] [DATETIME],
                            [IsActive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpBetTable";
                            bulkCopy.WriteToServer(allBets.ToDataTable());
                        }

                        command.CommandText = @"
                            INSERT INTO dbo.BetChangeLogs
                                SELECT 
                                    BetXmlId,
                                    Name,
                                    IsLive,
                                    MatchId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Bets AS TARGET
                                    USING dbo.#TmpBetTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.IsLive != SOURCE.IsLive
                                    WHEN MATCHED THEN
                                        UPDATE SET TARGET.IsLive = SOURCE.IsLive
                                    OUTPUT $action as ActionType, INSERTED.Id as BetXmlId, INSERTED.Name, INSERTED.IsLive, INSERTED.MatchId, GETDATE() as DateCreated, 'UPDATE' as ActionToTake
                                ) AllChanges (ActionType, BetXmlId, Name, IsLive, MatchId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.BetChangeLogs
                                SELECT 
                                    BetXmlId,
                                    Name,
                                    IsLive,
                                    MatchId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Bets AS TARGET
                                    USING dbo.#TmpBetTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id 
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, IsLive, MatchId, MatchStartDate, MatchType, IsActive)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.MatchId, SOURCE.MatchStartDate, SOURCE.MatchType, SOURCE.IsActive)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                         UPDATE SET TARGET.IsActive = 0
                                    OUTPUT $action as ActionType, INSERTED.Id as BetXmlId, INSERTED.Name, INSERTED.IsLive, INSERTED.MatchId, GETDATE() as DateCreated, 'DELETE' as ActionToTake
                                ) AllChanges (ActionType, BetXmlId, Name, IsLive, MatchId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'
                                DROP TABLE #TmpBetTable";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds, Updates and Deletes Odd objects from Odd database table according to current XML document
        /// Adds new OddChangeLog objects to OddChangeLogs database table when records are updated or deleted in Odd database table
        /// </summary>
        /// <param name="allOdds">All Odd objects from current XML document</param>
        private void AddOdds(IEnumerable<OddDTO> allOdds)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = connectionString })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpOddTable(
                            [Id] [INT],
                            [Name] [TEXT],
                            [VALUE] [DECIMAL],
                            [BetId] [INT],
                            [SpecialValueBet] [TEXT] NULL,
                            [IsActive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpOddTable";
                            bulkCopy.WriteToServer(allOdds.ToDataTable());
                        }

                        command.CommandText = @"
                            INSERT INTO dbo.OddChangeLogs
                                SELECT 
                                    OddXmlId,
                                    Name,
                                    Value,
                                    SpecialValueBet,
                                    BetId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Odds AS TARGET
                                    USING dbo.#TmpOddTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.Value != SOURCE.Value 
                                    WHEN MATCHED THEN
                                        UPDATE SET TARGET.Value = SOURCE.Value
                                    OUTPUT $action as ActionType, INSERTED.Id as OddXmlId, INSERTED.Name, INSERTED.Value, INSERTED.SpecialValueBet, INSERTED.BetId, GETDATE() as DateCreated, 'UPDATE' as ActionToTake
                                ) AllChanges (ActionType, OddXmlId, Name, Value, SpecialValueBet, BetId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.OddChangeLogs
                                SELECT 
                                    OddXmlId,
                                    Name,
                                    Value,
                                    SpecialValueBet,
                                    BetId,
                                    ActionToTake,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Odds AS TARGET
                                    USING dbo.#TmpOddTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, Value, SpecialValueBet, BetId, IsActive)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.Value, SOURCE.SpecialValueBet, SOURCE.BetId, SOURCE.IsActive)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                         UPDATE SET TARGET.IsActive = 0
                                    OUTPUT $action as ActionType, INSERTED.Id as OddXmlId, INSERTED.Name, INSERTED.Value, INSERTED.SpecialValueBet, INSERTED.BetId, GETDATE() as DateCreated, 'DELETE' as ActionToTake
                                ) AllChanges (ActionType, OddXmlId, Name, Value, SpecialValueBet, BetId, DateCreated, ActionToTake)
                                WHERE AllChanges.ActionType = 'UPDATE'
                                DROP TABLE #TmpOddTable";

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
