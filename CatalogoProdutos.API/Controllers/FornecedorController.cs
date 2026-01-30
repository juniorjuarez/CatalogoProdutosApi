using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Catalogo.Application.DTOs;
using Catalogo.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogoProdutos.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FornecedorController : Controller
    {
        private readonly IFornecedorService _service;

        public FornecedorController(IFornecedorService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorResponseDTO>>> Get()
        {
            try
            {
                var fornecedoresDto = await _service.GetFornecedorAsync();

                if (!fornecedoresDto.Any())
                {
                    return NotFound("Nenhum fornecedor encontrado");
                }

                return Ok(fornecedoresDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }

        [HttpGet("{id:int}", Name = "ObterFornecedor")]
        public async Task<ActionResult<FornecedorResponseDTO>> Get(int id)
        {
            try
            {
                var fornecedorDto = await _service.GetFornecedorByIdAsync(id);

                if (fornecedorDto == null)
                {
                    return NotFound("Nenhum fornecedor encontrado");
                }

                return Ok(fornecedorDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }


        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<FornecedorResponseProdutoDTO>>> GetFornecedoresProdutos()
        {
            try
            {
                var fornecedoresProdutosDto = await _service.GetFornecedorResponseProdutosAsync();
                return Ok(fornecedoresProdutosDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorCreateDTO>> Post(FornecedorCreateDTO fornecedorDto)
        {
            try
            {
                if (fornecedorDto is null)
                {
                    return BadRequest();
                }

                var FornecedorResponseDTO = await _service.CreateFornecedorAsync(fornecedorDto);

                return new CreatedAtRouteResult("ObterFornecedor", new { id = FornecedorResponseDTO.FornecedorId }, FornecedorResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, FornecedorCreateDTO fornecedorDto)
        {
            try
            {
                if (fornecedorDto == null) return BadRequest("Dados invalidos");

                var fornecedor = await _service.UpdateFornecedorAsync(id, fornecedorDto);

                if (fornecedor == null) return NotFound("Fornecedor não encontrado");

                return Ok(fornecedor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var fornecedor = await _service.DeleteFornecedorAsync(id);
                if (!fornecedor) return NotFound("Nenhum fornecedor encontrado");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizara solicitação: {ex}");
            }
        }
    }
}