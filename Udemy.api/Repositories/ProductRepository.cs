using Microsoft.EntityFrameworkCore;
using Udemy.api.Models.Domain;
using Udemy.api.Data;

namespace Udemy.api.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly DataContext _dataContext;
    public ProductRepository(DataContext dataContext) => _dataContext = dataContext;
    public async Task<Product> Create(Product product)
    {
        await _dataContext.AddAsync(product);
        await _dataContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> Delete(int id)
    {
        var deletedProd = await _dataContext.Products.FindAsync(id);
        if (deletedProd is not null)
        {
            _dataContext.Products.Remove(deletedProd);
           await _dataContext.SaveChangesAsync();
            return deletedProd;
        }
        else
        {
            return null;
        }
    }

    public async Task<List<Product>> GetAll(int pageNo = 1, int pageSize = 10)
    {
        var product = _dataContext.Products.AsQueryable();
        return await product.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Product?> GetProduct(int id)
    {
        return await _dataContext.Products.FindAsync(id);
    }

    public async Task<Product?> Update(int id, Product product)
    {
        var prod = await _dataContext.Products.FirstOrDefaultAsync(c => c.Id == id);
        if (prod is not null)
        {
             _dataContext.Entry(prod).CurrentValues.SetValues(product);
            await _dataContext.SaveChangesAsync();
            return product;
        }
        else
        {
        return prod;
        }
    }
}