using Catalogo.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Produto>? Produtos { get; set; }
        public DbSet<Fornecedor>? Fornecedores { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().HasOne(p => p.Fornecedor).WithMany(p => p.Produtos).HasForeignKey(p => p.FornecedorId);
    }
    }

}