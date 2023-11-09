using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CatShopService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace Startup;
public class Startup
{

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<MongoDbSettings>(
            Configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoClient>(ServiceProvider =>
        {
            var settings = ServiceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(ServiceProvider =>
        {
            var client = ServiceProvider.GetRequiredService<IMongoClient>();
            var settings = ServiceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });


        // Регистрируйте ICatService и CatService в DI контейнере
        services.AddScoped<ICatService, CatService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage(); // Middleware для обработки исключений в режиме разработки
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            }); // Middleware для Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Настройка Swagger UI
            });
        }
        else
        {
            app.UseHsts(); // Middleware для настройки HTTP Strict Transport Security (HSTS) в продакшн среде
        }

        app.UseHttpsRedirection(); // Middleware для перенаправления на HTTPS
        app.UseStaticFiles(); // Middleware для обслуживания статических файлов (CSS, JS и др.)
        app.UseRouting(); // Middleware для маршрутизации запросов
        app.UseAuthentication(); // Middleware для аутентификации
        app.UseAuthorization(); // Middleware для авторизации

        IApplicationBuilder applicationBuilder = app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "CatController",
                pattern: "Classes/CatController/{action=Index}/{id?}",
                defaults: new { controller = "CatController" }
            );
        })
;       
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}