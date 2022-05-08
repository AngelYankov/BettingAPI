using BettingAPI.DataContext.Models.History;

namespace BettingAPI.Services.Models
{
    public class EventHistoryDTO
    {
        public EventHistoryDTO(EventHistory eventHistory)
        {
            this.Id = eventHistory.Id;
            this.CategoryId = eventHistory.CategoryID;
            this.SportHistoryId = eventHistory.SportHistoryId;
            this.Name = eventHistory.Name;
            this.IsLive = eventHistory.IsLive;
        }

        public int Id { get; set; }
                
        public int CategoryId { get; set; }
        
        public int SportHistoryId { get; set; }
                
        public string Name { get; set; }

        public bool IsLive { get; set; }
    }
}
