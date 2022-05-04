using BettingAPI.DataContext;
using BettingAPI.DataContext.Models;
using System;
using System.Collections.Generic;
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

        public void SaveData()
        {
            var doc = LoadFile();
            var events = doc.SelectNodes("//Event");

            var allMatches = new List<Match>();

            for (int i = 0; i < events.Count; i++)
            {
                context.Events.Add(new Event()
                {
                    Id = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText),
                    CategoryID = Int32.Parse(events[i].SelectSingleNode("@CategoryID").InnerText),
                    Name = events[i].SelectSingleNode("@Name").InnerText,
                    IsLive = events[i].SelectSingleNode("@IsLive").InnerText == "true",
                });

                var matches = events[i].SelectNodes("//Match");

                for (int j = 0; j < matches.Count; j++)
                {
                    var match = new Match();

                    match.Id = Int32.Parse(matches[j].SelectSingleNode("@ID").InnerText);
                    match.Name = matches[j].SelectSingleNode("@Name").InnerText;
                    match.StartDate = DateTime.Parse(matches[j].SelectSingleNode("@StartDate").InnerText);
                    match.MatchType = Enum.Parse<MatchType>(matches[j].SelectSingleNode("@MatchType").InnerText);
                    match.EventId = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText);

                    allMatches.Add(match);

                    //var bets = matches[j].SelectNodes("//Bet");

                    //for (int k = 0; k < bets.Count; k++)
                    //{
                    //    context.Bets.Add(new Bet()
                    //    {
                    //        Id = Int32.Parse(bets[k].SelectSingleNode("@ID").InnerText),
                    //        Name = bets[k].SelectSingleNode("@Name").InnerText,
                    //        IsLive = bets[k].SelectSingleNode("@IsLive").InnerText == "true",
                    //        MatchId = Int32.Parse(matches[j].SelectSingleNode("@ID").InnerText)
                    //    });

                    //    this.context.SaveChanges();

                    //    var odds = bets[k].SelectNodes("//Odd");

                    //    for (int l = 0; l < odds.Count; l++)
                    //    {
                    //        context.Odds.Add(new Odd()
                    //        {
                    //            Id = Int32.Parse(odds[k].SelectSingleNode("@ID").InnerText),
                    //            Name = odds[k].SelectSingleNode("@Name").InnerText,
                    //            Value = Convert.ToDecimal(odds[k].SelectSingleNode("@Value").InnerText),
                    //            SpecialValueBet = Convert.ToDecimal(odds[k].SelectSingleNode("@SpecialBetValue")?.InnerText),
                    //            BetId = Int32.Parse(bets[k].SelectSingleNode("@ID").InnerText),
                    //        });

                    //        this.context.SaveChanges();
                    //    }
                    //}
                }
            }
            context.Matches.AddRange(allMatches);

            this.context.SaveChanges();
        }

        public XmlDocument LoadFile()
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
    }
}
