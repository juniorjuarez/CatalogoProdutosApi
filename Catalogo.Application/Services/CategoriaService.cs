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
    public class CategoriaService : ICategoriaService
    {

        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;
        //private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache;

        public CategoriaService(ICategoriaRepository repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }


        public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
        {
            IEnumerable<CategoriaResponseDTO>? categoriasDtos = null;

            string cachekey = CacheKeys.CATEGORIAS_KEY;

            string? categoriasJson = await _cache.GetStringAsync(cachekey);

            if (categoriasJson != null)
            {
                categoriasDtos = JsonSerializer.Deserialize<IEnumerable<CategoriaResponseDTO>>(categoriasJson);
                return categoriasDtos;
            }
            else
            {
                var categorias = await _repository.GetAllAsync();
                categoriasDtos = _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);

                categoriasJson = JsonSerializer.Serialize(categoriasDtos);
                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                await _cache.SetStringAsync(cachekey, categoriasJson, cacheOptions);
            }



            return categoriasDtos ?? Enumerable.Empty<CategoriaResponseDTO>();

        }
        public async Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id)
        {
            CategoriaResponseDTO? categoriaDto = null;

            string cachekey = CacheKeys.CategoriaPrefix + id;

            string? categoriaJson = await _cache.GetStringAsync(cachekey);

            if (categoriaJson != null)
            {
                categoriaDto = JsonSerializer.Deserialize<CategoriaResponseDTO>(categoriaJson);
                return categoriaDto;
            }
            else
            {
                var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);
                categoriaDto = _mapper.Map<CategoriaResponseDTO>(categoria);

                var cacheOprions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                categoriaJson = JsonSerializer.Serialize(categoriaDto);

                await _cache.SetStringAsync(cachekey, categoriaJson, cacheOprions);

            }
            return categoriaDto;
        }

        public async Task<IEnumerable<CategoriaResponseProdutosDTO?>> GetCategoriasProdutosAsync()
        {

            IEnumerable<CategoriaResponseProdutosDTO>? categoriaDtos = null;

            string cachekey = CacheKeys.CATEGORIAS_PRODUTOS_KEY;
            string? categoriasJson = await _cache.GetStringAsync(cachekey);

            if (categoriasJson != null)
            {
                categoriaDtos = JsonSerializer.Deserialize<IEnumerable<CategoriaResponseProdutosDTO>>(categoriasJson);
                return categoriaDtos;
            }
            else
            {

                var categorias = await _repository.GetCategoriasProdutosAsync();
                categoriaDtos = _mapper.Map<IEnumerable<CategoriaResponseProdutosDTO>>(categorias);

                categoriasJson = JsonSerializer.Serialize(categoriaDtos);
                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                await _cache.SetStringAsync(cachekey, categoriasJson, cacheOptions);

            }


            return categoriaDtos;
        }


        public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaSalva = await _repository.CreateAsync(categoria);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_KEY);
            return _mapper.Map<CategoriaResponseDTO>(categoriaSalva);
        }
        public async Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto)
        {
            string cachekey = CacheKeys.CategoriaPrefix + id;
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null) return null;

            _mapper.Map(categoriaDto, categoria);


            var catgoriaAtualizada = await _repository.UpdateAsync(categoria);
            await _cache.RemoveAsync(cachekey);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_KEY);

            return _mapper.Map<CategoriaResponseDTO>(catgoriaAtualizada);

        }
        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            string cachekey = CacheKeys.CategoriaPrefix + id;
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                return false;
            }

            await _repository.DeleteAsync(categoria);
            await _cache.RemoveAsync(cachekey);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_KEY);
            return true;
        }


    }
}