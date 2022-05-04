using System.Collections.Generic;

namespace BettingAPI.DataContext.Models
{
    public class Sport
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Event> Events { get; set; }
    }
}
