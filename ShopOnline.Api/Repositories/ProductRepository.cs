using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using NLog;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Entities.Validators;
using ShopOnline.Api.Repositories.Contracts;

namespace ShopOnline.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopOnlineDbcontext shopOnlineDbcontext;

        private readonly ILogger<ProductRepository> logger;
        public ProductRepository(ShopOnlineDbcontext shopOnlineDbcontext, ILogger<ProductRepository> logger)
        {
            this.shopOnlineDbcontext = shopOnlineDbcontext;
            this.logger = logger;
            logger.LogDebug("NLog is integrated to Product Repository");
        }
        public async Task<ProductCategory> GetCategory(int id)
        {
            logger.LogInformation("GetCategory method called");

            var category = await shopOnlineDbcontext.ProductCategories.SingleOrDefaultAsync(c => c.Id == id);

            logger.LogInformation("Get Category method executed");

            return category;
        }

        public async Task<Product> GetItem(int id)
        {
            logger.LogInformation("GetItem method called");

            var product = await shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory)
                            .SingleOrDefaultAsync(p => p.Id == id);

            logger.LogInformation("GetItem method executed");

            return product;
        }

        public async Task<IEnumerable<Product>> GetItems()
        {
            logger.LogInformation("GetItems method called");

            var products = await this.shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory).ToArrayAsync();

            logger.LogInformation("GetItems method executed");

            return products;
        }

        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            logger.LogInformation("GetCategories method called");

            var categories = await this.shopOnlineDbcontext.ProductCategories
                .ToListAsync();

            logger.LogInformation("GetCategories method executed");

            return categories;
        }

        public async Task<IEnumerable<Product>> GetItemsByCategory(int categoryId)
        {
            logger.LogInformation("GetItemsByCategory method called");

            var products = await this.shopOnlineDbcontext.Products
                            .Include(p => p.ProductCategory)
                            .Where(p => p.CategoryId == categoryId)
                            .ToListAsync();

            logger.LogInformation("GetItemsByCategory method executed");

            return products;
        }

        public async Task AddProduct(Product product)
        {
            logger.LogInformation("AddProduct method called");

            if (!(await shopOnlineDbcontext.ProductCategories.AnyAsync(c => c.Equals(product.ProductCategory) && c.Id == product.CategoryId)))
            {
                throw new BadHttpRequestException("Category is not exist");
            }
            product.ProductCategory = null;
            ProductValidator productValidator= new ProductValidator();

            string validationResultMessages = productValidator.Validate(product).ToString();

            if(validationResultMessages.IsNullOrEmpty())
            {
                await shopOnlineDbcontext.AddAsync<Product>(product);
                await shopOnlineDbcontext.SaveChangesAsync();

                logger.LogInformation("AddProduct method called");
            }
            else
            {
                logger.LogWarning(validationResultMessages);

                throw new BadHttpRequestException(validationResultMessages);
            }
        }
    }
}
