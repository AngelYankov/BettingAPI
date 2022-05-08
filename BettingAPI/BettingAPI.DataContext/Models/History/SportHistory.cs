using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.History
{
    public class SportHistory
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<EventHistory> EventHistories { get; set; }
    }
}
