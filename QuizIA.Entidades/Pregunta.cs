namespace QuizIA.Entidades;

/// <summary>
/// Una pregunta de opción múltiple con cuatro opciones. Pertenece a un Quiz.
/// </summary>
public class Pregunta
{
    public int Id { get; set; }

    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }

    /// <summary>Posición de la pregunta dentro del quiz (empieza en 1).</summary>
    public int Orden { get; set; }

    public string Enunciado { get; set; } = string.Empty;

    public string OpcionA { get; set; } = string.Empty;
    public string OpcionB { get; set; } = string.Empty;
    public string OpcionC { get; set; } = string.Empty;
    public string OpcionD { get; set; } = string.Empty;

    /// <summary>Opción correcta: 0 = A, 1 = B, 2 = C, 3 = D.</summary>
    public int IndiceCorrecta { get; set; }

    public string Explicacion { get; set; } = string.Empty;

    /// <summary>Fragmento del texto original que respalda la respuesta.</summary>
    public string CitaTextual { get; set; } = string.Empty;
}
