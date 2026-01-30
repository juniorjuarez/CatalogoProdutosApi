using Catalogo.Application.DTOs;
using FluentValidation;

namespace Catalogo.Application.Validators
{
    public class CategoryCreateDTOValidator : AbstractValidator<CategoriaCreateDTO>
    {
        public CategoryCreateDTOValidator()
        {
            RuleFor(c => c.Nome).NotEmpty().WithMessage("O nome da categoria é obrigatório.").Length(3, 10).WithMessage("O nome deve ter entre 3 e 100 caracteres.");


        }
    }
}
