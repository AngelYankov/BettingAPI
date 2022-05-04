using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models
{
    public class Sport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Event> Events { get; set; }
    }
}
