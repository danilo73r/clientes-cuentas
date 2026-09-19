using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrastructure.Persistencia.Configuraciones;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.Property(cliente => cliente.Contrasena)
            .IsRequired();

        builder.Property(cliente => cliente.Estado)
            .IsRequired();

        builder.Property(cliente => cliente.Version)
            .IsConcurrencyToken()
            .ValueGeneratedNever();

        builder.Property(cliente => cliente.OperacionPendienteId)
            .IsRequired(false);
    }
}