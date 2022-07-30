using BettingAPI.DataContext.Models.Active;

namespace BettingAPI.Services.Models
{
    public class EventDTO
    {
        public EventDTO(Event eventEntity)
        {
            this.Id = eventEntity.Id;
            this.Name = eventEntity.Name;
            this.IsLive = eventEntity.IsLive;
            this.CategoryID = eventEntity.CategoryID;
            this.SportId = eventEntity.SportId;
            this.IsActive = eventEntity.IsActive;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int CategoryID { get; set; }

        public int SportId { get; set; }

        public bool IsActive { get; set; }
    }
}
