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

        public TimeSpan cacheExpirationL1 = CacheKeys.ABSOLUTE_EXPIRATION_L1;
        public TimeSpan cacheExpirationL2 = CacheKeys.ABSOLUTE_EXPIRATION_L2;

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
                cacheExpirationL1,
                cacheExpirationL2
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
                cacheExpirationL1,
                cacheExpirationL2
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

                    absoluteExpirationL1: TimeSpan.FromMinutes(5),
                    absoluteExpirationL2: TimeSpan.FromMinutes(30)
                );

            return categoriasProdutoDto;
        }


        public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {


            string cacheKeyCategoriasAllL1 = $"{CacheKeys.CATEGORIAS_KEY}";
            string cacheKeyCategoriasAllL2 = $"{CacheKeys.CATEGORIAS_KEY}";

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";



            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaSalva = await _repository.CreateAsync(categoria);
            await _cache.RemoveAsync(cacheKeyCategoriasAllL1, cacheKeyCategoriasAllL2);
            await _cache.RemoveAsync(cacheKeyL1Category, cacheKeyL2Category);

            return _mapper.Map<CategoriaResponseDTO>(categoriaSalva);
        }
        public async Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto)
        {


            string cacheKeyL1 = $"{CacheKeys.CategoriaPrefix}{id}";
            string cacheKeyL2 = $"{CacheKeys.CategoriaPrefix}{id}";
            string cacheKeyCategoriasAllL1 = $"{CacheKeys.CATEGORIAS_KEY}";
            string cacheKeyCategoriasAllL2 = $"{CacheKeys.CATEGORIAS_KEY}";

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";

            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null) return null;

            _mapper.Map(categoriaDto, categoria);


            var catgoriaAtualizada = await _repository.UpdateAsync(categoria);
            await _cache.RemoveAsync(cacheKeyL1, cacheKeyL2);
            await _cache.RemoveAsync(cacheKeyCategoriasAllL1, cacheKeyCategoriasAllL2);
            await _cache.RemoveAsync(cacheKeyL1Category, cacheKeyL2Category);

            return _mapper.Map<CategoriaResponseDTO>(catgoriaAtualizada);

        }
        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            string cacheKeyL1 = $"{CacheKeys.CategoriaPrefix}{id}";
            string cacheKeyL2 = $"{CacheKeys.CategoriaPrefix}{id}";
            string cacheKeyCategoriasAllL1 = $"{CacheKeys.CATEGORIAS_KEY}";
            string cacheKeyCategoriasAllL2 = $"{CacheKeys.CATEGORIAS_KEY}";

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null)
            {
                return false;
            }

            await _repository.DeleteAsync(categoria);
            await _cache.RemoveAsync(cacheKeyL1, cacheKeyL2);
            await _cache.RemoveAsync(cacheKeyCategoriasAllL1, cacheKeyCategoriasAllL2);
            await _cache.RemoveAsync(cacheKeyL1Category, cacheKeyL2Category);
            return true;
        }


    }
}