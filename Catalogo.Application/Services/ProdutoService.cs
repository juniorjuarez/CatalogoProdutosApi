using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Text.Json;


namespace Catalogo.Application.Services
{
    public class ProdutoService : IProdutoService
    {


        private readonly IProdutoRepository _repository;
        private readonly IMapper _mapper;
        //private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache;


        public ProdutoService(IProdutoRepository repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;

        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosAsync()
        {
            IEnumerable<ProdutoResponseDTO>? produtoDtos;
            string cachekey = CacheKeys.PRODUTOS_KEY;

            string? produtosJson = await _cache.GetStringAsync(cachekey);

            if (produtosJson != null)
            {

                produtoDtos = JsonSerializer.Deserialize<IEnumerable<ProdutoResponseDTO>>(produtosJson);

                return produtoDtos;
            }
            else
            {

                var produtos = await _repository.GetAllAsync();
                produtoDtos = _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

                produtosJson = JsonSerializer.Serialize(produtoDtos);
                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                await _cache.SetStringAsync(cachekey, produtosJson, cacheOptions);

            }


            return produtoDtos;

        }
        public async Task<ProdutoResponseDTO?> GetProdutoByIdAsync(int id)
        {
            ProdutoResponseDTO? produtoDto = null;

            string cacheKey = CacheKeys.ProdutoPrefix + id;

            string? produtoJson = await _cache.GetStringAsync(cacheKey);

            if (produtoJson != null)
            {
                produtoDto = JsonSerializer.Deserialize<ProdutoResponseDTO>(produtoJson);
                return produtoDto;

            }
            else
            {
                var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

                produtoDto = _mapper.Map<ProdutoResponseDTO>(produto);

                produtoJson = JsonSerializer.Serialize(produtoDto);

                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                await _cache.SetStringAsync(cacheKey, produtoJson, cacheOptions);

            }

            return produtoDto;
        }
        public async Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produtoDTO)
        {

            var produto = _mapper.Map<Produto>(produtoDTO);
            var produtoSalvo = await _repository.CreateAsync(produto);
            await _cache.RemoveAsync(CacheKeys.PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            return _mapper.Map<ProdutoResponseDTO>(produtoSalvo);
        }
        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO)
        {

            string cacheKey = CacheKeys.ProdutoPrefix + id;
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

            if (produto == null) return null;

            _mapper.Map(produtoDTO, produto);

            var produtoAtualizado = await _repository.UpdateAsync(produto);
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync(CacheKeys.PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);

            return _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);

        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            string cacheKey = CacheKeys.ProdutoPrefix + id;
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto == null) return false;

            await _repository.DeleteAsync(produto);
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync(CacheKeys.PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            return true;
        }
    }
}