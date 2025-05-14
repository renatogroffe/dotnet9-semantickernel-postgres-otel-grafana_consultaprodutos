using Microsoft.EntityFrameworkCore;

namespace ConsoleAppChatAIProdutos.Data;

public class CatalogoContext : DbContext
{
    public DbSet<Produto>? Produtos { get; set; }
    public static string? ConnectionString { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString)
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }
}