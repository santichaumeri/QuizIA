using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizIA.Logica.Dtos;
using QuizIA.Logica.Servicios;
using QuizIA.Web.Models;

namespace QuizIA.Web.Controllers;

[Authorize] // sin login no se entra a ninguna accion de este controlador
public class QuizzesController(
    QuizGenerator generator,
    QuizValidator validator,
    QuizService quizService,
    ILogger<QuizzesController> logger) : Controller
{
    // El id sale de la cookie de sesion, nunca de un campo del formulario.
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public IActionResult Crear() => View(new CrearQuizViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(CrearQuizViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Crear", model);

        List<PreguntaValidada> resultado;
        try
        {
            resultado = await generator.GenerarYValidarAsync(model.Texto, model.Cantidad, ct);
        }
        catch (Exception ex) when (ex is InvalidOperationException or HttpRequestException)
        {
            logger.LogWarning(ex, "Fallo la generacion del quiz");
            ModelState.AddModelError(string.Empty, "No se pudo generar el quiz. Probá de nuevo o con otro texto.");
            return View("Crear", model);
        }

        var vm = new RevisionQuizViewModel
        {
            Titulo = TituloSugerido(model.Texto),
            TextoOrigen = model.Texto,
            Preguntas = resultado.Select(r => new PreguntaRevisionViewModel
            {
                Incluir = r.EsValida,
                Enunciado = r.Pregunta.Enunciado,
                Opciones = r.Pregunta.Opciones?.Cast<string?>().ToList() ?? new(),
                IndiceCorrecta = r.Pregunta.IndiceCorrecta,
                Explicacion = r.Pregunta.Explicacion,
                CitaTextual = r.Pregunta.CitaTextual,
                Errores = r.Errores
            }).ToList()
        };

        var invalidas = resultado.Count(r => !r.EsValida);
        var partes = new List<string>();
        if (resultado.Count < model.Cantidad)
            partes.Add($"Se generaron {resultado.Count} de {model.Cantidad} preguntas.");
        if (invalidas > 0)
            partes.Add($"{invalidas} no pasaron la validación y vienen destildadas.");
        vm.Mensaje = partes.Count > 0 ? string.Join(" ", partes) : null;

        CompletarOpciones(vm);
        ModelState.Clear();
        return View("Revisar", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(RevisionQuizViewModel model, CancellationToken ct)
    {
        var texto = model.TextoOrigen ?? string.Empty;
        // El texto viaja en un campo oculto: si lo manipularon, se corta aca.
        if (string.IsNullOrWhiteSpace(texto) || texto.Length > QuizGenerator.MaxCaracteresTexto
            || model.Preguntas.Count > QuizGenerator.MaxPreguntas)
            return BadRequest();

        var incluidas = model.Preguntas.Where(p => p.Incluir).ToList();
        if (incluidas.Count == 0)
            ModelState.AddModelError(string.Empty, "Tildá al menos una pregunta para guardar.");

        // Se vuelve a validar TODO en el servidor: el navegador puede haber mandado cualquier cosa.
        var validadas = validator.Validar(incluidas.Select(ADto), texto);
        for (var i = 0; i < incluidas.Count; i++)
            incluidas[i].Errores = validadas[i].Errores;
        if (validadas.Any(v => !v.EsValida))
            ModelState.AddModelError(string.Empty, "Corregí o destildá las preguntas con errores.");

        if (!ModelState.IsValid)
        {
            CompletarOpciones(model);
            return View("Revisar", model);
        }

        var quiz = await quizService.GuardarAsync(UserId, model.Titulo, texto, incluidas.Select(ADto), ct);
        return RedirectToAction(nameof(Guardado), new { id = quiz.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Guardado(int id, CancellationToken ct)
    {
        // ObtenerAsync filtra por UserId: no se puede ver el quiz de otro usuario.
        var quiz = await quizService.ObtenerAsync(id, UserId, ct);
        return quiz is null ? NotFound() : View(quiz);
    }

    private static PreguntaDto ADto(PreguntaRevisionViewModel p) => new(
        p.Enunciado ?? string.Empty,
        (p.Opciones ?? new()).Select(o => o ?? string.Empty).ToList(),
        p.IndiceCorrecta,
        p.Explicacion ?? string.Empty,
        p.CitaTextual ?? string.Empty);

    // La vista siempre muestra 4 campos de opcion, aunque la IA haya devuelto menos.
    private static void CompletarOpciones(RevisionQuizViewModel vm)
    {
        foreach (var p in vm.Preguntas)
        {
            p.Opciones ??= new();
            while (p.Opciones.Count < 4) p.Opciones.Add(string.Empty);
        }
    }

    private static string TituloSugerido(string texto)
    {
        var primeraLinea = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                .FirstOrDefault() ?? "Nuevo quiz";
        return primeraLinea.Length <= 60 ? primeraLinea : primeraLinea[..60].TrimEnd() + "…";
    }
}
