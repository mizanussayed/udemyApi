namespace Udemy.api.Models.Domain;
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public virtual List<Product> Products {get;set;} = new();
}