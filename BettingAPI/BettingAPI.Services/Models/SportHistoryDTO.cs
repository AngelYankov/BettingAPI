using BettingAPI.DataContext.Models.History;

namespace BettingAPI.Services.Models
{
    public class SportHistoryDTO
    {
        public SportHistoryDTO(SportHistory sportHistory)
        {
            this.Id = sportHistory.Id;
            this.Name = sportHistory.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
