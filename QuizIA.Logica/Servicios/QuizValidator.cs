using QuizIA.Logica.Dtos;

namespace QuizIA.Logica.Servicios;

/// <summary>
/// Revisa con codigo propio lo que devolvio la IA. No confia en el modelo:
/// controla formato, limites y que la cita textual exista realmente en el texto origen.
/// </summary>
public class QuizValidator
{
    // Limites iguales a los de PreguntaConfiguration (la base).
    public const int MaxEnunciado = 500;
    public const int MaxOpcion = 300;
    public const int MaxExplicacion = 1000;
    public const int MaxCita = 1000;

    public List<PreguntaValidada> Validar(IEnumerable<PreguntaDto> preguntas, string textoOrigen)
    {
        var textoNormalizado = Normalizar(textoOrigen);
        var enunciadosVistos = new HashSet<string>();
        var resultado = new List<PreguntaValidada>();

        foreach (var p in preguntas)
        {
            var errores = ValidarPregunta(p, textoNormalizado);

            // Duplicadas: solo cuenta como duplicada la segunda aparicion en adelante.
            if (!string.IsNullOrWhiteSpace(p.Enunciado) && !enunciadosVistos.Add(Normalizar(p.Enunciado)))
                errores.Add("Pregunta duplicada: ya hay otra con el mismo enunciado.");

            resultado.Add(new PreguntaValidada(p, errores));
        }

        return resultado;
    }

    public List<string> ValidarPregunta(PreguntaDto p, string textoOrigen)
    {
        // Acepta tanto el texto crudo como uno ya normalizado (normalizar es idempotente).
        var texto = Normalizar(textoOrigen);
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(p.Enunciado))
            errores.Add("El enunciado está vacío.");
        else if (p.Enunciado.Length > MaxEnunciado)
            errores.Add($"El enunciado supera {MaxEnunciado} caracteres.");

        if (p.Opciones is null || p.Opciones.Count != 4)
            errores.Add("Debe tener exactamente 4 opciones.");
        else
        {
            if (p.Opciones.Any(string.IsNullOrWhiteSpace))
                errores.Add("Hay opciones vacías.");
            if (p.Opciones.Any(o => o is not null && o.Length > MaxOpcion))
                errores.Add($"Una opción supera {MaxOpcion} caracteres.");
            if (p.Opciones.Select(o => Normalizar(o ?? "")).Distinct().Count() != 4)
                errores.Add("Las opciones no son distintas entre sí.");
        }

        if (p.IndiceCorrecta is < 0 or > 3)
            errores.Add("El índice de la opción correcta debe estar entre 0 y 3.");

        if (string.IsNullOrWhiteSpace(p.Explicacion))
            errores.Add("La explicación está vacía.");
        else if (p.Explicacion.Length > MaxExplicacion)
            errores.Add($"La explicación supera {MaxExplicacion} caracteres.");

        if (string.IsNullOrWhiteSpace(p.CitaTextual))
            errores.Add("Falta la cita textual.");
        else if (p.CitaTextual.Length > MaxCita)
            errores.Add($"La cita supera {MaxCita} caracteres.");
        else if (!texto.Contains(Normalizar(p.CitaTextual)))
            errores.Add("La cita no aparece en el texto original (posible invento de la IA).");

        return errores;
    }

    /// <summary>Minusculas y espacios/saltos de linea colapsados, para comparar sin falsos rechazos.</summary>
    private static string Normalizar(string s) =>
        string.Join(' ', s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
}
