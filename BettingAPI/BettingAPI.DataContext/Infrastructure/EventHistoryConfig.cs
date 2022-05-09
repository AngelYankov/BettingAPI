using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext.Infrastructure
{
    public class EventHistoryConfig : IEntityTypeConfiguration<EventHistory>
    {
        public void Configure(EntityTypeBuilder<EventHistory> builder)
        {
            builder.HasOne(e => e.SportHistory)
                .WithMany(s => s.EventHistories)
                .HasForeignKey(e => e.SportHistoryId);
        }
    }
}