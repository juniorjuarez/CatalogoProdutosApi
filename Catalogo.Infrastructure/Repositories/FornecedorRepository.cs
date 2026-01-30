using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Catalogo.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Repositories
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Fornecedor>> GetFornecedoresProdutosAsync()
        {
            return  await _context.Fornecedores.Include(f=> f.Produtos).AsNoTracking().ToListAsync();
        }
    }
}