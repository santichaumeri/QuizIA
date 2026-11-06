using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using QuizIA.Logica.Datos;
using QuizIA.Logica.IA;
using QuizIA.Logica.Servicios;

namespace QuizIA.Logica;

public static class DependencyInjection
{
    public static IServiceCollection AddLogica(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddSingleton(CrearChatClient(configuration));
        services.AddSingleton<QuizValidator>();
        services.AddScoped<QuizGenerator>();
        services.AddScoped<QuizService>();

        return services;
    }

    /// <summary>
    /// Elige la implementacion de IChatClient segun la configuracion (IA:Proveedor).
    /// El resto de la app solo conoce la interfaz: cambiar de proveedor no toca ninguna otra clase.
    /// </summary>
    private static IChatClient CrearChatClient(IConfiguration configuracion)
    {
        var proveedor = configuracion["IA:Proveedor"] ?? "Fake";

        if (!proveedor.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
            return new FakeChatClient();

        // La clave NO esta en el repo: viene de user-secrets o de una variable de entorno.
        var apiKey = configuracion["IA:Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "IA:Proveedor es 'Gemini' pero falta IA:Gemini:ApiKey. " +
                "Guardala con: dotnet user-secrets set \"IA:Gemini:ApiKey\" \"<tu key>\" --project QuizIA.Web " +
                "o poné IA:Proveedor en 'Fake'.");

        var modelo = configuracion["IA:Gemini:Modelo"] ?? "gemini-3.5-flash-lite";
        var endpoint = configuracion["IA:Gemini:Endpoint"] ?? "https://generativelanguage.googleapis.com/v1beta/openai/";

        // Gemini ofrece un endpoint compatible con la API de OpenAI: se usa el cliente de OpenAI apuntando alli.
        var cliente = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint),
            // Ante 429/503 (servicio saturado) reintenta con espera creciente antes de rendirse. El valor por defecto es 3.
            RetryPolicy = new ClientRetryPolicy(maxRetries: 5)
        });
        return cliente.GetChatClient(modelo).AsIChatClient();
    }
}
