using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;

namespace Catalogo.Application.DTOs
{
    public class ProdutoResponseDTO
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public int CategoriaId { get; set; }
        public int? FornecedorId { get; set; }
    }
}