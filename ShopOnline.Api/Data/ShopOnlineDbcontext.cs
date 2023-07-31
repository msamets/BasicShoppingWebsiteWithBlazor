using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Entities;

namespace ShopOnline.Api.Data
{
    public class ShopOnlineDbcontext:DbContext
    {
        public ShopOnlineDbcontext(DbContextOptions<ShopOnlineDbcontext> options): base(options) 
        {
            
        }

        
    
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<User> Users { get; set; }

    
    }
}
