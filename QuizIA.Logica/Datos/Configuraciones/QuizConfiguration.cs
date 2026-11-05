using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizIA.Entidades;

namespace QuizIA.Logica.Datos.Configuraciones;

/// <summary>
/// Como se guarda un Quiz en la base. Va aca (y no en la entidad) para que
/// QuizIA.Entidades no dependa de Entity Framework.
/// </summary>
public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(q => q.Id);

        // 450 coincide con el largo de AspNetUsers.Id.
        builder.Property(q => q.UserId).IsRequired().HasMaxLength(450);
        builder.Property(q => q.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(q => q.TextoOrigen).IsRequired();
        builder.Property(q => q.Proveedor).IsRequired().HasMaxLength(50);
        builder.Property(q => q.FechaCreacion).IsRequired();

        // Para listar "mis quizzes" rapido.
        builder.HasIndex(q => q.UserId);

        // Clave foranea hacia AspNetUsers sin que Entidades conozca a Identity.
        builder.HasOne<IdentityUser>()
               .WithMany()
               .HasForeignKey(q => q.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.Preguntas)
               .WithOne(p => p.Quiz)
               .HasForeignKey(p => p.QuizId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
