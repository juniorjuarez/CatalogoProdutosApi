using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;



namespace Catalogo.Application.Services
{
    public class ProdutoService : IProdutoService
    {

        private readonly IProdutoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHybridCacheService _cache;

        

        public ProdutoService(IProdutoRepository repository, IMapper mapper, IHybridCacheService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;

        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosAsync()
        {

            string cacheKeyL1 = $"{CacheKeys.PRODUTOS_KEY}";
            string cacheKeyL2 = $"{CacheKeys.PRODUTOS_KEY}";

            var produtoDtos = await _cache.GetOrCreateAsync(
                cacheKeyL1,
                cacheKeyL2,
                factory: async () =>
            {
                var produtos = await _repository.GetAllAsync();

                if (produtos == null) return Enumerable.Empty<ProdutoResponseDTO>();

                return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

            },

                    absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,
                    absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2

      );


            return produtoDtos ?? Enumerable.Empty<ProdutoResponseDTO>();

        }
        public async Task<ProdutoResponseDTO?> GetProdutoByIdAsync(int id)
        {


            string cacheKeyL1 = $"{CacheKeys.ProdutoPrefix}{id}";
            string cacheKeyL2 = $"{CacheKeys.ProdutoPrefix}{id}";


            var produtoDto = await _cache.GetOrCreateAsync
                (
                    cacheKeyL1,
                    cacheKeyL2,
                    factory: async () =>
                        {
                            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
                            if (produto == null) return null;
                            return _mapper.Map<ProdutoResponseDTO>(produto);

                        },
                    absoluteExpirationL1: CacheKeys.ABSOLUTE_EXPIRATION_L1,
                    absoluteExpirationL2: CacheKeys.ABSOLUTE_EXPIRATION_L2
                  );

            return produtoDto;
        }

        public async Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produtoDTO)
        {

            var produto = _mapper.Map<Produto>(produtoDTO);
            var produtoSalvo = await _repository.CreateAsync(produto);
            await InvalidateProductCacheAsync();
            return _mapper.Map<ProdutoResponseDTO>(produtoSalvo);
        }
        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO)
        {

            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

            if (produto == null) return null;

            _mapper.Map(produtoDTO, produto);

            var produtoAtualizado = await _repository.UpdateAsync(produto);
            await InvalidateProductCacheAsync(id);


            return _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);

        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {

            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto == null) return false;


            await _repository.DeleteAsync(produto);

            await InvalidateProductCacheAsync(id);

            return true;
        }
        private async Task InvalidateProductCacheAsync(int? id = null)
        {
            await _cache.RemoveAsync(CacheKeys.PRODUTOS_KEY, CacheKeys.PRODUTOS_KEY);

            await _cache.RemoveAsync(CacheKeys.CATEGORIAS_PRODUTOS_KEY, CacheKeys.CATEGORIAS_PRODUTOS_KEY);

            if (id.HasValue)
            {
                string cacheKeyL1 = $"{CacheKeys.ProdutoPrefix}{id}";
                string cacheKeyL2 = $"{CacheKeys.ProdutoPrefix}{id}";

                await _cache.RemoveAsync(cacheKeyL1, cacheKeyL2);
            }
        }
    }
}