using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
namespace Catalogo.Application.Services
{
    public class ProdutoService : IProdutoService
    {


        private readonly IProdutoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ProdutoService(IProdutoRepository repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;

        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosAsync()
        {

            var produtoDTO = await _cache.GetOrCreateAsync(CacheKeys.PRODUTOS_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var produtos = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

            });
            return produtoDTO;

        }
        public async Task<ProdutoResponseDTO?> GetProdutoByIdAsync(int id)
        {
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            return _mapper.Map<ProdutoResponseDTO>(produto);
        }
        public async Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produtoDTO)
        {

            var produto = _mapper.Map<Produto>(produtoDTO);
            var produtoSalvo = await _repository.CreateAsync(produto);
            _cache.Remove(CacheKeys.PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            return _mapper.Map<ProdutoResponseDTO>(produtoSalvo);
        }
        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO)
        {
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

            if (produto == null) return null;

            _mapper.Map(produtoDTO, produto);

            var produtoAtualizado = await _repository.UpdateAsync(produto);
            _cache.Remove(CacheKeys.PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);

            return _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);

        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto == null) return false;

            await _repository.DeleteAsync(produto);
            _cache.Remove(CacheKeys.PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            return true;
        }
    }
}