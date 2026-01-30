using AutoMapper;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;


namespace Catalogo.Application.Mappings
{
    public class MappingProfile : Profile
    {
        // Como os nomes das propriedades são IDÊNTICOS 
        // (ex: Produto.Nome e ProdutoResponseDTO.Nome),
        // o AutoMapper faz tudo sozinho.

        public MappingProfile()
        {
            CreateMap<Produto, ProdutoResponseDTO>();
            CreateMap<Categoria, CategoriaResponseDTO>();
            CreateMap<Fornecedor, FornecedorResponseDTO>();

            CreateMap<Categoria, CategoriaResponseProdutosDTO>();
            CreateMap<Fornecedor, FornecedorResponseProdutoDTO>();

            CreateMap<ProdutoCreateDTO, Produto>();
            CreateMap<CategoriaCreateDTO, Categoria>();
            CreateMap<FornecedorCreateDTO, Fornecedor>();


        }

    }
}