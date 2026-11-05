# QuizIA

Aplicación web en ASP.NET Core MVC que genera cuestionarios de estudio a partir de un texto usando IA generativa.
La salida del modelo se valida por código y una persona aprueba cada pregunta antes de guardarla.

Trabajo Práctico de Investigación - Programación Web 3 (UNLaM) - Grupo 6 - Tema: IA Generativa.

## Tecnologías
- .NET 10, ASP.NET Core MVC, ASP.NET Core Identity
- Entity Framework Core + SQL Server
- Microsoft.Extensions.AI (`IChatClient`)
- Bootstrap

## Estructura de la solución
| Proyecto | Responsabilidad |
|---|---|
| `QuizIA.Entidades` | Clases del dominio (Quiz, Pregunta, Intento). No depende de nadie. |
| `QuizIA.Logica` | Acceso a datos (EF Core, migraciones), servicios de negocio e integración con IA. |
| `QuizIA.Web` | ASP.NET Core MVC: controladores, vistas, login (Identity). |

Dependencias: `Web → Logica → Entidades`.

## Requisitos
- SDK de .NET 10
- SQL Server (LocalDB, que viene con Visual Studio, o una instancia propia)
- Herramienta de migraciones: `dotnet tool install --global dotnet-ef`

## Puesta en marcha
1. Clonar el repositorio.
2. Base de datos: por defecto usa LocalDB (`appsettings.json`). Si preferís otra instancia,
   guardá tu cadena de conexión **fuera del repo** con User Secrets, desde la carpeta de la solución:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<tu cadena de conexion>" --project QuizIA.Web
   ```

3. Crear/actualizar la base:

   ```bash
   dotnet ef database update --project QuizIA.Logica --startup-project QuizIA.Web
   ```

   Para crear una migración nueva después de cambiar las entidades:

   ```bash
   dotnet ef migrations add NombreDelCambio --project QuizIA.Logica --startup-project QuizIA.Web
   ```

4. Ejecutar:

   ```bash
   dotnet run --project QuizIA.Web
   ```

## Secretos
Nunca subir claves (API keys, contraseñas) al repositorio. Se guardan con `dotnet user-secrets`.
Cada integrante usa su propia base de datos: los datos no se comparten entre máquinas.
