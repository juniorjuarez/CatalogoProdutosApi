using Catalogo.Core.Entities; // Para encontrar Produto.cs
using Catalogo.Core.Interfaces; // Para encontrar IProdutoRepository
using Catalogo.Infrastructure.Data.Context;

namespace Catalogo.Infrastructure.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }
    }
}