using Xunit;
using Catalogo.Application.DTOs;
using Catalogo.Application.Validators;

namespace Catalogo.Application.Tests;

public class ProductCreateDTOValidatorTests
{
    // Objeto que será testado. Pode ser criado no construtor (um por classe)
    // ou dentro de cada método (um por cenário). Aqui: um para todos os testes.
    private readonly ProductCreateDTOValidator _validator;

    public ProductCreateDTOValidatorTests()
    {
        _validator = new ProductCreateDTOValidator();
    }

    // [Fact] = "isto é um teste". O xUnit descobre e executa este método.
    [Fact]
    public void Validate_QuandoNomeVazio_DeveRetornarErro()
    {
        // ========== ARRANGE (preparar) ==========
        // Dados de entrada: DTO com nome vazio, como se viesse do usuário.
        var dto = new ProdutoCreateDTO
        {
            Nome = "",
            Preco = -10,
            Estoque = 5
        };

        // ========== ACT (agir) ==========
        // Chamamos o método que queremos testar. Só isso deve ser "a unidade".
        var result = _validator.Validate(dto);

        // ========== ASSERT (afirmar) ==========
        // Afirmamos: "espero que a validação tenha falhado".
        Assert.False(result.IsValid);
        // Afirmamos: "espero que exista erro na propriedade Nome".
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
    }

    [Fact]
    public void Validate_QuandoNomeMaiorQue100_DeveRetornarErro()
    {
        // Arrange: validador exige Length(3, 100) — 101 caracteres deve falhar
        var dto = new ProdutoCreateDTO
        {
            Nome = new string('A', 101), // Nome com 101 caracteres
            Preco = 10,
            Estoque = 5
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Nome");
    }

    [Theory]
    [InlineData("", 10, 5, "Nome")]              // Nome vazio
    [InlineData("Jose", -10, 5, "Preco")]         // Preço negativo
    [InlineData("Jose", 10, -5, "Estoque")]       // Estoque negativo
    public void Validate_QuandoDadoInvalido_DeveRetornarErroNaPropriedade(string nome, decimal preco, int quantidade, string propriedadeEsperada)
    {
        // Arrange: usa os parâmetros
        var dto = new ProdutoCreateDTO
        {
            Nome = nome,
            Preco = preco,
            Estoque = quantidade
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert: verifica os erros conforme os parâmetros
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == propriedadeEsperada);

    }

}
