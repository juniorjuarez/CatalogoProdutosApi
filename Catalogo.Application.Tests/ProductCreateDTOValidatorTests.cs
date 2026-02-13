using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Catalogo.Application.DTOs;
using Catalogo.Application.Validators;


namespace Catalogo.Application.Tests
{
    public class ProductCreateDTOValidatorTests
    {

        private readonly ProductCreateDTOValidator _validator;

        public ProductCreateDTOValidatorTests()
        {
            _validator = new ProductCreateDTOValidator();
        }


        [Fact]
        public void Validate_QuandoNomeVazio_DeveRetornarErro()
        {

            // Arrange (preparar)
            var dto = new ProdutoCreateDTO
            {
                Nome = "",
                Preco = -10,
                Estoque = 5


            };

            // Act (agir)

            var result = _validator.Validate(dto);

            // Assert (afirmar): nome vazio = validação deve FALHAR
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Nome");

        }

        [Fact]

        public void Validate_QuandoPrecoNegativo_DeveRetornarErro()
        {
            // Arranging
            var dto2 = new ProdutoCreateDTO
            {
                Nome = "Produto Teste",
                Preco = -100,
                Estoque = 5
            };

            //Act

            var result2 = _validator.Validate(dto2);

            // Assert: preço negativo = validação deve FALHAR
            Assert.False(result2.IsValid);
            Assert.Contains(result2.Errors, e => e.PropertyName == "Preco");
        }

        [Fact]
        public void Validate_QuandoDtoValido_DeveRetornarIsValidTrue()
        {
            // Arrange: DTO dentro das regras (Nome 3–100 chars, Preco > 0, Estoque >= 0)
            var dto = new ProdutoCreateDTO
            {
                Nome = "Produto Teste",
                Preco = 100,
                Estoque = 5
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert: validação deve passar, sem erros
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_QuandoNomeComMenosDeTresCaracteres_DeveRetornarErro()
        {
            // Arrange: Nome com 2 caracteres (validador exige Length(3, 100))
            var dto = new ProdutoCreateDTO
            {
                Nome = "Ab",
                Preco = 10,
                Estoque = 0
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert: validação deve falhar, erro em Nome
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
        }

        [Fact]
        public void Validate_QuandoEstoqueNegativo_DeveRetornarErro()
        {
            // Arrange: Estoque negativo (validador exige >= 0)
            var dto = new ProdutoCreateDTO
            {
                Nome = "Produto Ok",
                Preco = 10,
                Estoque = -1
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert: validação deve falhar, erro em Estoque
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Estoque");
        }
    }
}