using Microsoft.EntityFrameworkCore;
using ExerciseTrackerAPI.Models;

namespace ExerciseTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ExerciseType> ExerciseTypes { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExerciseType>()
                .HasIndex(e => e.TypeName)
                .IsUnique();

            modelBuilder.Entity<ExerciseType>()
                .HasData(PredefinedExerciseTypes.AsExerciseTypes());

            base.OnModelCreating(modelBuilder);
        }
    }
}