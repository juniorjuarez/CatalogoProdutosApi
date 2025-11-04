using Catalogo.Application.DTOs;


namespace Catalogo.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoResponseDTO>> GetProdutosAsync();
        Task<ProdutoResponseDTO?> GetProdutoByIdAsync(int id);
        Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produtoDTO);
        Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO);
        Task<bool> DeleteProdutoAsync(int id);
    }
}