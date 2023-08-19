namespace Udemy.api.Models.Domain;
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ImgUrl { get; set; }
    public double Price {get; set;}
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}