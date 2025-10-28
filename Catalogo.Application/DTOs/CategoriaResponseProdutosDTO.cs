using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;

namespace Catalogo.Application.DTOs
{
    public class CategoriaResponseProdutosDTO
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }
        public List<ProdutoResponseDTO> Produtos { get; set; }
    }
}