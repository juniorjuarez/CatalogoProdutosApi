using AutoMapper;
using Catalogo.Application.Constants;
using Catalogo.Application.DTOs;
using Catalogo.Application.Interfaces;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;

namespace Catalogo.Application.Services
{
    public class FornecedorService : IFornecedorService
    {

        private readonly IFornecedorRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHybridCacheService _cache;

        public TimeSpan cacheExpirationL1 = CacheKeys.ABSOLUTE_EXPIRATION_L1;
        public TimeSpan cacheExpirationL2 = CacheKeys.ABSOLUTE_EXPIRATION_L2;


        public FornecedorService(IFornecedorRepository repository, IMapper mapper, IHybridCacheService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }




        public async Task<IEnumerable<FornecedorResponseDTO>> GetFornecedorAsync()
        {
            string cacheKeyL1 = $"{CacheKeys.Fornecedores_Key}";
            string cacheKeyL2 = $"{CacheKeys.Fornecedores_Key}";

            var fornecedoresDtos = await _cache.GetOrCreateAsync
            (
                cacheKeyL1,
                cacheKeyL2,
                factory: async () =>
                {
                    var fornecedores = await _repository.GetAllAsync();

                    if (fornecedores == null) return Enumerable.Empty<FornecedorResponseDTO>();

                    return _mapper.Map<IEnumerable<FornecedorResponseDTO>>(fornecedores);

                },
                cacheExpirationL1,
                cacheExpirationL2
            );

            return fornecedoresDtos ?? Enumerable.Empty<FornecedorResponseDTO>();
        }

        public async Task<FornecedorResponseDTO?> GetFornecedorByIdAsync(int id)
        {
            string cacheKeyL1 = $"{CacheKeys.FornecedorPrefix}{id}";
            string cacheKeyL2 = $"{CacheKeys.FornecedorPrefix}{id}";

            var fornecedorDto = await _cache.GetOrCreateAsync
            (
                cacheKeyL1,
                cacheKeyL2,
                factory: async () =>
                {
                    var fornecedor = await _repository.GetByIdAsync(f => f.FornecedorId == id);
                    if (fornecedor == null) return null;
                    return _mapper.Map<FornecedorResponseDTO>(fornecedor);
                },
                cacheExpirationL1,
                cacheExpirationL2
            );

            return fornecedorDto;
        }

        public async Task<IEnumerable<FornecedorResponseProdutoDTO>> GetFornecedorResponseProdutosAsync()
        {
            string cacheKeyL1 = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";
            string cacheKeyL2 = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";

            var fornecedoresDto = await _cache.GetOrCreateAsync
            (
                cacheKeyL1,
                cacheKeyL2,
                factory: async () =>
                {
                    var fornecedoresProdutos = await _repository.GetFornecedoresProdutosAsync();
                    if (fornecedoresProdutos == null) return Enumerable.Empty<FornecedorResponseProdutoDTO>();
                    return _mapper.Map<IEnumerable<FornecedorResponseProdutoDTO>>(fornecedoresProdutos);
                },
                cacheExpirationL1,
                cacheExpirationL2
            );
            return fornecedoresDto;
        }
        public async Task<FornecedorResponseDTO> CreateFornecedorAsync(FornecedorCreateDTO fornecedorDto)
        {
            string cacheKeyFornecedoresAllL1 = $"{CacheKeys.Fornecedores_Key}";
            string cacheKeyFornecedoresAllL2 = $"{CacheKeys.Fornecedores_Key}";

            string cacheKeyL1Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";
            string cacheKeyL2Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorDto);

            var fornecedorSalvo = await _repository.CreateAsync(fornecedor);
            await _cache.RemoveAsync(cacheKeyFornecedoresAllL1, cacheKeyFornecedoresAllL2);
            await _cache.RemoveAsync(cacheKeyL1Fornecedor, cacheKeyL2Fornecedor);

            return _mapper.Map<FornecedorResponseDTO>(fornecedorSalvo);
        }

        public async Task<FornecedorResponseDTO?> UpdateFornecedorAsync(int id, FornecedorCreateDTO fornecedorDto)
        {
            string cacheKeyl1 = $"{CacheKeys.FornecedorPrefix}{id}";
            string cacheKeyl2 = $"{CacheKeys.FornecedorPrefix}{id}";

            string cacheKeyFornecedoresAllL1 = $"{CacheKeys.Fornecedores_Key}";
            string cacheKeyFornecedoresAllL2 = $"{CacheKeys.Fornecedores_Key}";

            string cacheKeyL1Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";
            string cacheKeyL2Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";

            var fornecedor = await _repository.GetByIdAsync(f => f.FornecedorId == id);

            if (fornecedor == null) return null;

            _mapper.Map(fornecedorDto, fornecedor);

            var fornecedorAtualizado = await _repository.UpdateAsync(fornecedor);

            await _cache.RemoveAsync(cacheKeyFornecedoresAllL1, cacheKeyFornecedoresAllL2);
            await _cache.RemoveAsync(cacheKeyL1Fornecedor, cacheKeyL2Fornecedor);
            await _cache.RemoveAsync(cacheKeyl1, cacheKeyl2);

            return _mapper.Map<FornecedorResponseDTO>(fornecedorAtualizado);
        }

        public async Task<bool> DeleteFornecedorAsync(int id)
        {
            string cacheKeyl1 = $"{CacheKeys.FornecedorPrefix}{id}";
            string cacheKeyl2 = $"{CacheKeys.FornecedorPrefix}{id}";

            string cacheKeyFornecedoresAllL1 = $"{CacheKeys.Fornecedores_Key}";
            string cacheKeyFornecedoresAllL2 = $"{CacheKeys.Fornecedores_Key}";

            string cacheKeyL1Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";
            string cacheKeyL2Fornecedor = $"{CacheKeys.FORNECEDORES_PRODUTOS_KEY}";

            var fornecedor = await _repository.GetByIdAsync(f => f.FornecedorId == id);

            if (fornecedor == null) return false;

            await _repository.DeleteAsync(fornecedor);
            await _cache.RemoveAsync(cacheKeyFornecedoresAllL1, cacheKeyFornecedoresAllL2);
            await _cache.RemoveAsync(cacheKeyL1Fornecedor, cacheKeyL2Fornecedor);
            await _cache.RemoveAsync(cacheKeyl1, cacheKeyl2);

            return true;
        }
    }
}