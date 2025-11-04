using Catalogo.Application.DTOs;
using FluentValidation;


namespace Catalogo.Application.Validators
{
    public class ProductCreateDTOValidator : AbstractValidator<ProdutoCreateDTO>
    {
        public ProductCreateDTOValidator()
        {
            RuleFor(p => p.Nome).NotEmpty().WithMessage("O nome do produto é obrigatório.").Length(3, 10).WithMessage("O nome deve ter entre 3 e 100 caracteres.");

            RuleFor(p => p.Preco).GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

            RuleFor(p => p.Estoque).GreaterThanOrEqualTo(0).WithMessage("O estoque deve ser igual ou maior que zero.");
        }
    }
}
