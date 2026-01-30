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

        public TimeSpan cacheExpirationL1 =  CacheKeys.ABSOLUTE_EXPIRATION_L1;
        public TimeSpan cacheExpirationL2 = CacheKeys.ABSOLUTE_EXPIRATION_L2;

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
                var produtos =  await _repository.GetAllAsync();

                if (produtos == null) return Enumerable.Empty<ProdutoResponseDTO>();

                return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

            },

                cacheExpirationL1,
                cacheExpirationL2

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
                cacheExpirationL1,
                cacheExpirationL2
                  );

            return produtoDto;
        }

        public async Task<ProdutoResponseDTO> CreateProdutoAsync(ProdutoCreateDTO produtoDTO)
        {

            string cacheKeyL1 = $"{CacheKeys.PRODUTOS_KEY}";
            string cacheKeyL2 = $"{CacheKeys.PRODUTOS_KEY}";

            var produto = _mapper.Map<Produto>(produtoDTO);
            var produtoSalvo = await _repository.CreateAsync(produto);
            await _cache.RemoveAsync(cacheKeyL1, cacheKeyL2);
            return _mapper.Map<ProdutoResponseDTO>(produtoSalvo);
        }
        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO)
        {
            string cacheKeyL1ProductId = $"{CacheKeys.ProdutoPrefix}{id}";
            string cacheKeyL2ProductId = $"{CacheKeys.ProdutoPrefix}{id}";

            string cacheKeyL1ProductAll = $"{CacheKeys.PRODUTOS_KEY}";
            string cacheKeyL2ProductAll = $"{CacheKeys.PRODUTOS_KEY}";

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";


            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

            if (produto == null) return null;

            _mapper.Map(produtoDTO, produto);

            var produtoAtualizado = await _repository.UpdateAsync(produto);
            await _cache.RemoveAsync(cacheKeyL1ProductId, cacheKeyL2ProductId);
            await _cache.RemoveAsync(cacheKeyL1ProductAll, cacheKeyL2ProductAll);
            await _cache.RemoveAsync(cacheKeyL1Category, cacheKeyL2Category);


            return _mapper.Map<ProdutoResponseDTO>(produtoAtualizado);

        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            string cacheKeyL1ProductId = $"{CacheKeys.ProdutoPrefix}{id}";
            string cacheKeyL2ProductId = $"{CacheKeys.ProdutoPrefix}{id}";

            string cacheKeyL1ProductAll = $"{CacheKeys.PRODUTOS_KEY}";
            string cacheKeyL2ProductAll = $"{CacheKeys.PRODUTOS_KEY}";

            string cacheKeyL1Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";
            string cacheKeyL2Category = $"{CacheKeys.CATEGORIAS_PRODUTOS_KEY}";

            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto == null) return false;


            await _repository.DeleteAsync(produto);

            await _cache.RemoveAsync(cacheKeyL1ProductId, cacheKeyL2ProductId);
            await _cache.RemoveAsync(cacheKeyL1ProductAll, cacheKeyL2ProductAll);
            await _cache.RemoveAsync(cacheKeyL1Category, cacheKeyL2Category);

            return true;
        }
    }
}