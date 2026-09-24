using Cuentas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cuentas.Infrastructure.Persistencia.Configuraciones;

public sealed class ProyeccionClienteConfiguration : IEntityTypeConfiguration<ProyeccionCliente>
{
    public void Configure(EntityTypeBuilder<ProyeccionCliente> builder)
    {
        builder.ToTable("ProyeccionesClientes");
        
        builder.HasKey(cliente => cliente.ClienteId);

        builder.Property(cliente => cliente.ClienteId)
            .ValueGeneratedNever();

        builder.Property(cliente => cliente.Version)
            .IsConcurrencyToken()
            .ValueGeneratedNever();
    }
}
