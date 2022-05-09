using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext.Infrastructure
{
    public class OddHistoryConfig : IEntityTypeConfiguration<OddHistory>
    {
        public void Configure(EntityTypeBuilder<OddHistory> builder)
        {
            builder.HasKey(o => new { o.Id, o.Value });
            builder.Property(o => o.Value).HasColumnType("decimal(6,2)").IsRequired(true);
        }
    }
}
