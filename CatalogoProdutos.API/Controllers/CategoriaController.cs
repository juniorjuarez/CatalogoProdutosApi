using Catalogo.Application.DTOs;
using Catalogo.Application.Services;
using Microsoft.AspNetCore.Mvc;



namespace CatalogoProdutos.API.Controllers
{
    [Route("controller")]
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

            try
            {
                var categoriasDTO = await _service.GetCategoriasAsync();

                if (!categoriasDTO.Any())
                {
                    return NotFound("Nenhuma categoria encontrado!");
                }
                return Ok(categoriasDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");

            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaResponseDTO>> Get(int id)
        {

            try
            {

                var categoriaDTO = await _service.GetCategoriaByIdAsync(id);

                if (categoriaDTO == null)
                {
                    return NotFound("Nenhuma categoria encontrado!");
                }

                return Ok(categoriaDTO);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");

            }

        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaResponseProdutosDTO>>> GetCategoriasProdutos()
        {

            try
            {
                var categoriasProdutosDTO = await _service.GetCategoriasProdutosAsync();
                return Ok(categoriasProdutosDTO);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Post(CategoriaCreateDTO categoriaDTO)
        {

            try
            {
                if (categoriaDTO is null)
                {
                    return BadRequest();
                }

                var categoriaResponseDTO = await _service.CreateCategoriaAsync(categoriaDTO);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaResponseDTO.CategoriaId }, categoriaResponseDTO);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");

            }


        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CategoriaCreateDTO categoriaDTO)
        {

            try

            {
                if (categoriaDTO == null) return BadRequest("Dados inválidos.");

                var categoria = await _service.UpdateCategoriaAsync(id, categoriaDTO);

                if (categoria == null) return NotFound("Categoria não encontrada.");

                return Ok(categoria);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");

            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {

                var categoria = await _service.DeleteCategoriaAsync(id);
                if (!categoria) return NotFound("Nenhuma categoria encontrada!");

                return Ok();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao realizar a solicitação.");

            }

        }
    }
}

