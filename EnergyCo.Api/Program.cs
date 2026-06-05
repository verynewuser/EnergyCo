using EnergyCo.Api.Filters;
using EnergyCo.Application;
using EnergyCo.Application.Common.Interfaces;
using EnergyCo.Infrastructure;
using EnergyCo.Infrastructure.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace EnergyCo.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddApplication();
        builder.Services.AddCors(options => {
            options.AddPolicy("defaultPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
        });
        builder.Services.AddControllers(options => options.Filters.Add(new ApiExceptionFilterAttribute()))
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter()))
                    .AddFluentValidation(fv => fv
                    .RegisterValidatorsFromAssemblyContaining<IApplicationDbContext>());

        builder.Services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                })
            .AddApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

        builder.Services.AddInfrastructure(builder.Configuration);
        //builder.Services.AddSwaggerGen();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("defaultPolicy");
        app.UseAuthorization();


        app.MapControllers();

        // Ensure the database is created and migrations are applied on first run
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating or initializing the database.");
                throw;
            }
        }

        app.Run();
    }
}