using Catalogo.Application.DTOs;
using Catalogo.Application.Services;
using Microsoft.AspNetCore.Mvc;



namespace CatalogoProdutos.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _service;


        public ProdutosController(IProdutoService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoResponseDTO>>> Get()
        {



            var produtos = await _service.GetProdutosAsync();

            if (!produtos.Any())
            {
                return NotFound("Nenhum produto encontrado!");
            }
            return Ok(produtos);

        }
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoResponseDTO>> Get(int id)
        {

            var produto = await _service.GetProdutoByIdAsync(id);

            if (produto == null)
            {
                return NotFound("Nenhum produto encontrado!");
            }

            return Ok(produto);


        }

        [HttpPost]
        public async Task<ActionResult> Post(ProdutoCreateDTO produtoDTO)
        {


            if (produtoDTO is null)
            {
                return BadRequest();
            }

            var produtoResponseDTO = await _service.CreateProdutoAsync(produtoDTO);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoResponseDTO.ProdutoId }, produtoResponseDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ProdutoCreateDTO produtoCreateDTO)
        {


            if (produtoCreateDTO is null) return BadRequest("Dados inválidos.");

            var produtoResponseDTO = await _service.UpdateProdutoAsync(id, produtoCreateDTO);


            if (produtoResponseDTO == null) return NotFound("Produto não encontrado.");

            return Ok(produtoResponseDTO);

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {




            var produto = await _service.DeleteProdutoAsync(id);
            if (!produto)
            {
                return NotFound("Nenhum produto encontrado!");
            }
            return Ok();



        }


    }
}