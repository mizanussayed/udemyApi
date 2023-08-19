using Udemy.api.Models.Domain;
namespace Udemy.api.Repositories;
public interface IProductRepository
{
    Task<List<Product>> GetAll(int pageNo =1, int pageSize = 10);
    Task<Product?> GetProduct(int id);
    Task<Product> Create(Product product);
    Task<Product?> Update (int id, Product product);
    Task<Product?> Delete(int id);
}