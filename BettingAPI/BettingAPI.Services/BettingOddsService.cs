using BettingAPI.DataContext;
using BettingAPI.DataContext.Models;
using System;
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
            for (int i = 0; i < events.Count; i++)
            {
                context.Events.Add(new Event()
                {
                    EventID = Int32.Parse(events[i].SelectSingleNode("@ID").InnerText),
                    CategoryID = Int32.Parse(events[i].SelectSingleNode("@CategoryID").InnerText),
                    Name = events[i].SelectSingleNode("@Name").InnerText,
                    IsLive = events[i].SelectSingleNode("@IsLive").InnerText == "true",
                });
            }

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
