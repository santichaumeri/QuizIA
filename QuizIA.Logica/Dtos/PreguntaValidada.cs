namespace QuizIA.Logica.Dtos;

/// <summary>Una pregunta de la IA junto con el resultado de validarla. Sin errores = valida.</summary>
public record PreguntaValidada(PreguntaDto Pregunta, List<string> Errores)
{
    public bool EsValida => Errores.Count == 0;
}
