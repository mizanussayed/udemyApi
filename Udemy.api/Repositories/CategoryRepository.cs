using Microsoft.EntityFrameworkCore;
using Udemy.api.Data;
using Udemy.api.Models.Domain;

namespace Udemy.api.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _dataContext;
    public CategoryRepository(DataContext dataContext) => _dataContext = dataContext;

    public async Task<Category> Create(Category category)
    {
        await _dataContext.Categories.AddAsync(category);
        await _dataContext.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> Delete(int id)
    {
        var deletedCat = await _dataContext.Categories.FindAsync(id);
        if (deletedCat is not null)
        {
            _dataContext.Categories.Remove(deletedCat);
            await _dataContext.SaveChangesAsync();
            return deletedCat;
        }
        else
        {
            return null;
        }
    }

    public async Task<List<Category>> GetAll()
    {
        return await _dataContext.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategory(int id)
    {
        return await _dataContext.Categories.Where(c => c.Id == id).Include("Products").FirstOrDefaultAsync();
    }

    public async Task<Category?> Update(int id, Category category)
    {
        var cat = await _dataContext.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        if (cat is not null)
        {
            _dataContext.Entry(cat).CurrentValues.SetValues(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }
        return cat;
    }
}