using System;
using Edelstahl.DAL.Implementations.EntityFramework;
using Edelstahl.DAL.Implementations.Memory;
using Edelstahl.DAL.Implementations.SqlServer;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Services.Configuration;

namespace Edelstahl.DAL.Factory
{
    /// <summary>
    /// Centraliza los repositorios utilizados por Edelstahl ERP.
    ///
    /// En modo SQL Server utiliza Entity Framework y ADO.NET.
    /// En modo demostración utiliza repositorios en memoria.
    /// </summary>
    public static class FactoryDataAccess
    {
        private static bool _inicializada;

        public static IClienteRepository ClienteRepository
        {
            get;
            private set;
        }

        public static IPresupuestoRepository PresupuestoRepository
        {
            get;
            private set;
        }

        public static IPedidoRepository PedidoRepository
        {
            get;
            private set;
        }

        public static IUsuarioRepository UsuarioRepository
        {
            get;
            private set;
        }

        public static IRolRepository RolRepository
        {
            get;
            private set;
        }

        public static IBitacoraRepository BitacoraRepository
        {
            get;
            private set;
        }

        /// <summary>
        /// Inicializa los repositorios según el modo de ejecución
        /// seleccionado al comenzar la aplicación.
        /// </summary>
        public static void Inicializar(
            ExecutionMode modo)
        {
            if (modo == ExecutionMode.SqlServer)
            {
                InicializarSqlServer();
                return;
            }

            if (modo == ExecutionMode.Demo)
            {
                InicializarDemo();
                return;
            }

            throw new InvalidOperationException(
                "No se seleccionó un modo de ejecución válido.");
        }

        /// <summary>
        /// Configura los repositorios reales del sistema.
        /// </summary>
        private static void InicializarSqlServer()
        {
            ClienteRepository =
                new ClienteRepositoryEntityFramework();

            PresupuestoRepository =
                new PresupuestoRepositoryEntityFramework();

            PedidoRepository =
                new PedidoRepositoryEntityFramework();

            UsuarioRepository =
                new UsuarioRepositorySqlServer();

            RolRepository =
                new RolRepositorySqlServer();

            BitacoraRepository =
                new BitacoraRepositorySqlServer();

            _inicializada =
                true;
        }

        /// <summary>
        /// Configura los repositorios temporales en memoria.
        ///
        /// Este modo no utiliza SQL Server, usuarios reales,
        /// roles persistentes ni bitácora en base de datos.
        /// </summary>
        private static void InicializarDemo()
        {
            ClienteRepository =
                new ClienteRepositoryMemory();

            PresupuestoRepository =
                new PresupuestoRepositoryMemory();

            PedidoRepository =
                new PedidoRepositoryMemory();

            UsuarioRepository =
                null;

            RolRepository =
                null;

            BitacoraRepository =
                null;

            _inicializada =
                true;
        }

        public static bool EstaInicializada
        {
            get
            {
                return _inicializada;
            }
        }

        public static bool EsModoDemostracion
        {
            get
            {
                return ApplicationMode.IsDemo;
            }
        }

        public static bool UsaSqlServer
        {
            get
            {
                return ApplicationMode.UsesSqlServer;
            }
        }
    }
}