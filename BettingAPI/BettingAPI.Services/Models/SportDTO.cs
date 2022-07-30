using BettingAPI.DataContext.Models.Active;

namespace BettingAPI.Services.Models
{
    public class SportDTO
    {
        public SportDTO(Sport sport)
        {
            this.Id = sport.Id;
            this.Name = sport.Name;
            this.IsActive = sport.IsActive;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
