using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<TaskDetails> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
}
