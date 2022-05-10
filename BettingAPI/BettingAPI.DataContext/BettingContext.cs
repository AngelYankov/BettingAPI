using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models;
using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.ChangeLogs;
using BettingAPI.DataContext.Models.History;
using Microsoft.EntityFrameworkCore;

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
    }
}
