using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;


namespace Catalogo.Application.Services
{



    public class CategoriaService : ICategoriaService
    {

        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHybridCacheService _cache;


        public CategoriaService(ICategoriaRepository repository, IMapper mapper, IHybridCacheService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }


        public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
        {

            string cacheKeyL1 = $"{CacheKeys.CATEGORIAS_KEY}";
            string cacheKeyL2 = $"{CacheKeys.CATEGORIAS_KEY}";

            var categoriasDtos = await _cache.GetOrCreateAsync
            (
                cacheKeyL1,
                cacheKeyL2,
                factory: async () =>
                {
                    var categorias = await _repository.GetAllAsync();

                    if (categorias == null) return Enumerable.Empty<CategoriaResponseDTO>();

                    return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);

                },
                absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,
                absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2
             );


            return categoriasDtos ?? Enumerable.Empty<CategoriaResponseDTO>();

        }
        public async Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id)
        {

            string cacheKeyL1 = $"{CacheKeys.CategoriaPrefix}{id}";
            string cacheKeyL2 = $"{CacheKeys.CategoriaPrefix}{id}";


            var categoriasDto = await _cache.GetOrCreateAsync
                (
                    cacheKeyL1,
                    cacheKeyL2,
                    factory: async () =>

                        {
                            var categorias = await _repository.GetByIdAsync(c => c.CategoriaId == id);
                            if (categorias == null) return null;
                            return _mapper.Map<CategoriaResponseDTO>(categorias);

                        },
                absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,
                absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2
                   );

            return categoriasDto;
        }

        public async Task<IEnumerable<CategoriaResponseProdutosDTO?>> GetCategoriasProdutosAsync()
        {

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";

            var categoriasProdutoDto = await _cache.GetOrCreateAsync
                (

                    cacheKeyL1Category,
                    cacheKeyL2Category,
                    factory: async () =>

                    {
                        var categoriasProdutos = await _repository.GetCategoriasProdutosAsync();
                        if (categoriasProdutos == null) return Enumerable.Empty<CategoriaResponseProdutosDTO>();
                        return _mapper.Map<IEnumerable<CategoriaResponseProdutosDTO>>(categoriasProdutos);
                    },

                    absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,
                    absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2
                );

            return categoriasProdutoDto;
        }


        public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaSalva = await _repository.CreateAsync(categoria);
            await InvalidateCategoryCacheAsync();

            return _mapper.Map<CategoriaResponseDTO>(categoriaSalva);
        }
        public async Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto)
        {

            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null) return null;

            _mapper.Map(categoriaDto, categoria);


            var catgoriaAtualizada = await _repository.UpdateAsync(categoria);
            await InvalidateCategoryCacheAsync(id);

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
            await InvalidateCategoryCacheAsync(id);
            return true;
        }
        private async Task InvalidateCategoryCacheAsync(int? id = null)
        {
            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_KEY, CacheKeys.CATEGORIAS_KEY);

            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY, CacheKeys.CATEGORIAS_PRODUTOS_KEY);

            if (id.HasValue)
            {
                string cacheKeyL1 = $"{CacheKeys.CategoriaPrefix}{id}";
                string cacheKeyL2 = $"{CacheKeys.CategoriaPrefix}{id}";

                await _cache.RemoveAsync(cacheKeyL1, cacheKeyL2);
            }
        }

    }
}