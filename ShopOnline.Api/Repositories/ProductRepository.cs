using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;

namespace ShopOnline.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopOnlineDbcontext shopOnlineDbcontext;
        public ProductRepository(ShopOnlineDbcontext shopOnlineDbcontext)
        {
            this.shopOnlineDbcontext = shopOnlineDbcontext;
        }
        public async Task<ProductCategory> GetCategory(int id)
        {
            var category = await shopOnlineDbcontext.ProductCategories.SingleOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task<Product> GetItem(int id)
        {
            var product = await shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory)
                            .SingleOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetItems()
        {
            var products = await this.shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory).ToArrayAsync();
            
            return products;
        }

        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            var categories = await this.shopOnlineDbcontext.ProductCategories
                .ToListAsync();
            return categories;
        }

        public async Task<IEnumerable<Product>> GetItemsByCategory(int categoryId)
        {
            var products = await this.shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory)
                            .Where(p => p.CategoryId == categoryId)
                            .ToListAsync();
            return products;
        }
    }
}
