using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models.Active;
using System;
using System.Net;
using System.Xml;

namespace BettingAPI.Services
{
    public class DeserializeService : IDeserializeService
    {
        /// <summary>
        /// Adds attributes with data need to XML
        /// </summary>
        /// <returns>New XML with added data</returns>
        public XmlDocument TransformXml()
        {
            var document = LoadFile();

            var sports = document.SelectNodes(Constants.SportNodes);

            for (int m = 0; m < sports.Count; m++)
            {
                var sportEntity = new Sport()
                {
                    Id = Int32.Parse(sports[m].SelectSingleNode(Constants.IdAttribute).InnerText)
                };

                var events = sports[m].ChildNodes;

                for (int i = 0; i < events.Count; i++)
                {
                    var sportIdAttribute = document.CreateAttribute(Constants.SportIdAttribute);
                    sportIdAttribute.Value = sportEntity.Id.ToString();
                    events[i].Attributes.Append(sportIdAttribute);

                    var eventEntity = new Event()
                    {
                        Id = Int32.Parse(events[i].SelectSingleNode(Constants.IdAttribute).InnerText),
                    };

                    var matches = events[i].ChildNodes;

                    for (int j = 0; j < matches.Count; j++)
                    {
                        var eventIdAttribute = document.CreateAttribute(Constants.EventIdAttribute);
                        eventIdAttribute.Value = eventEntity.Id.ToString();
                        matches[j].Attributes.Append(eventIdAttribute);

                        var matchEntity = new Match()
                        {
                            Id = Int32.Parse(matches[j].SelectSingleNode(Constants.IdAttribute).InnerText),
                            MatchType = Enum.Parse<MatchType>(matches[j].SelectSingleNode(Constants.AtChar + Constants.MatchTypeAttribute).InnerText),
                            StartDate = DateTime.Parse(matches[j].SelectSingleNode(Constants.StartDateAttribute).InnerText)
                        };

                        var bets = matches[j].ChildNodes;

                        for (int k = 0; k < bets.Count; k++)
                        {
                            var attributeMatchId = document.CreateAttribute(Constants.MatchIdAttribute);
                            attributeMatchId.Value = matchEntity.Id.ToString();
                            bets[k].Attributes.Append(attributeMatchId);

                            var attributeMatchType = document.CreateAttribute(Constants.MatchTypeAttribute);
                            attributeMatchType.Value = matchEntity.MatchType.ToString();
                            bets[k].Attributes.Append(attributeMatchType);

                            var attributeMatchStartDate = document.CreateAttribute(Constants.MatchStartDateAttribute);
                            attributeMatchStartDate.Value = matchEntity.StartDate.ToString();
                            bets[k].Attributes.Append(attributeMatchStartDate);

                            var betEntity = new Bet()
                            {
                                Id = Int32.Parse(bets[k].SelectSingleNode(Constants.IdAttribute).InnerText),
                            };

                            var odds = bets[k].ChildNodes;

                            for (int l = 0; l < odds.Count; l++)
                            {
                                var attributeBetId = document.CreateAttribute(Constants.BetIdAttribute);
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

            //string filePath = @"C:\Users\Angel\Desktop\data.xml";
            //doc.Load(filePath);

            return doc;
        }
    }
}
