using System.ComponentModel.DataAnnotations;

namespace Vitalia.Application.DTOs.Product;

public class ProductRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "A categoria deve ser informada.")]
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "O nome do produto deve ter entre 2 e 150 caracteres."
    )]
    public string Name { get; set; } = string.Empty;

    [StringLength(
        1000,
        ErrorMessage = "A descrição deve ter no máximo 1000 caracteres."
    )]
    public string? Description { get; set; }

    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "O preço deve ser maior que zero."
    )]
    public decimal Price { get; set; }

    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "O estoque não pode ser negativo."
    )]
    public int Stock { get; set; }
}