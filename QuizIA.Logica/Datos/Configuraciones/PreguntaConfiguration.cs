using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizIA.Entidades;

namespace QuizIA.Logica.Datos.Configuraciones;

public class PreguntaConfiguration : IEntityTypeConfiguration<Pregunta>
{
    public void Configure(EntityTypeBuilder<Pregunta> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Orden).IsRequired();
        builder.Property(p => p.Enunciado).IsRequired().HasMaxLength(500);

        builder.Property(p => p.OpcionA).IsRequired().HasMaxLength(300);
        builder.Property(p => p.OpcionB).IsRequired().HasMaxLength(300);
        builder.Property(p => p.OpcionC).IsRequired().HasMaxLength(300);
        builder.Property(p => p.OpcionD).IsRequired().HasMaxLength(300);

        builder.Property(p => p.IndiceCorrecta).IsRequired();
        builder.Property(p => p.Explicacion).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.CitaTextual).IsRequired().HasMaxLength(1000);

        // Red de seguridad en la propia base: la opcion correcta solo puede ser 0, 1, 2 o 3.
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Preguntas_IndiceCorrecta",
            "[IndiceCorrecta] BETWEEN 0 AND 3"));
    }
}
