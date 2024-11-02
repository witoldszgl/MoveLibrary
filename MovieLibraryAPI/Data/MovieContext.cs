using Microsoft.EntityFrameworkCore;

public class MovieContext : DbContext
{
    public MovieContext(DbContextOptions<MovieContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Director).IsRequired(false);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Rate).IsRequired();
        });
    }
}
