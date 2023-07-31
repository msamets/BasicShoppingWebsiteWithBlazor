using Blazored.Toast.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic.FileIO;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services;
using ShopOnline.Web.Services.Contracts;
using System.Reflection;

namespace ShopOnline.Web.Pages
{
    public class AddProductBase: ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

        [Inject]
        public SweetAlertService alertService { get; set; }

        public AddProductDto Product { get; set; }

        public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Product = new AddProductDto()
            {
                Qty= 1,
                Price= 1,
            };

            ProductCategories = await ProductService.GetProductCategories();
        }

        protected async Task AddProduct_Click()
        {
            
            try
            {
                foreach (var category in ProductCategories)
                {
                    if(category.Id == Product.CategoryId)
                    {
                        Product.ProductCategory = category;
                    }
                }

                await this.ProductService.AddProduct(Product);

                await ManageProductsLocalStorageService.RemoveCollection();

                await alertService.FireAsync("Successfull", "Product added successfully", "success");
            }
            catch (Exception ex)
            {
                await alertService.FireAsync("Error", ex.Message, "error");
            }
        }
    }
}
