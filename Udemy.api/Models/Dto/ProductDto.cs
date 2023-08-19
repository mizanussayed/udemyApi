using System.ComponentModel.DataAnnotations;

namespace Udemy.api.Models.Dto;
public class ProductDto
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }
    public IFormFile? ImageFile { get; set; }
    public double Price { get; set; }
    public int CategoryId { get; set; }
}

public class ProductResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ImgUrl { get; set; }
    public double Price { get; set; }
    public int CategoryId { get; set; }
}