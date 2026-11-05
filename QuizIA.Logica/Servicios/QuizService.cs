using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using QuizIA.Entidades;
using QuizIA.Logica.Datos;
using QuizIA.Logica.Dtos;

namespace QuizIA.Logica.Servicios;

/// <summary>Acceso a la base para los quizzes. Convierte los DTOs de la IA en entidades.</summary>
public class QuizService(ApplicationDbContext db, IChatClient chatClient)
{
    public async Task<Quiz> GuardarAsync(
        string userId, string titulo, string textoOrigen,
        IEnumerable<PreguntaDto> preguntas, CancellationToken ct = default)
    {
        var quiz = new Quiz
        {
            UserId = userId,
            Titulo = titulo.Trim(),
            TextoOrigen = textoOrigen,
            Proveedor = chatClient.GetService<ChatClientMetadata>()?.ProviderName ?? "Desconocido",
            FechaCreacion = DateTime.UtcNow
        };

        var orden = 1;
        foreach (var p in preguntas)
        {
            quiz.Preguntas.Add(new Pregunta
            {
                Orden = orden++,
                Enunciado = p.Enunciado.Trim(),
                OpcionA = p.Opciones[0].Trim(),
                OpcionB = p.Opciones[1].Trim(),
                OpcionC = p.Opciones[2].Trim(),
                OpcionD = p.Opciones[3].Trim(),
                IndiceCorrecta = p.IndiceCorrecta,
                Explicacion = p.Explicacion.Trim(),
                CitaTextual = p.CitaTextual.Trim()
            });
        }

        db.Quizzes.Add(quiz);
        await db.SaveChangesAsync(ct);
        return quiz;
    }

    /// <summary>Devuelve el quiz solo si pertenece al usuario; si no, null.</summary>
    public Task<Quiz?> ObtenerAsync(int id, string userId, CancellationToken ct = default) =>
        db.Quizzes
          .AsNoTracking()
          .Include(q => q.Preguntas.OrderBy(p => p.Orden))
          .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId, ct);
}
