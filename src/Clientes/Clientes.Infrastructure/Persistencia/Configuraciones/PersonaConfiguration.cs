using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrastructure.Persistencia.Configuraciones;

public sealed class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.UseTptMappingStrategy();
        builder.ToTable("Personas");

        builder.HasKey(persona => persona.Id);

        builder.Property(persona => persona.Id)
            .ValueGeneratedNever();

        builder.Property(persona => persona.Nombre)
            .IsRequired();

        builder.Property(persona => persona.Genero)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(persona => persona.FechaNacimiento)
            .HasColumnType("date");

        builder.Property(persona => persona.Identificacion)
            .IsRequired();

        builder.HasIndex(persona => persona.Identificacion)
            .IsUnique()
            .HasDatabaseName("UX_Personas_Identificacion");

        builder.Property(persona => persona.Direccion)
            .IsRequired();

        builder.Property(persona => persona.Telefono)
            .IsRequired();
    }
}