using Catalogo.Core.Entities;

namespace Catalogo.Core.Interfaces
{
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        Task<IEnumerable<Fornecedor>> GetFornecedoresProdutosAsync();
    }
}