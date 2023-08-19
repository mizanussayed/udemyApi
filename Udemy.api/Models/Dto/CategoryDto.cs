using System.ComponentModel.DataAnnotations;

namespace Udemy.api.Models.Dto;
public class CategoryDto{
    public int Id { get; set;}
    [Required]
    [MinLength(3, ErrorMessage ="Name Must be at least {1} letter")]
    public required string Name { get; set;}
}
