using Udemy.api.Models.Domain;
namespace Udemy.api.Repositories;
public interface ICategoryRepository
{
    Task<List<Category>> GetAll();
    Task<Category?> GetCategory(int id);
    Task<Category> Create(Category category);
    Task<Category?> Update (int id, Category Category);
    Task<Category?> Delete(int id);
}