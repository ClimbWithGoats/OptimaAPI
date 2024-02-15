
using OptimaAPI.Controllers;
using OptimaAPI.DB;
using OptimaAPI.Interfaces;
using OptimaAPI.Repositories;

namespace OptimaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at http://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c=>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title="Optima integrator", Version="v1" });
            });

            builder.Services.AddScoped<IContractorsRepository, ContractorsRepository>();
            builder.Services.AddScoped<IMerchandiseCardsRepository, MerchandiseCardsRepository>();
            builder.Services.AddScoped<ICommodityGoupsRepository, CommodityGoupsRepository>();
            builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            builder.Services.AddScoped<IResourcesRepository, ResourcesRepository>();
            builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
            builder.Services.AddScoped<IImportDocumentsRepository, ImportDocumentsRepository>();

            var app = builder.Build();
      
            // Configure the HTTP request pipeline.
    
                app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

          //  StaticSenderController.InitializeProccess();

            app.Run();

        }
    }
}