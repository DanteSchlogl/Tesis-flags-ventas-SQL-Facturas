using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;



namespace Edelstahl.DAL.Tools
{
    /// <summary>
    /// Centraliza las conexiones con las bases de datos
    /// utilizadas por Edelstahl ERP.
    /// </summary>
    public static class SqlServerConnection
    {
        private const string BusinessConnectionString =
            @"Data Source=localhost\SQLEXPRESS;
              Initial Catalog=Edelstahl;
              Integrated Security=True";

        private const string ServicesConnectionString =
            @"Data Source=localhost\SQLEXPRESS;
              Initial Catalog=EdelstahlServicios;
              Integrated Security=True";

        /// <summary>
        /// Crea una conexión con la base de datos del negocio.
        /// Esta conexión se conserva temporalmente para mantener
        /// compatibles los repositorios actuales.
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            return CreateBusinessConnection();
        }

        /// <summary>
        /// Crea una conexión con la base de datos de negocio.
        /// </summary>
        public static SqlConnection CreateBusinessConnection()
        {
            return new SqlConnection(
                BusinessConnectionString);
        }

        /// <summary>
        /// Crea una conexión con la base de datos de servicios:
        /// usuarios, roles, seguridad y bitácora.
        /// </summary>
        public static SqlConnection CreateServicesConnection()
        {
            return new SqlConnection(
                ServicesConnectionString);
        }
    }
}