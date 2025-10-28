using AutoMapper;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;

namespace Catalogo.Application.Services
{
    public class ProdutoService : IProdutoService
    {


        private readonly IProdutoRepository _repository;
        private readonly IMapper _mapper;

        public ProdutoService(IProdutoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProdutoResponseDTO>> GetProdutosAsync()
        {
            var produtos = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProdutoResponseDTO>>(produtos);

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

            return _mapper.Map<ProdutoResponseDTO>(produtoSalvo);
        }
        public async Task<ProdutoResponseDTO> UpdateProdutoAsync(int id, ProdutoCreateDTO produtoDTO)
        {
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);

            if (produto == null) return null;

            _mapper.Map(produtoDTO, produto);

            var produtoAtualizado = await _repository.UpdateAsync(produto);

            return _mapper.Map<ProdutoResponseDTO>(produto);

        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _repository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto == null) return false;

            await _repository.DeleteAsync(produto);
            return true;
        }
    }
}