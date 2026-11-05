using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using QuizIA.Logica.Dtos;

namespace QuizIA.Logica.IA;

/// <summary>
/// IChatClient falso para desarrollar y para la demo sin internet.
/// No usa IA: arma las preguntas con las propias oraciones del texto recibido,
/// asi cada cita textual es real y el resto del sistema se puede probar de punta a punta.
/// </summary>
public partial class FakeChatClient : IChatClient
{
    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var usuario = messages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;

        var cantidad = int.TryParse(CantidadRegex().Match(usuario).Groups[1].Value, out var n) ? n : 5;
        var texto = TextoRegex().Match(usuario).Groups[1].Value.Trim();

        var quiz = Armar(texto, cantidad);
        var json = JsonSerializer.Serialize(quiz, JsonSerializerOptions.Web);
        return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, json)));
    }

    private static QuizDto Armar(string texto, int cantidad)
    {
        var oraciones = OracionRegex().Split(texto)
            .Select(o => o.Trim())
            .Where(o => o.Length is >= 30 and <= 300)
            .Distinct()
            .ToList();

        var preguntas = new List<PreguntaDto>();
        for (var i = 0; i < oraciones.Count && preguntas.Count < cantidad; i++)
        {
            var correcta = oraciones[i];

            // Distractores: otras oraciones del texto; si faltan, frases genericas.
            var distractores = oraciones.Where(o => o != correcta).Take(3).ToList();
            var genericas = new[] { "Ninguna de las ideas del texto.", "Una afirmación que el texto contradice.", "Un dato que no se menciona." };
            for (var g = 0; distractores.Count < 3; g++) distractores.Add(genericas[g]);

            var indice = i % 4;
            var opciones = new List<string>(distractores);
            opciones.Insert(indice, correcta);

            preguntas.Add(new PreguntaDto(
                Enunciado: $"¿Cuál de las siguientes afirmaciones aparece en el texto? (pregunta {preguntas.Count + 1})",
                Opciones: opciones,
                IndiceCorrecta: indice,
                Explicacion: "Esta afirmación figura textualmente en el texto.",
                CitaTextual: correcta));
        }

        return new QuizDto(preguntas);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var respuesta = await GetResponseAsync(messages, options, cancellationToken);
        foreach (var update in respuesta.ToChatResponseUpdates())
            yield return update;
    }

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;

    public void Dispose() { }

    [GeneratedRegex(@"Cantidad de preguntas:\s*(\d+)")]
    private static partial Regex CantidadRegex();

    [GeneratedRegex(@"<texto>(.*?)</texto>", RegexOptions.Singleline)]
    private static partial Regex TextoRegex();

    [GeneratedRegex(@"(?<=[.!?])\s+")]
    private static partial Regex OracionRegex();
}
