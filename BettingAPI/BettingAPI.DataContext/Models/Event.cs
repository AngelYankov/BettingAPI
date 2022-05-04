using System.Collections.Generic;

namespace BettingAPI.DataContext.Models
{
    public class Event
    {
        public int Id { get; set; }

        public int EventID { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int CategoryID { get; set; }

        public List<Match> Matches { get; set; }

    }
}