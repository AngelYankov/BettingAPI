using BettingAPI.DataContext.Models;
using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.ChangeLogs;
using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingAPI.DataContext
{
    public class BettingContext : DbContext
    {
        public BettingContext(DbContextOptions<BettingContext> options)
            : base(options) { }

        public DbSet<SportHistory> SportHistories { get; set; }

        public DbSet<EventHistory> EventHistories { get; set; }

        public DbSet<MatchHistory> MatchHistories { get; set; }

        public DbSet<BetHistory> BetHistories { get; set; }

        public DbSet<OddHistory> OddHistories { get; set; }

        public DbSet<Sport> Sports { get; set; }
        
        public DbSet<Event> Events { get; set; }
        
        public DbSet<Match> Matches { get; set; }
        
        public DbSet<Bet> Bets { get; set; }
        
        public DbSet<Odd> Odds { get; set; }

        public DbSet<MatchChangeLog> MatchChangeLogs { get; set; }

        public DbSet<BetChangeLog> BetChangeLogs { get; set; }
        
        public DbSet<OddChangeLog> OddChangeLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EventHistoryConfig());
            modelBuilder.ApplyConfiguration(new MatchHistoryConfig());
            modelBuilder.ApplyConfiguration(new BetHistoryConfig());
            modelBuilder.ApplyConfiguration(new OddHistoryConfig());
            modelBuilder.ApplyConfiguration(new OddConfig());
        }

        public class EventHistoryConfig : IEntityTypeConfiguration<EventHistory>
        {
            public void Configure(EntityTypeBuilder<EventHistory> builder)
            {
                builder.HasOne(e => e.SportHistory)
                    .WithMany(s => s.EventHistories)
                    .HasForeignKey(e => e.SportHistoryId);
            }
        }

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

        public class BetHistoryConfig : IEntityTypeConfiguration<BetHistory>
        {
            public void Configure(EntityTypeBuilder<BetHistory> builder)
            {
                builder.HasOne(b => b.MatchHistory)
                    .WithMany(m => m.BetHistories)
                    .HasForeignKey(b => new { b.MatchHistoryId, b.MatchType, b.MatchStartDate });
            }
        }

        public class OddHistoryConfig : IEntityTypeConfiguration<OddHistory>
        {
            public void Configure(EntityTypeBuilder<OddHistory> builder)
            {
                builder.HasKey(o => new { o.Id, o.Value });
                builder.Property(o => o.Value).HasColumnType("decimal(6,2)").IsRequired(true);
            }
        }
        
        public class OddConfig : IEntityTypeConfiguration<Odd>
        {
            public void Configure(EntityTypeBuilder<Odd> builder)
            {
                builder.Property(x => x.Value).HasColumnType("decimal(6,2)").IsRequired(true);
            }
        }
    }
}
