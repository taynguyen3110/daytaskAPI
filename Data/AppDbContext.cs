using Microsoft.EntityFrameworkCore;
using daytask.Models;

namespace daytask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>()
                .HasIndex(t => t.UserId);
            modelBuilder.Entity<Note>()
                .HasIndex(n => n.UserId);

            base.OnModelCreating(modelBuilder);
        }  
    }
}
