using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Edelstahl.DAL.Interfaces;
using Edelstahl.DAL.Tools;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.DAL.Implementations.SqlServer
{
    /// <summary>
    /// Guarda y recupera clientes desde la tabla dbo.ClientesERP.
    /// </summary>
    public class ClienteRepositorySqlServer : IClienteRepository
    {
        private const string Columnas =
            @"Id, CUIT, RazonSocial, DireccionFacturacion,
              DireccionEntrega, Localidad, Provincia, CodigoPostal,
              Email, Telefono, LimiteCredito, DeudaActual, Activo, FechaAlta";

        public void Add(Cliente entity)
        {
            ValidarCliente(entity);
            entity.CUIT = NormalizarCUIT(entity.CUIT);

            if (ExistsByCUIT(entity.CUIT))
            {
                throw new InvalidOperationException(
                    "Ya existe un cliente con el CUIT ingresado.");
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            if (entity.FechaAlta == default(DateTime))
            {
                entity.FechaAlta = DateTime.Now;
            }

            const string sql =
                @"INSERT INTO dbo.ClientesERP
                  (Id, CUIT, RazonSocial, DireccionFacturacion,
                   DireccionEntrega, Localidad, Provincia, CodigoPostal,
                   Email, Telefono, LimiteCredito, DeudaActual, Activo, FechaAlta)
                  VALUES
                  (@Id, @CUIT, @RazonSocial, @DireccionFacturacion,
                   @DireccionEntrega, @Localidad, @Provincia, @CodigoPostal,
                   @Email, @Telefono, @LimiteCredito, @DeudaActual,
                   @Activo, @FechaAlta)";

            EjecutarEscritura(sql, entity, false);
        }

        public void Update(Cliente entity)
        {
            ValidarCliente(entity);

            if (entity.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(entity));
            }

            entity.CUIT = NormalizarCUIT(entity.CUIT);

            Cliente existente = GetByCUIT(entity.CUIT);

            if (existente != null && existente.Id != entity.Id)
            {
                throw new InvalidOperationException(
                    "Ya existe otro cliente con el CUIT ingresado.");
            }

            const string sql =
                @"UPDATE dbo.ClientesERP
                  SET CUIT = @CUIT,
                      RazonSocial = @RazonSocial,
                      DireccionFacturacion = @DireccionFacturacion,
                      DireccionEntrega = @DireccionEntrega,
                      Localidad = @Localidad,
                      Provincia = @Provincia,
                      CodigoPostal = @CodigoPostal,
                      Email = @Email,
                      Telefono = @Telefono,
                      LimiteCredito = @LimiteCredito,
                      DeudaActual = @DeudaActual,
                      Activo = @Activo,
                      FechaAlta = @FechaAlta
                  WHERE Id = @Id";

            EjecutarEscritura(sql, entity, true);
        }

        public void Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(id));
            }

            const string sql =
                @"UPDATE dbo.ClientesERP
                  SET Activo = 0
                  WHERE Id = @Id";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@Id", SqlDbType.UniqueIdentifier).Value = id;

                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new InvalidOperationException(
                            "No se encontró el cliente que se desea eliminar.");
                    }
                }
            }
        }

        public Cliente GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            string sql =
                "SELECT " + Columnas +
                " FROM dbo.ClientesERP WHERE Id = @Id";

            return ObtenerUno(
                sql,
                command => command.Parameters.Add(
                    "@Id", SqlDbType.UniqueIdentifier).Value = id);
        }

        public List<Cliente> GetAll()
        {
            List<Cliente> clientes = new List<Cliente>();
            string sql =
                "SELECT " + Columnas +
                " FROM dbo.ClientesERP ORDER BY RazonSocial";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(Mapear(reader));
                    }
                }
            }

            return clientes;
        }

        public Cliente GetByCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
            {
                return null;
            }

            string sql =
                "SELECT " + Columnas +
                " FROM dbo.ClientesERP WHERE CUIT = @CUIT";

            return ObtenerUno(
                sql,
                command => command.Parameters.Add(
                    "@CUIT", SqlDbType.NVarChar, 20).Value =
                    NormalizarCUIT(cuit));
        }

        public bool ExistsByCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
            {
                return false;
            }

            const string sql =
                @"SELECT COUNT(1)
                  FROM dbo.ClientesERP
                  WHERE CUIT = @CUIT";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@CUIT", SqlDbType.NVarChar, 20).Value =
                        NormalizarCUIT(cuit);

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        private static Cliente ObtenerUno(
            string sql,
            Action<SqlCommand> configurar)
        {
            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    configurar(command);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? Mapear(reader) : null;
                    }
                }
            }
        }

        private static void EjecutarEscritura(
            string sql,
            Cliente cliente,
            bool exigirFila)
        {
            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    AgregarParametros(command, cliente);
                    int filas = command.ExecuteNonQuery();

                    if (exigirFila && filas == 0)
                    {
                        throw new InvalidOperationException(
                            "No se encontró el cliente que se desea modificar.");
                    }
                }
            }
        }

        private static Cliente Mapear(SqlDataReader reader)
        {
            return new Cliente
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                CUIT = Texto(reader, "CUIT"),
                RazonSocial = Texto(reader, "RazonSocial"),
                DireccionFacturacion = Texto(reader, "DireccionFacturacion"),
                DireccionEntrega = Texto(reader, "DireccionEntrega"),
                Localidad = Texto(reader, "Localidad"),
                Provincia = Texto(reader, "Provincia"),
                CodigoPostal = Texto(reader, "CodigoPostal"),
                Email = Texto(reader, "Email"),
                Telefono = Texto(reader, "Telefono"),
                LimiteCredito = Decimal(reader, "LimiteCredito"),
                DeudaActual = Decimal(reader, "DeudaActual"),
                Activo = Booleano(reader, "Activo"),
                FechaAlta = Fecha(reader, "FechaAlta")
            };
        }

        private static void AgregarParametros(
            SqlCommand command,
            Cliente cliente)
        {
            command.Parameters.Add(
                "@Id", SqlDbType.UniqueIdentifier).Value = cliente.Id;
            command.Parameters.Add(
                "@CUIT", SqlDbType.NVarChar, 20).Value =
                NormalizarCUIT(cliente.CUIT);
            command.Parameters.Add(
                "@RazonSocial", SqlDbType.NVarChar, 250).Value =
                Limpiar(cliente.RazonSocial);
            command.Parameters.Add(
                "@DireccionFacturacion", SqlDbType.NVarChar, 500).Value =
                Limpiar(cliente.DireccionFacturacion);
            command.Parameters.Add(
                "@DireccionEntrega", SqlDbType.NVarChar, 500).Value =
                Limpiar(cliente.DireccionEntrega);
            command.Parameters.Add(
                "@Localidad", SqlDbType.NVarChar, 150).Value =
                Limpiar(cliente.Localidad);
            command.Parameters.Add(
                "@Provincia", SqlDbType.NVarChar, 150).Value =
                Limpiar(cliente.Provincia);
            command.Parameters.Add(
                "@CodigoPostal", SqlDbType.NVarChar, 20).Value =
                Limpiar(cliente.CodigoPostal);
            command.Parameters.Add(
                "@Email", SqlDbType.NVarChar, 250).Value =
                Limpiar(cliente.Email);
            command.Parameters.Add(
                "@Telefono", SqlDbType.NVarChar, 100).Value =
                Limpiar(cliente.Telefono);

            AgregarDecimal(command, "@LimiteCredito", cliente.LimiteCredito);
            AgregarDecimal(command, "@DeudaActual", cliente.DeudaActual);

            command.Parameters.Add(
                "@Activo", SqlDbType.Bit).Value = cliente.Activo;
            command.Parameters.Add(
                "@FechaAlta", SqlDbType.DateTime2).Value = cliente.FechaAlta;
        }

        private static void AgregarDecimal(
            SqlCommand command,
            string nombre,
            decimal valor)
        {
            SqlParameter parametro =
                command.Parameters.Add(nombre, SqlDbType.Decimal);

            parametro.Precision = 18;
            parametro.Scale = 2;
            parametro.Value = valor;
        }

        private static string Texto(SqlDataReader reader, string columna)
        {
            object valor = reader[columna];
            return valor == DBNull.Value ? string.Empty : valor.ToString();
        }

        private static decimal Decimal(SqlDataReader reader, string columna)
        {
            object valor = reader[columna];
            return valor == DBNull.Value ? 0m : Convert.ToDecimal(valor);
        }

        private static bool Booleano(SqlDataReader reader, string columna)
        {
            object valor = reader[columna];
            return valor != DBNull.Value && Convert.ToBoolean(valor);
        }

        private static DateTime Fecha(SqlDataReader reader, string columna)
        {
            object valor = reader[columna];
            return valor == DBNull.Value ? DateTime.Now : Convert.ToDateTime(valor);
        }

        private static string Limpiar(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }

        private static string NormalizarCUIT(string cuit)
        {
            return string.IsNullOrWhiteSpace(cuit)
                ? string.Empty
                : cuit.Trim()
                    .Replace("-", string.Empty)
                    .Replace(" ", string.Empty)
                    .Replace(".", string.Empty);
        }

        private static void ValidarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(
                    nameof(cliente),
                    "El cliente no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(cliente.CUIT))
            {
                throw new InvalidOperationException(
                    "El CUIT es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                throw new InvalidOperationException(
                    "La razón social es obligatoria.");
            }

            if (cliente.LimiteCredito < 0m || cliente.DeudaActual < 0m)
            {
                throw new InvalidOperationException(
                    "El límite de crédito y la deuda no pueden ser negativos.");
            }
        }
    }
}
