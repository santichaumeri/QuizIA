# Seguridad en QuizIA

Resumen de las amenazas que consideramos, qué defensa tiene cada una y dónde está en el código.
La idea central es la **defensa en capas**: ninguna capa es infalible, pero juntas reducen mucho el riesgo.

## Riesgos propios de la IA generativa

| Amenaza | Defensa | Dónde |
|---|---|---|
| **Prompt injection**: el texto pegado trae órdenes ("ignorá lo anterior y…") que el modelo podría obedecer | Las reglas van en un mensaje de **sistema** separado; el texto del usuario va aparte, entre `<texto>…</texto>`, con la orden explícita de ignorar instrucciones que estén dentro | `QuizGenerator.InstruccionesSistema` y `GenerarAsync` |
| **Romper el delimitador**: el texto incluye `</texto>` para "salir de la caja" y que sus frases parezcan instrucciones | Se quitan las etiquetas `<texto>` y `</texto>` del texto antes de armar el prompt (repetido hasta que no quede ninguna) | `QuizGenerator.Neutralizar` |
| **Respuesta fuera de formato** (texto libre, JSON roto, campos faltantes) | **Salida estructurada**: se fuerza un esquema JSON (`QuizDto`); si no calza, se descarta y se reintenta una vez | `GetResponseAsync<QuizDto>` y `GenerarYValidarAsync` |
| **Alucinación**: el modelo inventa datos que no están en el texto | La **cita textual** de cada pregunta debe existir dentro del texto original; si no, la pregunta se marca inválida | `QuizValidator.ValidarPregunta` |
| **Preguntas mal formadas** (opciones repetidas, índice fuera de rango, duplicadas) | Validación por código de cada campo, con los mismos límites que la base de datos | `QuizValidator` |
| **Confiar ciegamente en la IA** | **Revisión humana** antes de guardar: las inválidas vienen destildadas y con el motivo; el usuario puede editar | pantalla `Revisar` |
| **Abuso y gasto de cuota** | Máximo de 5 generaciones por minuto por usuario, texto de hasta 8000 caracteres y hasta 10 preguntas | `Program.cs` (limitador), `QuizGenerator` |
| **Filtración de la API key** | La clave **no está en el repo**: se guarda con `dotnet user-secrets` (o variable de entorno) | `DependencyInjection.CrearChatClient` |

## Riesgos de una aplicación web

| Amenaza | Defensa | Dónde |
|---|---|---|
| Acceso sin sesión | `[Authorize]` en todo el controlador de quizzes | `QuizzesController` |
| Ver, rendir o borrar el quiz de otro usuario | Toda consulta filtra por `Id` **y** `UserId`; el id del usuario sale de la cookie de sesión, nunca del formulario; un quiz ajeno da 404 | `QuizService` |
| Manipular el formulario de revisión (HTML editado a mano) | El servidor **vuelve a validar todo** al guardar y limita el tamaño del texto y la cantidad de preguntas | `QuizzesController.Guardar` |
| Ver la respuesta correcta en el código fuente de la página al rendir | La vista de rendir no incluye el índice correcto, la explicación ni la cita; la corrección se hace en el servidor | `Rendir.cshtml`, `Corregir` |
| CSRF | Token anti-falsificación en todos los `POST` | `[ValidateAntiForgeryToken]` |
| XSS: HTML o scripts dentro del texto o de lo que devuelve la IA | Razor **escapa** todo lo que se imprime | vistas `.cshtml` |
| Contraseñas | ASP.NET Core Identity (hash PBKDF2 con sal) | `AddDefaultIdentity` |
| Inyección SQL | Entity Framework Core usa consultas parametrizadas | `QuizService` |

## Límites conocidos (para decirlo con honestidad)

- **Ninguna defensa de prompt es infalible.** Un modelo puede obedecer una inyección sofisticada. Por eso el control final es **código propio más una persona**.
- La verificación de la cita solo comprueba que el fragmento **exista** en el texto, no que justifique la respuesta marcada.
- El plan gratuito de Google puede usar lo que enviamos para mejorar sus productos: no pegar información privada.
- El limitador de generaciones vive en memoria: se reinicia al reiniciar la app y no se comparte entre varias instancias.
