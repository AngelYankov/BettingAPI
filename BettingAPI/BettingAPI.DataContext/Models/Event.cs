using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int CategoryID { get; set; }

        public List<Match> Matches { get; set; }
    }
}