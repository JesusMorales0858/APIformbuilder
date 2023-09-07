using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace APIformbuilder
{
    public class Startup
    {
        // Este método se llama en tiempo de ejecución. Use este método para configurar servicios.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:8081")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        // Este método se llama en tiempo de ejecución. Use este método para configurar el pipeline de solicitud HTTP.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
        }
    }
}
