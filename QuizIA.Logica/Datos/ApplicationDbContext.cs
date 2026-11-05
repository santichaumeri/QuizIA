using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizIA.Entidades;

namespace QuizIA.Logica.Datos
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Pregunta> Preguntas => Set<Pregunta>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Primero Identity configura sus propias tablas (AspNetUsers, etc.).
            base.OnModelCreating(builder);

            // Despues se cargan todas las clases IEntityTypeConfiguration de este proyecto.
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
