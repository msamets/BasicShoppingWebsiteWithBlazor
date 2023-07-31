using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using NLog;
using NLog.Web;
using ShopOnline.Api.Data;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Repositories.Contracts;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContextPool<ShopOnlineDbcontext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ShopOnlineConnection"))
    );

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IShoppingCartRepostiory, ShoppingCartRepository>();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors(policy =>
        policy.WithOrigins("http://localhost:7266", "https://localhost:7266")
            .AllowAnyMethod()
            .WithHeaders(HeaderNames.ContentType)
    );

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex);
    throw;
}
finally
{
    LogManager.Shutdown();
}