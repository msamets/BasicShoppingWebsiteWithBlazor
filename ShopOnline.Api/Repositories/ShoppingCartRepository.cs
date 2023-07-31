using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepostiory
    {
        private readonly ShopOnlineDbcontext shopOnlineDbcontext;

        private readonly ILogger<ProductRepository> logger;
        public ShoppingCartRepository(ShopOnlineDbcontext shopOnlineDbcontext, ILogger<ProductRepository> logger)
        {
            this.shopOnlineDbcontext = shopOnlineDbcontext;
            this.logger = logger;
            logger.LogDebug("NLog is integrated to Product Repository");
        }

        private async Task<bool> CartItemsExists(int cartId, int productId)
        {
            logger.LogInformation("GetCategory method called");
            return await this.shopOnlineDbcontext.CartItems.AnyAsync(c => c.CartId == cartId &&
                                                                     c.ProductId == productId);
        }
        public async Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            logger.LogInformation("AddItem method called");

            if (await CartItemsExists(cartItemToAddDto.CartId, cartItemToAddDto.ProductId) == false)
            {
                var item = await (from product in this.shopOnlineDbcontext.Products
                                  where product.Id == cartItemToAddDto.ProductId
                                  select new CartItem
                                  {
                                      CartId = cartItemToAddDto.CartId,
                                      ProductId = cartItemToAddDto.ProductId,
                                      Qty = cartItemToAddDto.Qty
                                  }).SingleOrDefaultAsync();
                
                if (item != null)
                {
                    var result = await this.shopOnlineDbcontext.CartItems.AddAsync(item);
                    await this.shopOnlineDbcontext.SaveChangesAsync();

                    logger.LogInformation("AddItem method executed");

                    return result.Entity;
                }

            }
            logger.LogWarning("AddItem method can't executed");


            return null;
        }

        public async Task<CartItem> DeleteItem(int id)
        {
            logger.LogInformation("DeleteItem method called");

            var item = await this.shopOnlineDbcontext.CartItems.FindAsync(id);

            if(item != null)
            {
                this.shopOnlineDbcontext.CartItems.Remove(item);
                await this.shopOnlineDbcontext.SaveChangesAsync();
            }

            logger.LogInformation("DeleteItem method executed");

            return item;
        }

        public async Task<CartItem> GetItem(int id)
        {
            logger.LogInformation("GetItem method called");

            return await (from cart in this.shopOnlineDbcontext.Carts
                          join cartItem in this.shopOnlineDbcontext.CartItems
                          on cart.Id equals cartItem.CartId
                          where cartItem.Id == id
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>> GetItems(int userId)
        {
            logger.LogInformation("GetItems method called");

            return await (from cart in this.shopOnlineDbcontext.Carts
                          join cartItem in this.shopOnlineDbcontext.CartItems
                          on cart.Id equals cartItem.CartId
                          where cart.UserId == userId
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).ToListAsync();
        }

        public async Task<CartItem> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            logger.LogInformation("UpdateQty method called");

            var item = await this.shopOnlineDbcontext.CartItems.FindAsync(id);

            if(item != null )
            {
                item.Qty = cartItemQtyUpdateDto.Qty;
                await this.shopOnlineDbcontext.SaveChangesAsync();

                logger.LogInformation("UpdateQty method executed");

                return item;
            }

            logger.LogWarning("UpdateQty method can't executed");


            return null;
        }
    }
}
