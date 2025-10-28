using Catalogo.Core.Entities;

namespace Catalogo.Core.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetCategoriasProdutosAsync();
    }
}