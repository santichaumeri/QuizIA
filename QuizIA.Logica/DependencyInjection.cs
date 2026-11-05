using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizIA.Logica.Datos;

namespace QuizIA.Logica;

/// <summary>
/// Registra en el contenedor de dependencias todo lo que ofrece la capa Logica.
/// La capa Web solo llama a AddLogica y no necesita saber que base de datos se usa.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
