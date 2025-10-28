using Catalogo.Core.Entities; // Para encontrar Categoria.cs
using Catalogo.Core.Interfaces; // Para encontrar ICategoriaRepository
using Catalogo.Infrastructure.Data.Context; // Para encontrar AppDbContext
using Microsoft.EntityFrameworkCore;


namespace Catalogo.Infrastructure.Repositories;



public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasProdutosAsync()
    {
        return await _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToListAsync();
    }
}

