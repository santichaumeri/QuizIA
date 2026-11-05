namespace QuizIA.Entidades;

/// <summary>
/// Un cuestionario generado a partir de un texto. Pertenece a un usuario.
/// </summary>
public class Quiz
{
    public int Id { get; set; }

    /// <summary>Id del usuario dueño (AspNetUsers.Id). Es un string porque Identity usa GUID en texto.</summary>
    public string UserId { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    /// <summary>Texto original que se pegó para generar el quiz.</summary>
    public string TextoOrigen { get; set; } = string.Empty;

    /// <summary>Proveedor de IA que generó las preguntas (por ejemplo "Gemini" o "Fake").</summary>
    public string Proveedor { get; set; } = string.Empty;

    /// <summary>Fecha de creación en UTC.</summary>
    public DateTime FechaCreacion { get; set; }

    public List<Pregunta> Preguntas { get; set; } = new();
}
