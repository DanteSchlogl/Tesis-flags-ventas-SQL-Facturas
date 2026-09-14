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
    /// Guarda y recupera pedidos y sus detalles desde SQL Server.
    /// </summary>
    public class PedidoRepositorySqlServer : IPedidoRepository
    {
        private const string ColumnasPedido =
            @"Id, Numero, ClienteId, PresupuestoId, FechaPedido, Estado,
              Moneda, TipoCambio, PorcentajeIVA, PorcentajeRecargo,
              PorcentajeDescuentoGeneral, PorcentajeAnticipo,
              ImporteAnticipo, MedioPagoAnticipo, ComprobanteAnticipo,
              FechaAnticipo, CondicionPago, PlazoEntrega,
              EntregaIncluida, Observaciones";

        public void Add(Pedido entity)
        {
            ValidarPedido(entity);

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            if (ExistsByNumero(entity.Numero))
            {
                throw new InvalidOperationException(
                    "Ya existe un pedido con el número ingresado.");
            }

            if (ExistsByPresupuestoId(entity.PresupuestoId))
            {
                throw new InvalidOperationException(
                    "El presupuesto ya fue convertido en un pedido.");
            }

            using (SqlConnection connection =
                SqlServerConnection.CreateServicesConnection())
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        InsertarPedido(connection, transaction, entity);
                        InsertarDetalles(connection, transaction, entity);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(Pedido entity)
        {
            ValidarPedido(entity);

            if (entity.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del pedido no es válido.",
                    nameof(entity));
            }

            const string sql =
                @"UPDATE dbo.Pedidos
                  SET Numero = @Numero,
                      ClienteId = @ClienteId,
                      PresupuestoId = @PresupuestoId,
                      FechaPedido = @FechaPedido,
                      Estado = @Estado,
                      Moneda = @Moneda,
                      TipoCambio = @TipoCambio,
                      PorcentajeIVA = @PorcentajeIVA,
                      PorcentajeRecargo = @PorcentajeRecargo,
                      PorcentajeDescuentoGeneral = @PorcentajeDescuentoGeneral,
                      PorcentajeAnticipo = @PorcentajeAnticipo,
                      ImporteAnticipo = @ImporteAnticipo,
                      MedioPagoAnticipo = @MedioPagoAnticipo,
                      ComprobanteAnticipo = @ComprobanteAnticipo,
                      FechaAnticipo = @FechaAnticipo,
                      CondicionPago = @CondicionPago,
                      PlazoEntrega = @PlazoEntrega,
                      EntregaIncluida = @EntregaIncluida,
                      Observaciones = @Observaciones
                  WHERE Id = @Id";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command =
                            new SqlCommand(sql, connection, transaction))
                        {
                            AgregarParametrosPedido(command, entity);

                            if (command.ExecuteNonQuery() == 0)
                            {
                                throw new InvalidOperationException(
                                    "No se encontró el pedido que se desea modificar.");
                            }
                        }

                        EliminarDetalles(connection, transaction, entity.Id);
                        InsertarDetalles(connection, transaction, entity);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del pedido no es válido.",
                    nameof(id));
            }

            const string sql =
                @"DELETE FROM dbo.Pedidos WHERE Id = @Id";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@Id", SqlDbType.UniqueIdentifier).Value = id;

                    command.ExecuteNonQuery();
                }
            }
        }

        public Pedido GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            string sql =
                "SELECT " + ColumnasPedido +
                " FROM dbo.Pedidos WHERE Id = @Id";

            return ObtenerPedido(
                sql,
                command => command.Parameters.Add(
                    "@Id", SqlDbType.UniqueIdentifier).Value = id);
        }

        public List<Pedido> GetAll()
        {
            string sql =
                "SELECT " + ColumnasPedido +
                " FROM dbo.Pedidos ORDER BY FechaPedido DESC";

            return ObtenerListaPedidos(sql, null);
        }

        public Pedido GetByNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return null;
            }

            string sql =
                "SELECT " + ColumnasPedido +
                " FROM dbo.Pedidos WHERE Numero = @Numero";

            return ObtenerPedido(
                sql,
                command => command.Parameters.Add(
                    "@Numero", SqlDbType.NVarChar, 50).Value =
                    numero.Trim().ToUpperInvariant());
        }

        public Pedido GetByPresupuestoId(Guid presupuestoId)
        {
            if (presupuestoId == Guid.Empty)
            {
                return null;
            }

            string sql =
                "SELECT " + ColumnasPedido +
                " FROM dbo.Pedidos WHERE PresupuestoId = @PresupuestoId";

            return ObtenerPedido(
                sql,
                command => command.Parameters.Add(
                    "@PresupuestoId", SqlDbType.UniqueIdentifier).Value =
                    presupuestoId);
        }

        public List<Pedido> GetByClienteId(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
            {
                return new List<Pedido>();
            }

            string sql =
                "SELECT " + ColumnasPedido +
                " FROM dbo.Pedidos WHERE ClienteId = @ClienteId" +
                " ORDER BY FechaPedido DESC";

            return ObtenerListaPedidos(
                sql,
                command => command.Parameters.Add(
                    "@ClienteId", SqlDbType.UniqueIdentifier).Value =
                    clienteId);
        }

        public bool ExistsByNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return false;
            }

            const string sql =
                @"SELECT COUNT(1)
                  FROM dbo.Pedidos
                  WHERE Numero = @Numero";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@Numero", SqlDbType.NVarChar, 50).Value =
                        numero.Trim().ToUpperInvariant();

                    return Convert.ToInt32(
                        command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool ExistsByPresupuestoId(Guid presupuestoId)
        {
            if (presupuestoId == Guid.Empty)
            {
                return false;
            }

            const string sql =
                @"SELECT COUNT(1)
                  FROM dbo.Pedidos
                  WHERE PresupuestoId = @PresupuestoId";

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@PresupuestoId", SqlDbType.UniqueIdentifier).Value =
                        presupuestoId;

                    return Convert.ToInt32(
                        command.ExecuteScalar()) > 0;
                }
            }
        }

        private static void InsertarPedido(
            SqlConnection connection,
            SqlTransaction transaction,
            Pedido pedido)
        {
            const string sql =
                @"INSERT INTO dbo.Pedidos
                  (
                      Id, Numero, ClienteId, PresupuestoId, FechaPedido,
                      Estado, Moneda, TipoCambio, PorcentajeIVA,
                      PorcentajeRecargo, PorcentajeDescuentoGeneral,
                      PorcentajeAnticipo, ImporteAnticipo,
                      MedioPagoAnticipo, ComprobanteAnticipo,
                      FechaAnticipo, CondicionPago, PlazoEntrega,
                      EntregaIncluida, Observaciones
                  )
                  VALUES
                  (
                      @Id, @Numero, @ClienteId, @PresupuestoId, @FechaPedido,
                      @Estado, @Moneda, @TipoCambio, @PorcentajeIVA,
                      @PorcentajeRecargo, @PorcentajeDescuentoGeneral,
                      @PorcentajeAnticipo, @ImporteAnticipo,
                      @MedioPagoAnticipo, @ComprobanteAnticipo,
                      @FechaAnticipo, @CondicionPago, @PlazoEntrega,
                      @EntregaIncluida, @Observaciones
                  )";

            using (SqlCommand command =
                new SqlCommand(sql, connection, transaction))
            {
                AgregarParametrosPedido(command, pedido);
                command.ExecuteNonQuery();
            }
        }

        private static void InsertarDetalles(
            SqlConnection connection,
            SqlTransaction transaction,
            Pedido pedido)
        {
            if (pedido.Detalles == null)
            {
                return;
            }

            const string sql =
                @"INSERT INTO dbo.DetallesPedido
                  (
                      Id, PedidoId, DetallePresupuestoId, Codigo,
                      Descripcion, DescripcionTecnica, TipoItem,
                      Cantidad, PrecioUnitario, PorcentajeDescuento,
                      RequiereFabricacion
                  )
                  VALUES
                  (
                      @Id, @PedidoId, @DetallePresupuestoId, @Codigo,
                      @Descripcion, @DescripcionTecnica, @TipoItem,
                      @Cantidad, @PrecioUnitario, @PorcentajeDescuento,
                      @RequiereFabricacion
                  )";

            foreach (DetallePedido detalle in pedido.Detalles)
            {
                if (detalle.Id == Guid.Empty)
                {
                    detalle.Id = Guid.NewGuid();
                }

                detalle.PedidoId = pedido.Id;

                using (SqlCommand command =
                    new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.Add(
                        "@Id", SqlDbType.UniqueIdentifier).Value = detalle.Id;
                    command.Parameters.Add(
                        "@PedidoId", SqlDbType.UniqueIdentifier).Value = pedido.Id;
                    command.Parameters.Add(
                        "@DetallePresupuestoId", SqlDbType.UniqueIdentifier).Value =
                        detalle.DetallePresupuestoId;
                    command.Parameters.Add(
                        "@Codigo", SqlDbType.NVarChar, 100).Value =
                        LimpiarTexto(detalle.Codigo);
                    command.Parameters.Add(
                        "@Descripcion", SqlDbType.NVarChar, 500).Value =
                        LimpiarTexto(detalle.Descripcion);
                    command.Parameters.Add(
                        "@DescripcionTecnica", SqlDbType.NVarChar, 1000).Value =
                        LimpiarTexto(detalle.DescripcionTecnica);
                    command.Parameters.Add(
                        "@TipoItem", SqlDbType.Int).Value =
                        (int)detalle.TipoItem;

                    AgregarDecimal(command, "@Cantidad", detalle.Cantidad);
                    AgregarDecimal(command, "@PrecioUnitario", detalle.PrecioUnitario);
                    AgregarDecimal(
                        command,
                        "@PorcentajeDescuento",
                        detalle.PorcentajeDescuento);

                    command.Parameters.Add(
                        "@RequiereFabricacion", SqlDbType.Bit).Value =
                        detalle.RequiereFabricacion;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EliminarDetalles(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid pedidoId)
        {
            const string sql =
                @"DELETE FROM dbo.DetallesPedido
                  WHERE PedidoId = @PedidoId";

            using (SqlCommand command =
                new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add(
                    "@PedidoId", SqlDbType.UniqueIdentifier).Value = pedidoId;

                command.ExecuteNonQuery();
            }
        }

        private Pedido ObtenerPedido(
            string sql,
            Action<SqlCommand> configurarComando)
        {
            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    if (configurarComando != null)
                    {
                        configurarComando(command);
                    }

                    Pedido pedido;

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        pedido = MapearPedido(reader);
                    }

                    pedido.Detalles =
                        ObtenerDetalles(connection, pedido.Id);

                    return pedido;
                }
            }
        }

        private List<Pedido> ObtenerListaPedidos(
            string sql,
            Action<SqlCommand> configurarComando)
        {
            List<Pedido> pedidos =
                new List<Pedido>();

            using (SqlConnection connection =
                SqlServerConnection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    if (configurarComando != null)
                    {
                        configurarComando(command);
                    }

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pedidos.Add(
                                MapearPedido(reader));
                        }
                    }

                    foreach (Pedido pedido in pedidos)
                    {
                        pedido.Detalles =
                            ObtenerDetalles(connection, pedido.Id);
                    }
                }
            }

            return pedidos;
        }

        private static List<DetallePedido> ObtenerDetalles(
            SqlConnection connection,
            Guid pedidoId)
        {
            List<DetallePedido> detalles =
                new List<DetallePedido>();

            const string sql =
                @"SELECT
                      Id,
                      PedidoId,
                      DetallePresupuestoId,
                      Codigo,
                      Descripcion,
                      DescripcionTecnica,
                      TipoItem,
                      Cantidad,
                      PrecioUnitario,
                      PorcentajeDescuento,
                      RequiereFabricacion
                  FROM dbo.DetallesPedido
                  WHERE PedidoId = @PedidoId
                  ORDER BY Codigo";

            using (SqlCommand command =
                new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@PedidoId", SqlDbType.UniqueIdentifier).Value = pedidoId;

                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DetallePedido detalle =
                            new DetallePedido();

                        detalle.Id =
                            reader.GetGuid(reader.GetOrdinal("Id"));
                        detalle.PedidoId =
                            reader.GetGuid(reader.GetOrdinal("PedidoId"));
                        detalle.DetallePresupuestoId =
                            reader.GetGuid(
                                reader.GetOrdinal("DetallePresupuestoId"));
                        detalle.Codigo =
                            ObtenerTexto(reader, "Codigo");
                        detalle.Descripcion =
                            ObtenerTexto(reader, "Descripcion");
                        detalle.DescripcionTecnica =
                            ObtenerTexto(reader, "DescripcionTecnica");
                        detalle.TipoItem =
                            (TipoItemPresupuesto)
                            Convert.ToInt32(reader["TipoItem"]);
                        detalle.Cantidad =
                            Convert.ToDecimal(reader["Cantidad"]);
                        detalle.PrecioUnitario =
                            Convert.ToDecimal(reader["PrecioUnitario"]);
                        detalle.PorcentajeDescuento =
                            Convert.ToDecimal(reader["PorcentajeDescuento"]);
                        detalle.RequiereFabricacion =
                            Convert.ToBoolean(reader["RequiereFabricacion"]);

                        detalles.Add(detalle);
                    }
                }
            }

            return detalles;
        }

        private static Pedido MapearPedido(
            SqlDataReader reader)
        {
            Pedido pedido =
                new Pedido();

            pedido.Id =
                reader.GetGuid(reader.GetOrdinal("Id"));
            pedido.Numero =
                ObtenerTexto(reader, "Numero");
            pedido.ClienteId =
                reader.GetGuid(reader.GetOrdinal("ClienteId"));
            pedido.PresupuestoId =
                reader.GetGuid(reader.GetOrdinal("PresupuestoId"));
            pedido.FechaPedido =
                Convert.ToDateTime(reader["FechaPedido"]);
            pedido.Estado =
                (EstadoPedido)Convert.ToInt32(reader["Estado"]);
            pedido.Moneda =
                (Moneda)Convert.ToInt32(reader["Moneda"]);
            pedido.TipoCambio =
                Convert.ToDecimal(reader["TipoCambio"]);
            pedido.PorcentajeIVA =
                Convert.ToDecimal(reader["PorcentajeIVA"]);
            pedido.PorcentajeRecargo =
                Convert.ToDecimal(reader["PorcentajeRecargo"]);
            pedido.PorcentajeDescuentoGeneral =
                Convert.ToDecimal(reader["PorcentajeDescuentoGeneral"]);
            pedido.PorcentajeAnticipo =
                Convert.ToDecimal(reader["PorcentajeAnticipo"]);
            pedido.ImporteAnticipo =
                Convert.ToDecimal(reader["ImporteAnticipo"]);
            pedido.MedioPagoAnticipo =
                ObtenerTexto(reader, "MedioPagoAnticipo");
            pedido.ComprobanteAnticipo =
                ObtenerTexto(reader, "ComprobanteAnticipo");
            pedido.FechaAnticipo =
                reader["FechaAnticipo"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["FechaAnticipo"]);
            pedido.CondicionPago =
                ObtenerTexto(reader, "CondicionPago");
            pedido.PlazoEntrega =
                ObtenerTexto(reader, "PlazoEntrega");
            pedido.EntregaIncluida =
                Convert.ToBoolean(reader["EntregaIncluida"]);
            pedido.Observaciones =
                ObtenerTexto(reader, "Observaciones");
            pedido.Detalles =
                new List<DetallePedido>();

            return pedido;
        }

        private static void AgregarParametrosPedido(
            SqlCommand command,
            Pedido pedido)
        {
            command.Parameters.Add(
                "@Id", SqlDbType.UniqueIdentifier).Value = pedido.Id;
            command.Parameters.Add(
                "@Numero", SqlDbType.NVarChar, 50).Value =
                LimpiarTexto(pedido.Numero);
            command.Parameters.Add(
                "@ClienteId", SqlDbType.UniqueIdentifier).Value =
                pedido.ClienteId;
            command.Parameters.Add(
                "@PresupuestoId", SqlDbType.UniqueIdentifier).Value =
                pedido.PresupuestoId;
            command.Parameters.Add(
                "@FechaPedido", SqlDbType.DateTime2).Value =
                pedido.FechaPedido;
            command.Parameters.Add(
                "@Estado", SqlDbType.Int).Value = (int)pedido.Estado;
            command.Parameters.Add(
                "@Moneda", SqlDbType.Int).Value = (int)pedido.Moneda;

            AgregarDecimal(command, "@TipoCambio", pedido.TipoCambio);
            AgregarDecimal(command, "@PorcentajeIVA", pedido.PorcentajeIVA);
            AgregarDecimal(
                command,
                "@PorcentajeRecargo",
                pedido.PorcentajeRecargo);
            AgregarDecimal(
                command,
                "@PorcentajeDescuentoGeneral",
                pedido.PorcentajeDescuentoGeneral);
            AgregarDecimal(
                command,
                "@PorcentajeAnticipo",
                pedido.PorcentajeAnticipo);
            AgregarDecimal(
                command,
                "@ImporteAnticipo",
                pedido.ImporteAnticipo);

            command.Parameters.Add(
                "@MedioPagoAnticipo", SqlDbType.NVarChar, 100).Value =
                LimpiarTexto(pedido.MedioPagoAnticipo);
            command.Parameters.Add(
                "@ComprobanteAnticipo", SqlDbType.NVarChar, 100).Value =
                LimpiarTexto(pedido.ComprobanteAnticipo);
            command.Parameters.Add(
                "@FechaAnticipo", SqlDbType.DateTime2).Value =
                pedido.FechaAnticipo.HasValue
                    ? (object)pedido.FechaAnticipo.Value
                    : DBNull.Value;
            command.Parameters.Add(
                "@CondicionPago", SqlDbType.NVarChar, 250).Value =
                LimpiarTexto(pedido.CondicionPago);
            command.Parameters.Add(
                "@PlazoEntrega", SqlDbType.NVarChar, 250).Value =
                LimpiarTexto(pedido.PlazoEntrega);
            command.Parameters.Add(
                "@EntregaIncluida", SqlDbType.Bit).Value =
                pedido.EntregaIncluida;
            command.Parameters.Add(
                "@Observaciones", SqlDbType.NVarChar, 1000).Value =
                LimpiarTexto(pedido.Observaciones);
        }

        private static void AgregarDecimal(
            SqlCommand command,
            string nombre,
            decimal valor)
        {
            SqlParameter parametro =
                command.Parameters.Add(nombre, SqlDbType.Decimal);

            parametro.Precision = 18;
            parametro.Scale = 4;
            parametro.Value = valor;
        }

        private static string ObtenerTexto(
            SqlDataReader reader,
            string columna)
        {
            object valor = reader[columna];

            return valor == null || valor == DBNull.Value
                ? string.Empty
                : valor.ToString();
        }

        private static string LimpiarTexto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }

        private static void ValidarPedido(Pedido pedido)
        {
            if (pedido == null)
            {
                throw new ArgumentNullException(
                    nameof(pedido),
                    "El pedido no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(pedido.Numero))
            {
                throw new InvalidOperationException(
                    "El número del pedido es obligatorio.");
            }

            if (pedido.ClienteId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "El cliente del pedido no es válido.");
            }

            if (pedido.PresupuestoId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "El presupuesto del pedido no es válido.");
            }

            if (pedido.Detalles == null ||
                pedido.Detalles.Count == 0)
            {
                throw new InvalidOperationException(
                    "El pedido debe contener al menos un detalle.");
            }
        }
    }
}
