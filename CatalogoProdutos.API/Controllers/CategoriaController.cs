using Catalogo.Application.DTOs;
using Catalogo.Application.Services;
using CatalogoProdutos.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace CatalogoProdutos.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;




        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaResponseDTO>>> Get()
        {

            var categoriasDTO = await _service.GetCategoriasAsync();

            if (!categoriasDTO.Any())
            {
                return NotFound("Nenhuma categoria encontrado!");
            }
            return Ok(categoriasDTO);



        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaResponseDTO>> Get(int id)
        {


            var categoriaDTO = await _service.GetCategoriaByIdAsync(id);

            if (categoriaDTO == null)
            {
                return NotFound("Nenhuma categoria encontrado!");
            }

            return Ok(categoriaDTO);


        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaResponseProdutosDTO>>> GetCategoriasProdutos()
        {


            var categoriasProdutosDTO = await _service.GetCategoriasProdutosAsync();
            return Ok(categoriasProdutosDTO);



        }

        [HttpPost]
        public async Task<ActionResult> Post(CategoriaCreateDTO categoriaDTO)
        {


            var categoriaResponseDTO = await _service.CreateCategoriaAsync(categoriaDTO);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaResponseDTO.CategoriaId }, categoriaResponseDTO);


        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CategoriaCreateDTO categoriaDTO)
        {

            var categoria = await _service.UpdateCategoriaAsync(id, categoriaDTO);

            if (categoria == null) return NotFound("Categoria não encontrada.");

            return Ok(categoria);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {


            var categoria = await _service.DeleteCategoriaAsync(id);
            if (!categoria) return NotFound("Nenhuma categoria encontrada!");

            return Ok();


        }
    }
}

