using MediatR;
using Serilog;
using StructureMap.Pipeline;
using XMLTAgsExtractor.Services;

namespace XMLTAgsExtractor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IExtraction, Extraction>();
            builder.Host.UseSerilog((hostContext, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(hostContext.Configuration);
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
           
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}