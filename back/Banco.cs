using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Individuo> Individuos { get; set; }
    public DbSet<Sequencia> Sequencias { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        string con = "server=localhost;port=3306;database=genes;user=root;password=123123";
        builder.UseMySQL(con)
               .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Individuo>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Sequencia>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Sequencia>()
            .HasOne(s => s.Individuo)
            .WithMany()
            .HasForeignKey(s => s.IndividuoId);
    }
}