using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext.Infrastructure
{
    public class BetHistoryConfig : IEntityTypeConfiguration<BetHistory>
    {
        public void Configure(EntityTypeBuilder<BetHistory> builder)
        {
            builder.HasOne(b => b.MatchHistory)
                .WithMany(m => m.BetHistories)
                .HasForeignKey(b => new { b.MatchHistoryId, b.MatchType, b.MatchStartDate });
        }
    }
}
