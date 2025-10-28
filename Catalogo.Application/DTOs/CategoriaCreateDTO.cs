using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;

namespace Catalogo.Application.DTOs
{
    public class CategoriaCreateDTO
    {
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }
    }
}