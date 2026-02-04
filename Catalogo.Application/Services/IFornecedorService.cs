using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalogo.Application.DTOs;

namespace Catalogo.Application.Services
{
    public interface IFornecedorService
    {
        Task<IEnumerable<FornecedorResponseDTO>> GetFornecedorAsync();
        Task<FornecedorResponseDTO> GetFornecedorByIdAsync(int id);
        Task<IEnumerable<FornecedorResponseProdutoDTO>> GetFornecedorResponseProdutosAsync();

        Task<FornecedorResponseDTO> CreateFornecedorAsync(FornecedorCreateDTO fornecedorDto);

        Task<FornecedorResponseDTO?> UpdateFornecedorAsync(int id, FornecedorCreateDTO fornecedorDto);

        Task<bool> DeleteFornecedorAsync(int id);

    }
}