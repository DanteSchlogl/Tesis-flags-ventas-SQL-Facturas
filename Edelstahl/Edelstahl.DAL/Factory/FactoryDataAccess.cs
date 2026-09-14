using Edelstahl.DAL.Implementations.EntityFramework;
using Edelstahl.DAL.Implementations.SqlServer;
using Edelstahl.DAL.Interfaces;

namespace Edelstahl.DAL.Factory
{
    /// <summary>
    /// Centraliza las instancias de los repositorios
    /// utilizados por Edelstahl ERP.
    ///
    /// Negocio:
    /// Entity Framework sobre EdelstahlNegocio.
    ///
    /// Servicios:
    /// ADO.NET sobre EdelstahlServicios.
    /// </summary>
    public static class FactoryDataAccess
    {
        public static IClienteRepository
            ClienteRepository
        {
            get;
        }

        public static IPresupuestoRepository
            PresupuestoRepository
        {
            get;
        }

        public static IPedidoRepository
            PedidoRepository
        {
            get;
        }

        public static IUsuarioRepository
            UsuarioRepository
        {
            get;
        }

        public static IRolRepository
            RolRepository
        {
            get;
        }

        public static IBitacoraRepository
            BitacoraRepository
        {
            get;
        }

        static FactoryDataAccess()
        {
            /*
             * Base de negocio: EdelstahlNegocio
             * Tecnología: Entity Framework 6
             */

            ClienteRepository =
                new ClienteRepositoryEntityFramework();

            PresupuestoRepository =
                new PresupuestoRepositoryEntityFramework();

            PedidoRepository =
                new PedidoRepositoryEntityFramework();

            /*
             * Base de servicios: EdelstahlServicios
             * Tecnología: ADO.NET
             */

            UsuarioRepository =
                new UsuarioRepositorySqlServer();

            RolRepository =
                new RolRepositorySqlServer();

            BitacoraRepository =
                new BitacoraRepositorySqlServer();
        }
    }
}