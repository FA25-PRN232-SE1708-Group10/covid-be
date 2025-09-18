using COVID.BusinessLogicLayer.Services;
using COVID.DataAccessLayer;
using COVID.DataAccessLayer.Data;
using COVID.DataAccessLayer.Repositories;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace COVID.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CovidDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddScoped<ICovidDataRepository, CovidDataRepository>();
            builder.Services.AddScoped<ICovidDataService, CovidDataService>();

            builder
                .Services.AddControllers()
                .AddOData(opt =>
                    opt.Select()
                        .Filter()
                        .OrderBy()
                        .Expand()
                        .SetMaxTop(1000)
                        .Count()
                        .AddRouteComponents("odata", GetEdmModel())
                );

            static IEdmModel GetEdmModel()
            {
                var builder = new ODataConventionModelBuilder();
                builder.EntitySet<COVID.DataAccessLayer.Models.CovidDataPoint>("CovidData");
                return builder.GetEdmModel();
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            await DataSeeder.SeedData(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.Use(
                async (context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments("/odata/CovidData"))
                    {
                        context.Response.Headers.CacheControl = "public, max-age=86400";
                    }
                    await next();
                }
            );

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
