using System.ComponentModel.DataAnnotations;

namespace Vitalia.Application.DTOs.Category;

public class CategoryRequest
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(
        100,
        ErrorMessage = "O nome da categoria deve ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(
        500,
        ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Description { get; set; }
}