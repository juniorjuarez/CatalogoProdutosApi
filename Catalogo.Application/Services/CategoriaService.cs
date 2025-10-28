using AutoMapper;
using Catalogo.Application.DTOs;
using Catalogo.Core.Entities;
using Catalogo.Core.Interfaces;
namespace Catalogo.Application.Services
{
    public class CategoriaService : ICategoriaService
    {

        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;

        public CategoriaService(ICategoriaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<CategoriaResponseDTO>> GetCategoriasAsync()
        {
            var categorias = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaResponseDTO>>(categorias);
        }
        public async Task<CategoriaResponseDTO?> GetCategoriaByIdAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);
            return _mapper.Map<CategoriaResponseDTO>(categoria);
        }

        public async Task<IEnumerable<CategoriaResponseProdutosDTO?>> GetCategoriasProdutosAsync()
        {
            var categorias = await _repository.GetCategoriasProdutosAsync();
            return _mapper.Map<IEnumerable<CategoriaResponseProdutosDTO>>(categorias);
        }


        public async Task<CategoriaResponseDTO> CreateCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            var categoriaSalva = await _repository.CreateAsync(categoria);
            return _mapper.Map<CategoriaResponseDTO>(categoriaSalva);
        }
        public async Task<CategoriaResponseDTO> UpdateCategoriaAsync(int id, CategoriaCreateDTO categoriaDto)
        {
            var categoria = await _repository.GetByIdAsync(c => c.CategoriaId == id);

            if (categoria == null) return null;

            _mapper.Map(categoriaDto, categoria);


            var catgoriaAtualizada = await _repository.UpdateAsync(categoria);

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
            return true;
        }


    }
}