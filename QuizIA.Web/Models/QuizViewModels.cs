using System.ComponentModel.DataAnnotations;
using QuizIA.Logica.Servicios;

namespace QuizIA.Web.Models;

public class CrearQuizViewModel
{
    [Required(ErrorMessage = "Pegá el texto con el que querés armar el quiz.")]
    [StringLength(QuizGenerator.MaxCaracteresTexto, ErrorMessage = "El texto no puede superar los {1} caracteres.")]
    [Display(Name = "Texto de estudio")]
    public string Texto { get; set; } = string.Empty;

    [Range(1, QuizGenerator.MaxPreguntas, ErrorMessage = "La cantidad debe estar entre {1} y {2}.")]
    [Display(Name = "Cantidad de preguntas")]
    public int Cantidad { get; set; } = QuizGenerator.MaxPreguntas;
}

public class RevisionQuizViewModel
{
    [Required(ErrorMessage = "Poné un título.")]
    [StringLength(200, ErrorMessage = "El título no puede superar los {1} caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    // Se reenvia en un campo oculto: la cita de cada pregunta se vuelve a validar contra este texto.
    public string? TextoOrigen { get; set; }

    public List<PreguntaRevisionViewModel> Preguntas { get; set; } = new();

    public string? Mensaje { get; set; }
}

// Las propiedades son "string?" a proposito: si el usuario deja un campo vacio queremos
// que lo detecte nuestro validador (con un mensaje claro), no el [Required] implicito de MVC.
public class PreguntaRevisionViewModel
{
    public bool Incluir { get; set; }
    public string? Enunciado { get; set; }
    public List<string?> Opciones { get; set; } = new();
    public int IndiceCorrecta { get; set; }
    public string? Explicacion { get; set; }
    public string? CitaTextual { get; set; }

    // Solo se usa para mostrar; no se lee del formulario, se recalcula en el servidor.
    public List<string> Errores { get; set; } = new();
}

public class ResultadoQuizViewModel
{
    public required QuizIA.Entidades.Quiz Quiz { get; init; }
    public required List<ResultadoPreguntaViewModel> Preguntas { get; init; }
    public int Aciertos => Preguntas.Count(p => p.Acerto);
}

public class ResultadoPreguntaViewModel
{
    public required QuizIA.Entidades.Pregunta Pregunta { get; init; }
    /// <summary>Opcion elegida (0 a 3) o null si no respondio.</summary>
    public int? Elegida { get; init; }
    public bool Acerto => Elegida == Pregunta.IndiceCorrecta;
}
