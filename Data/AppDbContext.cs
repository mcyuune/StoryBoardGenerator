using Microsoft.EntityFrameworkCore;
using StoryBoardGenerator.Models;

namespace StoryBoardGenerator.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Scene> Scenes => Set<Scene>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Story>(entity =>
        {
            entity.Property(s => s.Title).HasMaxLength(200).IsRequired();
            entity.Property(s => s.SourceText).IsRequired();
            entity.HasMany(s => s.Scenes)
                .WithOne(scene => scene.Story)
                .HasForeignKey(scene => scene.StoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Scene>(entity =>
        {
            entity.Property(s => s.Caption).IsRequired();
            entity.Property(s => s.ImagePath).HasMaxLength(500).IsRequired();
            entity.HasIndex(s => new { s.StoryId, s.Sequence }).IsUnique();
        });
    }
}
