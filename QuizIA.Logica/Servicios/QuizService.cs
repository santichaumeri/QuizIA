using Microsoft.EntityFrameworkCore;
using QuizIA.Entidades;
using QuizIA.Logica.Datos;
using QuizIA.Logica.Dtos;

namespace QuizIA.Logica.Servicios;

/// <summary>Acceso a la base para los quizzes. Convierte los DTOs de la IA en entidades.</summary>
public class QuizService(ApplicationDbContext db)
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
            // A proposito generico: la app no revela que proveedor de IA hay detras.
            Proveedor = "IA",
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

    /// <summary>Lista los quizzes del usuario (mas nuevos primero) sin traer el texto origen, que puede ser grande.</summary>
    public Task<List<QuizResumen>> ListarAsync(string userId, CancellationToken ct = default) =>
        db.Quizzes
          .AsNoTracking()
          .Where(q => q.UserId == userId)
          .OrderByDescending(q => q.FechaCreacion)
          .Select(q => new QuizResumen(q.Id, q.Titulo, q.FechaCreacion, q.Proveedor, q.Preguntas.Count))
          .ToListAsync(ct);

    /// <summary>Borra el quiz solo si es del usuario (las preguntas se borran en cascada). Devuelve false si no existia.</summary>
    public async Task<bool> EliminarAsync(int id, string userId, CancellationToken ct = default) =>
        await db.Quizzes.Where(q => q.Id == id && q.UserId == userId).ExecuteDeleteAsync(ct) > 0;
}
