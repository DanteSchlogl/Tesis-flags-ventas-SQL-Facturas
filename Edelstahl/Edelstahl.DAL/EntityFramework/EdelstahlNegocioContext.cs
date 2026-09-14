using System.Data.Entity;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.DAL.EntityFramework
{
    /// <summary>
    /// Contexto de Entity Framework 6 para la base EdelstahlNegocio.
    /// </summary>
    public class EdelstahlNegocioContext : DbContext
    {
        public EdelstahlNegocioContext()
            : base("name=EdelstahlNegocioConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = true;
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Presupuesto> Presupuestos { get; set; }

        public DbSet<DetallePresupuesto> DetallesPresupuesto { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<DetallePedido> DetallesPedido { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ClienteConfiguration());
            modelBuilder.Configurations.Add(new PresupuestoConfiguration());
            modelBuilder.Configurations.Add(new DetallePresupuestoConfiguration());
            modelBuilder.Configurations.Add(new PedidoConfiguration());
            modelBuilder.Configurations.Add(new DetallePedidoConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
