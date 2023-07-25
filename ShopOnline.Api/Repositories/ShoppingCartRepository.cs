﻿using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepostiory
    {
        private readonly ShopOnlineDbcontext shopOnlineDbcontext;
        public ShoppingCartRepository(ShopOnlineDbcontext shopOnlineDbcontext)
        {
            this.shopOnlineDbcontext = shopOnlineDbcontext;
        }

        private async Task<bool> CartItemsExists(int cartId, int productId)
        {
            return await this.shopOnlineDbcontext.CartItems.AnyAsync(c => c.CartId == cartId &&
                                                                     c.ProductId == productId);
        }
        public async Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            if(await CartItemsExists(cartItemToAddDto.CartId, cartItemToAddDto.ProductId) == false)
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
                    return result.Entity;
                }

            }

            return null;
        }

        public async Task<CartItem> DeleteItem(int id)
        {
            var item = await this.shopOnlineDbcontext.CartItems.FindAsync(id);

            if(item != null)
            {
                this.shopOnlineDbcontext.CartItems.Remove(item);
                await this.shopOnlineDbcontext.SaveChangesAsync();
            }

            return item;
        }

        public async Task<CartItem> GetItem(int id)
        {
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
            var item = await this.shopOnlineDbcontext.CartItems.FindAsync(id);

            if(item != null )
            {
                item.Qty = cartItemQtyUpdateDto.Qty;
                await this.shopOnlineDbcontext.SaveChangesAsync();
                return item;
            }

            return null;
        }
    }
}
