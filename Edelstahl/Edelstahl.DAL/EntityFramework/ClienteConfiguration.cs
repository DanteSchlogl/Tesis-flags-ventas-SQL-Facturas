using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Edelstahl.Domain.Comercial;
using System.Data.Entity.Infrastructure.Annotations;

namespace Edelstahl.DAL.EntityFramework
{
    public class ClienteConfiguration
        : EntityTypeConfiguration<Cliente>
    {
        public ClienteConfiguration()
        {
            ToTable("Clientes");

            HasKey(cliente => cliente.Id);

            Property(cliente => cliente.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(cliente => cliente.CUIT)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("UQ_Clientes_CUIT")
                        {
                            IsUnique = true
                        }));

            Property(cliente => cliente.RazonSocial)
                .IsRequired()
                .HasMaxLength(250);

            Property(cliente => cliente.DireccionFacturacion)
                .IsRequired()
                .HasMaxLength(500);

            Property(cliente => cliente.DireccionEntrega)
                .IsRequired()
                .HasMaxLength(500);

            Property(cliente => cliente.Localidad)
                .IsRequired()
                .HasMaxLength(150);

            Property(cliente => cliente.Provincia)
                .IsRequired()
                .HasMaxLength(150);

            Property(cliente => cliente.CodigoPostal)
                .IsRequired()
                .HasMaxLength(20);

            Property(cliente => cliente.Email)
                .IsRequired()
                .HasMaxLength(250);

            Property(cliente => cliente.Telefono)
                .IsRequired()
                .HasMaxLength(100);

            Property(cliente => cliente.LimiteCredito)
                .HasPrecision(18, 2);

            Property(cliente => cliente.DeudaActual)
                .HasPrecision(18, 2);

            Property(cliente => cliente.FechaAlta)
                .HasColumnType("datetime2");
        }
    }
}
