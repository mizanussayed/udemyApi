namespace Udemy.api.Models.Dto;

public class CategoryResponse {
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public List<ProductDto> Products {get; set; } = new ();
}