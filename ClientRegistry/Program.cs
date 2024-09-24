using ClientRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientRegistry;

public class Program
{
    public const int MaxCharsInn = 12;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (Environment.GetEnvironmentVariable("IS_DOCKER") != "true")
        {
            builder.Services.AddNpgsql<AppDbContext>(builder.Configuration.GetConnectionString("debug_postgres"));
        }
        else
        {
            builder.Services.AddNpgsql<AppDbContext>(builder.Configuration.GetConnectionString("postgres"));
        }


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();


        
        {
            var scope = app.Services.CreateScope();
            
            using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();;
        
            dbContext.Database.EnsureCreated();
            // dbContext.Database.Migrate();
            scope.Dispose();
        }


        app.Run();
    }
}