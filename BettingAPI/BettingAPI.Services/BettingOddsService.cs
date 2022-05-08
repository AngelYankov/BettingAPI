﻿using BettingAPI.DataContext;
using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models.History;
using BettingAPI.Services.Interfaces;
using BettingAPI.Services.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml;

namespace BettingAPI.Services
{
    public class BettingOddsService : IBettingOddsService
    {
        private readonly BettingContext context;

        public BettingOddsService(BettingContext context)
        {
            this.context = context;
        }

        //public void SaveData()
        //{
        //    var doc1 = TransformXml();
        //    var doc = LoadFile();

        //    //var sportEntity = new Sport()
        //    //{
        //    //    Name = doc.SelectNodes("//XmlSports//Sport")[0].SelectSingleNode("@Name").InnerText,
        //    //    Id = Int32.Parse(doc.SelectSingleNode("//Sport").SelectSingleNode("@Id").InnerText)
        //    //};

        //    //this.context.Sports.Add(sportEntity);

        //    var events = doc.SelectNodes("//Event");

        //    for (int i = 0; i < events.Count; i++)
        //    {
        //        var eventEntity = new Event()
        //        {
        //            Id = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText),
        //            CategoryID = Int32.Parse(events[i].SelectSingleNode("@CategoryID").InnerText),
        //            Name = events[i].SelectSingleNode("@Name").InnerText,
        //            IsLive = events[i].SelectSingleNode("@IsLive").InnerText == "true"
        //        };

        //        //var existingEvent = GetEvent(eventEntity.Id);

        //        //if (existingEvent == null)
        //        //{
        //        this.context.Events.Add(eventEntity);
        //        //}

        //        var matches = events[i].ChildNodes;

        //        for (int j = 0; j < matches.Count; j++)
        //        {
        //            var eventIdAttribute = doc.CreateAttribute("EventId");
        //            eventIdAttribute.Value = eventEntity.Id.ToString();
        //            matches[j].Attributes.Append(eventIdAttribute);

        //            var matchEntity = new Match()
        //            {
        //                Id = Int32.Parse(matches[j].SelectSingleNode("@ID").InnerText),
        //                Name = matches[j].SelectSingleNode("@Name").InnerText,
        //                StartDate = DateTime.Parse(matches[j].SelectSingleNode("@StartDate").InnerText),
        //                MatchType = Enum.Parse<MatchType>(matches[j].SelectSingleNode("@MatchType").InnerText),
        //                EventId = eventEntity.Id
        //            };

        //            this.context.Matches.Add(matchEntity);

        //            var bets = matches[j].ChildNodes;

        //            for (int k = 0; k < bets.Count; k++)
        //            {
        //                var betEntity = new Bet()
        //                {
        //                    Id = Int32.Parse(bets[k].SelectSingleNode("@ID").InnerText),
        //                    Name = bets[k].SelectSingleNode("@Name").InnerText,
        //                    IsLive = bets[k].SelectSingleNode("@IsLive").InnerText == "true",
        //                    MatchId = matchEntity.Id
        //                };

        //                this.context.Bets.Add(betEntity);

        //                var odds = bets[k].ChildNodes;

        //                for (int l = 0; l < odds.Count; l++)
        //                {
        //                    var oddEntity = new Odd()
        //                    {
        //                        Id = Int32.Parse(odds[l].SelectSingleNode("@ID").InnerText),
        //                        Name = odds[l].SelectSingleNode("@Name").InnerText,
        //                        Value = Convert.ToDecimal(odds[l].SelectSingleNode("@Value").InnerText),
        //                        SpecialValueBet = odds[l].SelectSingleNode("@SpecialBetValue")?.InnerText,
        //                        BetId = betEntity.Id
        //                    };

        //                    this.context.Odds.Add(oddEntity);
        //                }
        //            }
        //        }
        //    }
        //    doc.Save(@"C:\Users\Angel\Desktop\tt.xml");
        //    this.context.SaveChanges();
        //}

        public void Print(int number)
        {
            Console.WriteLine($"Log number: {number}");
        }

        public void Save()
        {
            var document = TransformXml();

            var allSports = SaveSports(document);
            var allEvents = SaveEvents(document);
            var allMatches = SaveMatches(document);
            var allBets = SaveBets(document, allMatches);
            var allOdds = SaveOdds(document);

            AddSports(allSports);
            AddEvents(allEvents);
            AddMatches(allMatches);
            AddBets(allBets);
            AddOdds(allOdds);
        }

        private void AddSports(IEnumerable<SportHistoryDTO> allSports)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = "Server=.\\; Database=BettingDB; Integrated Security=True" })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpSportsTable(
                        [Id] [INT],
                        [Name] [TEXT] NULL)";

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
                    catch (System.Exception ex)
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

        private void AddEvents(IEnumerable<EventHistoryDTO> eventHistories)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = "Server=.\\; Database=BettingDB; Integrated Security=True" })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpEventTable(
                        [Id] [INT],
                        [CategoryId] [INT] NULL,
                        [SportIdentification] [INT],
                        [Name] [TEXT] NULL,
                        [IsLive] [BIT] NULL)";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpEventTable";
                            bulkCopy.WriteToServer(eventHistories.ToDataTable());
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
                    catch (System.Exception ex)
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

        private void AddMatches(IEnumerable<MatchHistoryDTO> matchHistories)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = "Server=.\\; Database=BettingDB; Integrated Security=True" })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpMatchTable(
                        [Id] [INT],
                        [Name] [TEXT] NULL,
                        [StartDate] [DATETIME] NULL,
                        [MatchType] [INT],
                        [EventIdentification] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpMatchTable";
                            bulkCopy.WriteToServer(matchHistories.ToDataTable());
                        }

                        command.CommandText = @"
                                MERGE INTO dbo.Matches AS TARGET
                                USING dbo.#TmpMatchTable AS SOURCE
                                ON TARGET.Id = SOURCE.Id
                                WHEN MATCHED THEN
                                    UPDATE SET TARGET.MatchType = SOURCE.MatchType,
                                               TARGET.StartDate = SOURCE.StartDate
                                WHEN NOT MATCHED BY TARGET THEN
                                    INSERT (Id, Name, StartDate, MatchType, EventId)
                                    VALUES (SOURCE.Id, SOURCE.Name, SOURCE.StartDate, SOURCE.MatchType, SOURCE.EventIdentification)
                                WHEN NOT MATCHED BY SOURCE THEN
                                    DELETE;

                                --OUTPUT $action AS ActionType,
                                --INSERTED.Name AS Name,
                                --INSERTED.StartDate AS StartDate,
                                --INSERTED.MatchType AS MatchType
                                --INSERTED.EventId AS EventId
                                --INTO dbo.ChangeLogs;

                                DROP TABLE #TmpMatchTable";

                        command.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
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

        private void AddBets(IEnumerable<BetHistoryDTO> betHistories)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = "Server=.\\; Database=BettingDB; Integrated Security=True" })
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
                        [MatchIdentification] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpBetTable";
                            bulkCopy.WriteToServer(betHistories.ToDataTable());
                        }

                        command.CommandTimeout = 3000;

                        command.CommandText = @"
                                MERGE INTO dbo.Bets AS TARGET
                                USING dbo.#TmpBetTable AS SOURCE
                                ON TARGET.Id = SOURCE.Id
                                WHEN MATCHED THEN
                                    UPDATE SET TARGET.IsLive = SOURCE.IsLive
                                    --INSERT INTO dbo.ChangeLogs (ValueChangedName, ValueChangedId, OldValue, NewValue)
                                    --VALUES (Source.Name, Source.Id, TARGET.IsLive, SOURCE.IsLive)
                                WHEN NOT MATCHED BY TARGET THEN
                                    INSERT (Id, Name, IsLive, MatchId)
                                    VALUES (SOURCE.Id, SOURCE.Name, SOURCE.IsLive, SOURCE.MatchIdentification)
                                WHEN NOT MATCHED BY SOURCE THEN
                                    DELETE;
                                DROP TABLE #TmpBetTable";

                        command.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
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

        private void AddOdds(IEnumerable<OddHistoryDTO> oddHistories)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = "Server=.\\; Database=BettingDB; Integrated Security=True" })
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();

                        command.CommandText = @"CREATE TABLE #TmpOddTable(
                        [Id] [INT],
                        [Name] [TEXT] NULL,
                        [VALUE] [DECIMAL] NULL,
                        [SpecialValueBet] [TEXT] NULL,
                        [BetIdentification] [INT])";

                        command.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TmpOddTable";
                            bulkCopy.WriteToServer(oddHistories.ToDataTable());
                        }

                        command.CommandTimeout = 3000;

                        command.CommandText = @"
                                MERGE INTO dbo.Odds AS TARGET
                                USING dbo.#TmpOddTable AS SOURCE
                                ON TARGET.Id = SOURCE.Id
                                WHEN MATCHED THEN
                                    UPDATE SET TARGET.Value = SOURCE.Value
                                    --INSERT INTO dbo.ChangeLogs (ValueChangedName, ValueChangedId, OldValue, NewValue)
                                    --VALUES (Source.Name, Source.Id, convert(varchar,convert(decimal(8,2),TARGET.Value)), convert(varchar,convert(decimal(8,2),SOURCE.Value)))
                                WHEN NOT MATCHED BY TARGET THEN
                                    INSERT (Id, Name, Value, SpecialValueBet, BetId)
                                    VALUES (SOURCE.Id, SOURCE.Name, SOURCE.Value, SOURCE.SpecialValueBet, SOURCE.BetIdentification)
                                WHEN NOT MATCHED BY SOURCE THEN
                                    DELETE;
                                DROP TABLE #TmpOddTable";

                        command.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
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

        private IEnumerable<SportHistoryDTO> SaveSports(XmlDocument document)
        {
            var allSports = new List<SportHistory>();
            var sports = document.SelectNodes("//Sport");
            for (int i = 0; i < sports.Count; i++)
            {
                var sport = new SportHistory()
                {
                    Id = Int32.Parse(sports[i].SelectSingleNode("@ID").InnerText),
                    Name = sports[i].SelectSingleNode("@Name").InnerText,
                };

                try
                {
                    this.context.SportHistories.Add(sport);
                    this.context.SaveChanges();
                    allSports.Add(sport);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return allSports.Select(s => new SportHistoryDTO(s));
        }

        private IEnumerable<EventHistoryDTO> SaveEvents(XmlDocument document)
        {
            var allEvents = new List<EventHistory>();
            var events = document.SelectNodes("//Event");
            for (int i = 0; i < events.Count; i++)
            {
                var eventHistory = new EventHistory()
                {
                    Id = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText),
                    CategoryID = Int32.Parse(events[i].SelectSingleNode("@CategoryID").InnerText),
                    Name = events[i].SelectSingleNode("@Name").InnerText,
                    IsLive = events[i].SelectSingleNode("@IsLive").InnerText == "true",
                    SportHistoryId = Int32.Parse(events[i].SelectSingleNode("@SportId").InnerText),
                    //SportHistoryId = GetSport(Int32.Parse(events[i].SelectSingleNode("@SportId").InnerText)).Id
                };

                try
                {
                    this.context.EventHistories.Add(eventHistory);
                    this.context.SaveChanges();
                    allEvents.Add(eventHistory);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return allEvents.Select(e => new EventHistoryDTO(e));
        }

        private IEnumerable<MatchHistoryDTO> SaveMatches(XmlDocument document)
        {
            var allMatches = new List<MatchHistory>();
            var matches = document.SelectNodes("//Match");
            for (int i = 0; i < matches.Count; i++)
            {
                var match = new MatchHistory()
                {
                    Id = Int32.Parse(matches[i].SelectSingleNode("@ID").InnerText),
                    Name = matches[i].SelectSingleNode("@Name").InnerText,
                    StartDate = DateTime.Parse(matches[i].SelectSingleNode("@StartDate").InnerText),
                    MatchType = Enum.Parse<MatchType>(matches[i].SelectSingleNode("@MatchType").InnerText),
                    EventHistoryId = Int32.Parse(matches[i].SelectSingleNode("@EventId").InnerText),
                    //EventHistoryId = GetEvent(Int32.Parse(matches[i].SelectSingleNode("@EventId").InnerText)).Id
                };

                try
                {
                    this.context.MatchHistories.Add(match);
                    this.context.SaveChanges();
                    allMatches.Add(match);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return allMatches.Select(m => new MatchHistoryDTO(m));
        }

        private IEnumerable<BetHistoryDTO> SaveBets(XmlDocument document, IEnumerable<MatchHistoryDTO> allMatches)
        {
            var allBets = new List<BetHistory>();
            var bets = document.SelectNodes("//Bet");
            for (int i = 0; i < bets.Count; i++)
            {
                var bet = new BetHistory()
                {
                    Id = Int32.Parse(bets[i].SelectSingleNode("@ID").InnerText),
                    Name = bets[i].SelectSingleNode("@Name").InnerText,
                    IsLive = bets[i].SelectSingleNode("@IsLive").InnerText == "true",
                    MatchHistoryId = Int32.Parse(bets[i].SelectSingleNode("@MatchId").InnerText),
                    MatchType = (MatchType)GetMatch(Int32.Parse(bets[i].SelectSingleNode("@MatchId").InnerText), allMatches).MatchType,
                    MatchStartDate = GetMatch(Int32.Parse(bets[i].SelectSingleNode("@MatchId").InnerText), allMatches).StartDate
                    //MatchHistoryId = GetMatch(Int32.Parse(bets[i].SelectSingleNode("@MatchId").InnerText)).Id
                };

                try
                {
                    this.context.BetHistories.Add(bet);
                    this.context.SaveChanges();
                    allBets.Add(bet);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return allBets.Select(b => new BetHistoryDTO(b));
        }

        private IEnumerable<OddHistoryDTO> SaveOdds(XmlDocument document)
        {
            var allOdds = new List<OddHistory>();
            var odds = document.SelectNodes("//Odd");
            for (int i = 0; i < odds.Count; i++)
            {
                var odd = new OddHistory()
                {
                    Id = Int32.Parse(odds[i].SelectSingleNode("@ID").InnerText),
                    Name = odds[i].SelectSingleNode("@Name").InnerText,
                    Value = Convert.ToDecimal(odds[i].SelectSingleNode("@Value").InnerText),
                    SpecialValueBet = odds[i].SelectSingleNode("@SpecialBetValue")?.InnerText,
                    BetHistoryId = Int32.Parse(odds[i].SelectSingleNode("@BetId").InnerText),
                    //BetHistoryId = GetBet(Int32.Parse(odds[i].SelectSingleNode("@BetId").InnerText)).Id
                };

                try
                {
                    this.context.OddHistories.Add(odd);
                    this.context.SaveChanges();
                    allOdds.Add(odd);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return allOdds.Select(o => new OddHistoryDTO(o));
        }

        private XmlDocument TransformXml()
        {
            var doc = LoadFile();

            var sports = doc.SelectNodes("//Sport");

            for (int m = 0; m < sports.Count; m++)
            {
                var sportEntity = new SportHistory()
                {
                    Id = Int32.Parse(sports[m].SelectSingleNode("@ID").InnerText)
                };

                var events = sports[m].ChildNodes;

                for (int i = 0; i < events.Count; i++)
                {
                    var sportIdAttribute = doc.CreateAttribute("SportId");
                    sportIdAttribute.Value = sportEntity.Id.ToString();
                    events[i].Attributes.Append(sportIdAttribute);

                    var eventEntity = new EventHistory()
                    {
                        Id = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText),
                    };

                    var matches = events[i].ChildNodes;

                    for (int j = 0; j < matches.Count; j++)
                    {
                        var eventIdAttribute = doc.CreateAttribute("EventId");
                        eventIdAttribute.Value = eventEntity.Id.ToString();
                        matches[j].Attributes.Append(eventIdAttribute);

                        var matchEntity = new MatchHistory()
                        {
                            Id = Int32.Parse(matches[j].SelectSingleNode("@ID").InnerText),
                        };

                        var bets = matches[j].ChildNodes;

                        for (int k = 0; k < bets.Count; k++)
                        {
                            var matchIdAttribute = doc.CreateAttribute("MatchId");
                            matchIdAttribute.Value = matchEntity.Id.ToString();
                            bets[k].Attributes.Append(matchIdAttribute);

                            var betEntity = new BetHistory()
                            {
                                Id = Int32.Parse(bets[k].SelectSingleNode("@ID").InnerText),
                            };

                            var odds = bets[k].ChildNodes;

                            for (int l = 0; l < odds.Count; l++)
                            {
                                var betIdAttribute = doc.CreateAttribute("BetId");
                                betIdAttribute.Value = betEntity.Id.ToString();
                                odds[l].Attributes.Append(betIdAttribute);
                            }
                        }
                    }
                }
            }

            doc.Save(@"C:\Users\Angel\Desktop\tt.xml");
            return doc;
        }

        private XmlDocument LoadFile()
        {
            XmlDocument doc = new XmlDocument();
            //doc.Load(@"C:\Users\Angel\Desktop\data.xml");
            string url = "https://sports.ultraplay.net/sportsxml?clientKey=9C5E796D-4D54-42FD-A535-D7E77906541A&sportId=2357&days=7";

            using (var client = new WebClient())
            {
                string result = client.DownloadString(url);
                doc.LoadXml(result);
            }

            return doc;
        }

        private SportHistory GetSport(int id)
        {
            return this.context.SportHistories.First(s => s.Id == id);
        }

        private EventHistory GetEvent(int id)
        {
            return this.context.EventHistories.First(e => e.Id == id);
        }

        //private MatchHistory GetMatch(int id)
        //{
        //    return this.context.MatchHistories.First(m => m.MatchIdentification == id);
        //}

        private MatchHistoryDTO GetMatch(int id, IEnumerable<MatchHistoryDTO> allMatches)
        {
            var match = this.context.MatchHistories.First(m => m.Id == id);
            return new MatchHistoryDTO(match);
        }

        private BetHistory GetBet(int id)
        {
            return this.context.BetHistories.First(b => b.Id == id);
        }
    }
}
