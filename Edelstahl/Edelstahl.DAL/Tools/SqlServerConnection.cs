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
              Initial Catalog=EdelstahlNegocio;
              Integrated Security=True";

        private const string ServicesConnectionString =
            @"Data Source=localhost\SQLEXPRESS;
              Initial Catalog=EdelstahlServicios;
              Integrated Security=True";

        private const string MasterConnectionString =
            @"Data Source=localhost\SQLEXPRESS;
              Initial Catalog=master;
              Integrated Security=True";

        public static SqlConnection CreateConnection()
        {
            return CreateBusinessConnection();
        }

        public static SqlConnection CreateBusinessConnection()
        {
            return new SqlConnection(BusinessConnectionString);
        }

        public static SqlConnection CreateServicesConnection()
        {
            return new SqlConnection(ServicesConnectionString);
        }

        public static SqlConnection CreateMasterConnection()
        {
            return new SqlConnection(MasterConnectionString);
        }
    }
}
