using BettingAPI.DataContext.Models.Active;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext.Infrastructure
{
    public class OddConfig : IEntityTypeConfiguration<Odd>
    {
        public void Configure(EntityTypeBuilder<Odd> builder)
        {
            builder.Property(x => x.Value).HasColumnType("decimal(6,2)").IsRequired(true);
        }
    }
}
