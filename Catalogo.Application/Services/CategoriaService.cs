using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Catalogo.Application.Services
{
    public class CategoriaService : ICategoriaService
    {

        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CategoriaService(ICategoriaRepository repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }


        public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
        {


            var categoriasDTO = await _cache.GetOrCreateAsync(CacheKeys.CATEGORIAS_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var categorias = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
            });

            return categoriasDTO;

        }
        public async Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);
            return _mapper.Map<CategoriaResponseDTO>(categoria);
        }

        public async Task<IEnumerable<CategoriaResponseProdutosDTO?>> GetCategoriasProdutosAsync()
        {

            var categoriasDTO = await _cache.GetOrCreateAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var categorias = await _repository.GetCategoriasProdutosAsync();
                return _mapper.Map<IEnumerable<CategoriaResponseProdutosDTO>>(categorias);
            });

            return categoriasDTO;
        }


        public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaSalva = await _repository.CreateAsync(categoria);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_KEY);
            return _mapper.Map<CategoriaResponseDTO>(categoriaSalva);
        }
        public async Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto)
        {
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null) return null;

            _mapper.Map(categoriaDto, categoria);


            var catgoriaAtualizada = await _repository.UpdateAsync(categoria);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_KEY);

            return _mapper.Map<CategoriaResponseDTO>(catgoriaAtualizada);

        }
        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                return false;
            }

            await _repository.DeleteAsync(categoria);
            _cache.Remove(CacheKeys.CATEGORIAS_PRODUTOS_KEY);
            _cache.Remove(CacheKeys.CATEGORIAS_KEY);
            return true;
        }


    }
}