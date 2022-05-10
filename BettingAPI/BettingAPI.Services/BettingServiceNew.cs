using BettingAPI.DataContext;
using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models.History;
using BettingAPI.Services.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml;

namespace BettingAPI.Services
{
    public class BettingServiceNew : IBettingServiceNew
    {
        private const string SportNodes = "//Sport";
        private const string EventNodes = "//Event";
        private const string MatchNodes = "//Match";
        private const string BetNodes = "//Bet";
        private const string OddNodes = "//Odd";

        private const string IdAttribute = "@ID";
        private const string NameAttribute = "@Name";
        private const string CategoryAttribute = "@CategoryID";
        private const string IsLiveAttribute = "@IsLive";
        private const string StartDateAttribute = "@StartDate";
        private const string ValueAttribute = "@Value";
        private const string SpecialBetValueAttribute = "@SpecialBetValue";
        private const string SportIdAttribute = "SportId";
        private const string EventIdAttribute = "EventId";
        private const string MatchIdAttribute = "MatchId";
        private const string BetIdAttribute = "BetId";
        private const string MatchTypeAttribute = "MatchType";
        private const string MatchStartDateAttribute = "MatchStartDate";

        private readonly string connectionString;

        public BettingServiceNew(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Saves all data from the XML
        /// </summary>
        public void Save()
        {
            var document = TransformXml();

            var allSports = MapSports(document);
            AddSportHistories(allSports);

            var allEvents = MapEvents(document);
            AddEventHistories(allEvents);

            var allMatches = MapMatches(document);
            AddMatchHistories(allMatches);

            var allBets = MapBets(document);
            AddBetHistories(allBets);

            var allOdds = MapOdds(document);
            AddOddHistories(allOdds);

            AddSports(allSports);
            AddEvents(allEvents);
            AddMatches(allMatches);
            AddBets(allBets);
            AddOdds(allOdds);
        }

        /// <summary>
        /// Adds attributes with data need to XML
        /// </summary>
        /// <returns>New XML with added data</returns>
        private XmlDocument TransformXml()
        {
            var document = LoadFile();

            var sports = document.SelectNodes(SportNodes);

            for (int m = 0; m < sports.Count; m++)
            {
                var sportEntity = new SportHistory()
                {
                    Id = Int32.Parse(sports[m].SelectSingleNode(IdAttribute).InnerText)
                };

                var events = sports[m].ChildNodes;

                for (int i = 0; i < events.Count; i++)
                {
                    var sportIdAttribute = document.CreateAttribute(SportIdAttribute);
                    sportIdAttribute.Value = sportEntity.Id.ToString();
                    events[i].Attributes.Append(sportIdAttribute);

                    var eventEntity = new EventHistory()
                    {
                        Id = Int32.Parse(events[i].SelectSingleNode(IdAttribute).InnerText),
                    };

                    var matches = events[i].ChildNodes;

                    for (int j = 0; j < matches.Count; j++)
                    {
                        var eventIdAttribute = document.CreateAttribute(EventIdAttribute);
                        eventIdAttribute.Value = eventEntity.Id.ToString();
                        matches[j].Attributes.Append(eventIdAttribute);

                        var matchEntity = new MatchHistory()
                        {
                            Id = Int32.Parse(matches[j].SelectSingleNode(IdAttribute).InnerText),
                            MatchType = Enum.Parse<MatchType>(matches[j].SelectSingleNode("@" + MatchTypeAttribute).InnerText),
                            StartDate = DateTime.Parse(matches[j].SelectSingleNode(StartDateAttribute).InnerText)
                        };

                        var bets = matches[j].ChildNodes;

                        for (int k = 0; k < bets.Count; k++)
                        {
                            var attributeMatchId = document.CreateAttribute(MatchIdAttribute);
                            attributeMatchId.Value = matchEntity.Id.ToString();
                            bets[k].Attributes.Append(attributeMatchId);

                            var attributeMatchType = document.CreateAttribute(MatchTypeAttribute);
                            attributeMatchType.Value = matchEntity.MatchType.ToString();
                            bets[k].Attributes.Append(attributeMatchType);

                            var attributeMatchStartDate = document.CreateAttribute(MatchStartDateAttribute);
                            attributeMatchStartDate.Value = matchEntity.StartDate.ToString();
                            bets[k].Attributes.Append(attributeMatchStartDate);

                            var betEntity = new BetHistory()
                            {
                                Id = Int32.Parse(bets[k].SelectSingleNode(IdAttribute).InnerText),
                            };

                            var odds = bets[k].ChildNodes;

                            for (int l = 0; l < odds.Count; l++)
                            {
                                var attributeBetId = document.CreateAttribute(BetIdAttribute);
                                attributeBetId.Value = betEntity.Id.ToString();
                                odds[l].Attributes.Append(attributeBetId);
                            }
                        }
                    }
                }
            }

            return document;
        }

        /// <summary>
        /// Loads XML data from source
        /// </summary>
        /// <returns>Loaded XML data</returns>
        private XmlDocument LoadFile()
        {
            XmlDocument doc = new XmlDocument();
            string url = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

            using (var client = new WebClient())
            {
                string result = client.DownloadString(url);
                doc.LoadXml(result);
            }

            return doc;
        }

        /// <summary>
        /// Maps all Sport nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Sport objects mapped from XML document</returns>
        private IEnumerable<SportHistoryDTO> MapSports(XmlDocument document)
        {
            var allSports = new List<SportHistory>();
            var sports = document.SelectNodes(SportNodes);
            for (int i = 0; i < sports.Count; i++)
            {
                var sport = new SportHistory()
                {
                    Id = Int32.Parse(sports[i].SelectSingleNode(IdAttribute).InnerText),
                    Name = sports[i].SelectSingleNode(NameAttribute).InnerText,
                };

                allSports.Add(sport);
            }

            return allSports.Select(s => new SportHistoryDTO(s));
        }

        /// <summary>
        /// Maps all Event nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Event objects mapped from XML document</returns>
        private IEnumerable<EventHistoryDTO> MapEvents(XmlDocument document)
        {
            var allEvents = new List<EventHistory>();
            var events = document.SelectNodes(EventNodes);
            for (int i = 0; i < events.Count; i++)
            {
                var eventHistory = new EventHistory()
                {
                    Id = Int32.Parse(events[i].SelectSingleNode(IdAttribute).InnerText),
                    CategoryID = Int32.Parse(events[i].SelectSingleNode(CategoryAttribute).InnerText),
                    Name = events[i].SelectSingleNode(NameAttribute).InnerText,
                    IsLive = events[i].SelectSingleNode(IsLiveAttribute).InnerText == "true",
                    SportHistoryId = Int32.Parse(events[i].SelectSingleNode("@" + SportIdAttribute).InnerText),
                };

                allEvents.Add(eventHistory);
            }

            return allEvents.Select(e => new EventHistoryDTO(e));
        }

        /// <summary>
        /// Maps all Match nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Match objects mapped from XML document</returns>
        private IEnumerable<MatchHistoryDTO> MapMatches(XmlDocument document)
        {
            var allMatches = new List<MatchHistory>();
            var matches = document.SelectNodes(MatchNodes);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = new MatchHistory()
                {
                    Id = Int32.Parse(matches[i].SelectSingleNode(IdAttribute).InnerText),
                    Name = matches[i].SelectSingleNode(NameAttribute).InnerText,
                    StartDate = DateTime.Parse(matches[i].SelectSingleNode(StartDateAttribute).InnerText),
                    MatchType = Enum.Parse<MatchType>(matches[i].SelectSingleNode("@" + MatchTypeAttribute).InnerText),
                    EventHistoryId = Int32.Parse(matches[i].SelectSingleNode("@" + EventIdAttribute).InnerText),
                };
                allMatches.Add(match);
            }

            return allMatches.Select(m => new MatchHistoryDTO(m));
        }

        /// <summary>
        /// Maps all Bet nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Bet objects mapped from XML document</returns>
        private IEnumerable<BetHistoryDTO> MapBets(XmlDocument document)
        {
            var allBets = new List<BetHistory>();
            var bets = document.SelectNodes(BetNodes);
            for (int i = 0; i < bets.Count; i++)
            {
                var bet = new BetHistory()
                {
                    Id = Int32.Parse(bets[i].SelectSingleNode(IdAttribute).InnerText),
                    Name = bets[i].SelectSingleNode(NameAttribute).InnerText,
                    IsLive = bets[i].SelectSingleNode(IsLiveAttribute).InnerText == "true",
                    MatchHistoryId = Int32.Parse(bets[i].SelectSingleNode("@" + MatchIdAttribute).InnerText),
                    MatchType = Enum.Parse<MatchType>(bets[i].SelectSingleNode("@" + MatchTypeAttribute).InnerText),
                    MatchStartDate = DateTime.Parse(bets[i].SelectSingleNode("@" + MatchStartDateAttribute).InnerText)
                };

                allBets.Add(bet);
            }

            return allBets.Select(b => new BetHistoryDTO(b));
        }

        /// <summary>
        /// Maps all Odd nodes to objects from XML document
        /// </summary>
        /// <param name="document">XML document with data</param>
        /// <returns>List with all Odd objects mapped from XML document</returns>
        private IEnumerable<OddHistoryDTO> MapOdds(XmlDocument document)
        {
            var allOdds = new List<OddHistory>();
            var odds = document.SelectNodes(OddNodes);
            for (int i = 0; i < odds.Count; i++)
            {
                var odd = new OddHistory()
                {
                    Id = Int32.Parse(odds[i].SelectSingleNode(IdAttribute).InnerText),
                    Name = odds[i].SelectSingleNode(NameAttribute).InnerText,
                    Value = Convert.ToDecimal(odds[i].SelectSingleNode(ValueAttribute).InnerText),
                    SpecialValueBet = odds[i].SelectSingleNode(SpecialBetValueAttribute)?.InnerText,
                    BetHistoryId = Int32.Parse(odds[i].SelectSingleNode("@" + BetIdAttribute).InnerText),
                };

                allOdds.Add(odd);
            }

            return allOdds.Select(o => new OddHistoryDTO(o));
        }

        /// <summary>
        /// Adds all Sport objects from XML to SportHistories database table
        /// </summary>
        /// <param name="allSports">All Sport objects from current XML document</param>
        private void AddSportHistories(IEnumerable<SportHistoryDTO> allSports)
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
                            [Name] [TEXT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpSportsTable";
                            bulkCopy.WriteToServer(allSports.ToDataTable());
                        }

                        command.CommandText = @"
                            MERGE INTO dbo.SportHistories AS TARGET
                            USING dbo.#TmpSportsTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name)
                                VALUES (SOURCE.Id, SOURCE.Name);
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
        /// Adds all Event objects from XML to EventtHistories database table
        /// </summary>
        /// <param name="allEvents">All Event objects from current XML document</param>
        private void AddEventHistories(IEnumerable<EventHistoryDTO> allEvents)
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
                            [CategoryId] [INT],
                            [SportHistoryId] [INT],
                            [Name] [TEXT],
                            [IsLive] [BIT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpEventTable";
                            bulkCopy.WriteToServer(allEvents.ToDataTable());
                        }

                        command.CommandText = @"
                            MERGE INTO dbo.EventHistories AS TARGET
                            USING dbo.#TmpEventTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, IsLive, CategoryId, SportHistoryId)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.CategoryId, SOURCE.SportHistoryId);
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
        /// Adds all new Match objects from XML to MatchHistories database table
        /// </summary>
        /// <param name="allMatches">All Match objects from current XML document</param>
        private void AddMatchHistories(IEnumerable<MatchHistoryDTO> allMatches)
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
                            [EventHistoryId] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpMatchTable";
                            bulkCopy.WriteToServer(allMatches.ToDataTable());
                        }

                        command.CommandText = @"
                            MERGE INTO dbo.MatchHistories AS TARGET
                            USING dbo.#TmpMatchTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id AND
                               TARGET.StartDate = SOURCE.StartDate AND
                               TARGET.MatchType = SOURCE.MatchType
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, StartDate, MatchType, EventHistoryId, DateCreated)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.StartDate, SOURCE.MatchType, SOURCE.EventHistoryId, GETDATE());
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
        /// Adds all new Bet objects from XML to BetHistories database table
        /// </summary>
        /// <param name="allBets">All Bet objects from current XML document</param>
        private void AddBetHistories(IEnumerable<BetHistoryDTO> allBets)
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
                            [Name] [TEXT] NULL,
                            [IsLive] [BIT] NULL,
                            [MatchHistoryId] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpBetTable";
                            bulkCopy.WriteToServer(allBets.ToDataTable());
                        }

                        command.CommandTimeout = 3000;

                        command.CommandText = @"
                            MERGE INTO dbo.BetHistories AS TARGET
                            USING dbo.#TmpBetTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id AND
                               TARGET.IsLive = SOURCE.IsLive
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, IsLive, MatchHistoryId, DateCreated)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.MatchHistoryId, GETDATE());
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
        /// Adds all new Odd objects from XML to OddHistories database table
        /// </summary>
        /// <param name="allOdds">All Odd objects from current XML document</param>
        private void AddOddHistories(IEnumerable<OddHistoryDTO> allOdds)
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
                            [SpecialValueBet] [TEXT] NULL,
                            [BetHistoryId] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpOddTable";
                            bulkCopy.WriteToServer(allOdds.ToDataTable());
                        }

                        command.CommandTimeout = 3000;

                        command.CommandText = @"
                            MERGE INTO dbo.OddHistories AS TARGET
                            USING dbo.#TmpOddTable AS SOURCE
                            ON TARGET.Id = SOURCE.Id AND
                               TARGET.Value = SOURCE.Value
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (Id, Name, Value, SpecialValueBet, BetHistoryId, DateCreated)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.Value, SOURCE.SpecialValueBet, SOURCE.BetHistoryId, GETDATE());
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

        /// <summary>
        /// Adds, Updates and Deletes Sport objects from Sport database table according to current XML document
        /// </summary>
        /// <param name="allSports">All Sport objects from current XML document</param>
        private void AddSports(IEnumerable<SportHistoryDTO> allSports)
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
                            [Name] [TEXT])";

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
                                INSERT (Id, Name)
                                VALUES (SOURCE.Id, SOURCE.Name)
                            WHEN NOT MATCHED BY SOURCE THEN
                                DELETE;
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
        private void AddEvents(IEnumerable<EventHistoryDTO> allEvents)
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
                            [CategoryId] [INT],
                            [SportIdentification] [INT],
                            [Name] [TEXT],
                            [IsLive] [BIT])";

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
                                INSERT (Id, Name, IsLive, CategoryId, SportId)
                                VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.CategoryId, SOURCE.SportIdentification)
                            WHEN NOT MATCHED BY SOURCE THEN
                                DELETE;
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
        private void AddMatches(IEnumerable<MatchHistoryDTO> allMatches)
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
                            [EventIdentification] [INT])";

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
                                    ActionType,
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
                                    OUTPUT $action as ActionType, INSERTED.Id as MatchXmlId, INSERTED.Name, INSERTED.StartDate, INSERTED.MatchType, INSERTED.EventId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, MatchXmlId, Name, StartDate, MatchType, EventId, DateCreated)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.MatchChangeLogs
                                SELECT 
                                    MatchXmlId,
                                    Name,
                                    StartDate,
                                    MatchType,
                                    EventId,
                                    ActionType,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Matches AS TARGET
                                    USING dbo.#TmpMatchTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.MatchType = SOURCE.MatchType AND
                                       TARGET.StartDate = SOURCE.StartDate
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, StartDate, MatchType, EventId)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.StartDate, SOURCE.MatchType, SOURCE.EventIdentification)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                        DELETE
                                    OUTPUT $action as ActionType, DELETED.Id as MatchXmlId, DELETED.Name, DELETED.StartDate, DELETED.MatchType, DELETED.EventId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, MatchXmlId, Name, StartDate, MatchType, EventId, DateCreated)
                                WHERE AllChanges.ActionType = 'DELETE'
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
        private void AddBets(IEnumerable<BetHistoryDTO> allBets)
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
                            [Name] [TEXT],
                            [IsLive] [BIT],
                            [MatchIdentification] [INT])";

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
                                    ActionType,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Bets AS TARGET
                                    USING dbo.#TmpBetTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.IsLive != SOURCE.IsLive
                                    WHEN MATCHED THEN
                                        UPDATE SET TARGET.IsLive = SOURCE.IsLive
                                    OUTPUT $action as ActionType, INSERTED.Id as BetXmlId, INSERTED.Name, INSERTED.IsLive, INSERTED.MatchId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, BetXmlId, Name, IsLive, MatchId, DateCreated)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.BetChangeLogs
                                SELECT 
                                    BetXmlId,
                                    Name,
                                    IsLive,
                                    MatchId,
                                    ActionType,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Bets AS TARGET
                                    USING dbo.#TmpBetTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.IsLive = SOURCE.IsLive
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, IsLive, MatchId)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.MatchIdentification)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                        DELETE
                                    OUTPUT $action as ActionType, DELETED.Id as BetXmlId, DELETED.Name, DELETED.IsLive, DELETED.MatchId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, BetXmlId, Name, IsLive, MatchId, DateCreated)
                                WHERE AllChanges.ActionType = 'DELETE'
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
        private void AddOdds(IEnumerable<OddHistoryDTO> allOdds)
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
                            [SpecialValueBet] [TEXT] NULL,
                            [BetIdentification] [INT])";

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
                                    ActionType,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Odds AS TARGET
                                    USING dbo.#TmpOddTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.Value != SOURCE.Value 
                                    WHEN MATCHED THEN
                                        UPDATE SET TARGET.Value = SOURCE.Value
                                    OUTPUT $action as ActionType, INSERTED.Id as OddXmlId, INSERTED.Name, INSERTED.Value, INSERTED.SpecialValueBet, INSERTED.BetId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, OddXmlId, Name, Value, SpecialValueBet, BetId, DateCreated)
                                WHERE AllChanges.ActionType = 'UPDATE'

                            INSERT INTO dbo.OddChangeLogs
                                SELECT 
                                    OddXmlId,
                                    Name,
                                    Value,
                                    SpecialValueBet,
                                    BetId,
                                    ActionType,
                                    DateCreated
                                FROM (
                                    MERGE INTO dbo.Odds AS TARGET
                                    USING dbo.#TmpOddTable AS SOURCE
                                    ON TARGET.Id = SOURCE.Id AND
                                       TARGET.Value = SOURCE.Value
                                    WHEN NOT MATCHED BY TARGET THEN
                                        INSERT (Id, Name, Value, SpecialValueBet, BetId)
                                        VALUES (SOURCE.Id, SOURCE.Name, SOURCE.Value, SOURCE.SpecialValueBet, SOURCE.BetIdentification)
                                    WHEN NOT MATCHED BY SOURCE THEN
                                        DELETE
                                    OUTPUT $action as ActionType, DELETED.Id as OddXmlId, DELETED.Name, DELETED.Value, DELETED.SpecialValueBet, DELETED.BetId, GETDATE() as DateCreated
                                ) AllChanges (ActionType, OddXmlId, Name, Value, SpecialValueBet, BetId, DateCreated)
                                WHERE AllChanges.ActionType = 'DELETE'
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
