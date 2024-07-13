using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<TaskDetails> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Team> Teams { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Team>()
            .HasOne(t => t.Manager)
            .WithMany()
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
