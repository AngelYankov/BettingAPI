using BettingAPI.DataContext.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace BettingAPI.DataContext
{
    public class BettingContext : DbContext
    {
        public BettingContext(DbContextOptions<BettingContext> options) 
            : base(options) { }

        public DbSet<Event> Events { get; set; }

        public DbSet<Sport> Sports { get; set; }
        
        public DbSet<Match> Matches { get; set; }
        
        public DbSet<Bet> Bets { get; set; }
        
        public DbSet<Odd> Odds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new OddConfig());

        }

        public class OddConfig : IEntityTypeConfiguration<Odd>
        {
            public void Configure(EntityTypeBuilder<Odd> builder)
            {
                builder.Property(x => x.Value).HasColumnType("decimal(5,2)").IsRequired(true);

                builder.Property(x => x.SpecialValueBet).HasColumnType("decimal(5,2)").IsRequired(false);

            }
        }
    }
}
