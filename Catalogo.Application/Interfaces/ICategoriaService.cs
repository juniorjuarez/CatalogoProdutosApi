using Catalogo.Application.DTOs;


namespace Catalogo.Application.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync();
        Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id);
        Task<IEnumerable<CategoriaResponseProdutosDTO>> GetCategoriasProdutosAsync();
        Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto);
        Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto);
        Task<bool> DeleteCategoriaAsync(int id);


    }
}