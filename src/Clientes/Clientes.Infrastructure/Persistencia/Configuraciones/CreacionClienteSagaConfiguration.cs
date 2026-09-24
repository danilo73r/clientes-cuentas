using Clientes.Infrastructure.Mensajeria.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrastructure.Persistencia.Configuraciones;

public sealed class CreacionClienteSagaConfiguration : IEntityTypeConfiguration<CreacionClienteSaga>
{
    public void Configure(EntityTypeBuilder<CreacionClienteSaga> builder)
    {
        builder.ToTable("CreacionesClientes");
        
        builder.HasKey(saga => saga.Id);


        builder.Property(saga => saga.Id)
            .ValueGeneratedNever();

        builder.Property(saga => saga.Version)
            .IsConcurrencyToken();

        builder.HasIndex(saga => saga.ClienteId)
            .IsUnique();
    }
}
