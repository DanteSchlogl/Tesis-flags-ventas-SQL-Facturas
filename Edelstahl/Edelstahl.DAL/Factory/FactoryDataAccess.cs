using Edelstahl.DAL.Implementations.Memory;
using Edelstahl.DAL.Implementations.SqlServer;
using Edelstahl.DAL.Interfaces;

namespace Edelstahl.DAL.Factory
{
    /// <summary>
    /// Centraliza las instancias de los repositorios
    /// utilizados por Edelstahl.
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

        static FactoryDataAccess()
        {
            ClienteRepository =
                new ClienteRepositorySqlServer();

            PresupuestoRepository =
                new PresupuestoRepositoryMemory();

            PedidoRepository =
                new PedidoRepositorySqlServer();
        }
    }
}