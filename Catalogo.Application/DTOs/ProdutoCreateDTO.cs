using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;

namespace Catalogo.Application.DTOs
{
    public class ProdutoCreateDTO
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public decimal Preco { get; set; }
        public float Estoque { get; set; }
        public int CategoriaId { get; set; }
        public int? FornecedorId { get; set; }
    }
}