using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using QuizIA.Logica.Datos;
using QuizIA.Logica.IA;
using QuizIA.Logica.Servicios;

namespace QuizIA.Logica;

public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // Proveedor de IA. Por ahora el falso; en el Paso 7 se cambia por Gemini.
        services.AddSingleton<IChatClient, FakeChatClient>();
        services.AddSingleton<QuizValidator>();
        services.AddScoped<QuizGenerator>();
        services.AddScoped<QuizService>();

        return services;
    }
}
