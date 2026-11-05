namespace QuizIA.Logica.Dtos;

/// <summary>Datos livianos de un quiz para mostrar en la lista "Mis quizzes".</summary>
public record QuizResumen(int Id, string Titulo, DateTime FechaCreacion, string Proveedor, int CantidadPreguntas);
