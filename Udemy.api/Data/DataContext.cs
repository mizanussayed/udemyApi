using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Udemy.api.Models.Domain;

namespace Udemy.api.Data;
public class DataContext: IdentityDbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {       
    }
    public DbSet<ApplicationUser> ApplicationUsers {get; set;}
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; } 
}