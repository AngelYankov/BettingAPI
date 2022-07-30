using BettingAPI.DataContext.Infrastructure;
using BettingAPI.DataContext.Models;
using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.ChangeLogs;
using Microsoft.EntityFrameworkCore;

namespace BettingAPI.DataContext
{
    public class BettingContext : DbContext
    {
        public BettingContext(DbContextOptions<BettingContext> options)
            : base(options) { }

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

            modelBuilder.ApplyConfiguration(new OddConfig());
        }
    }
}
