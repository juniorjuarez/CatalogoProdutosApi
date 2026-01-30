using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Catalogo.Core.Entities;


[Table("Fornecedores")]
public class Fornecedor
{
    [Key]
    public int FornecedorId { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Phone]
    [Required]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data de Cadastro")]
    public DateTime DataCadastro { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ICollection<Produto>? Produtos { get; set; }
}
