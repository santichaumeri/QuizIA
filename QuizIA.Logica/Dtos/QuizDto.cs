namespace QuizIA.Logica.Dtos;

/// <summary>Una pregunta tal como la devuelve el modelo de IA (todavia sin validar).</summary>
public record PreguntaDto(
    string Enunciado,
    List<string> Opciones,
    int IndiceCorrecta,
    string Explicacion,
    string CitaTextual);

/// <summary>Formato JSON que le pedimos al modelo: una lista de preguntas.</summary>
public record QuizDto(List<PreguntaDto> Preguntas);
