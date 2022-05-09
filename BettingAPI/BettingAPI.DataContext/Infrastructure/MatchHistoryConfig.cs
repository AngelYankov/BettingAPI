using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext.Infrastructure
{
    public class MatchHistoryConfig : IEntityTypeConfiguration<MatchHistory>
    {
        public void Configure(EntityTypeBuilder<MatchHistory> builder)
        {
            builder.HasKey(m => new { m.Id, m.MatchType, m.StartDate });

            builder.HasOne(m => m.EventHistory)
                .WithMany(e => e.MatchHistories)
                .HasForeignKey(m => m.EventHistoryId);
        }
    }
}