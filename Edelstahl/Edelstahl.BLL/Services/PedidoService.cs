using System;
using System.Collections.Generic;
using System.Linq;
using Edelstahl.DAL.Factory;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.BLL.Services
{
    /// <summary>
    /// Coordina la creación, validación y persistencia
    /// de los pedidos comerciales.
    /// </summary>
    public class PedidoService
    {
        private readonly IPedidoRepository
            _pedidoRepository;

        private readonly IPresupuestoRepository
            _presupuestoRepository;

        private readonly IClienteRepository
            _clienteRepository;

        public PedidoService()
        {
            _pedidoRepository =
                FactoryDataAccess.PedidoRepository;

            _presupuestoRepository =
                FactoryDataAccess.PresupuestoRepository;

            _clienteRepository =
                FactoryDataAccess.ClienteRepository;
        }

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IPresupuestoRepository presupuestoRepository,
            IClienteRepository clienteRepository)
        {
            _pedidoRepository =
                pedidoRepository
                ?? throw new ArgumentNullException(
                    nameof(pedidoRepository));

            _presupuestoRepository =
                presupuestoRepository
                ?? throw new ArgumentNullException(
                    nameof(presupuestoRepository));

            _clienteRepository =
                clienteRepository
                ?? throw new ArgumentNullException(
                    nameof(clienteRepository));
        }

        public Pedido ConfirmarDesdePresupuesto(
            Guid clienteId,
            Guid presupuestoId,
            decimal importeAnticipo = 0m,
            string medioPagoAnticipo = "",
            string comprobanteAnticipo = "",
            DateTime? fechaAnticipo = null)
        {
            Cliente cliente =
                ObtenerClienteValido(
                    clienteId);

            Presupuesto presupuesto =
                ObtenerPresupuestoValido(
                    presupuestoId,
                    cliente.Id);

            ValidarPedidoNoExistente(
                presupuesto.Id);

            ValidarAnticipo(
                presupuesto,
                importeAnticipo,
                medioPagoAnticipo,
                comprobanteAnticipo,
                fechaAnticipo);

            ValidarCredito(
                cliente,
                presupuesto,
                importeAnticipo);

            Pedido pedido =
                CrearPedidoDesdePresupuesto(
                    cliente,
                    presupuesto,
                    importeAnticipo,
                    medioPagoAnticipo,
                    comprobanteAnticipo,
                    fechaAnticipo);

            pedido.Confirmar();

            _pedidoRepository.Add(
                pedido);

            presupuesto.Estado =
                EstadoPresupuesto
                    .ConvertidoEnPedido;

            _presupuestoRepository.Update(
                presupuesto);

            return pedido;
        }

        public Pedido ObtenerPorId(
            Guid pedidoId)
        {
            if (pedidoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del pedido no es válido.",
                    nameof(pedidoId));
            }

            return _pedidoRepository.GetById(
                pedidoId);
        }

        public Pedido ObtenerPorNumero(
            string numero)
        {
            if (string.IsNullOrWhiteSpace(
                numero))
            {
                return null;
            }

            return _pedidoRepository.GetByNumero(
                numero.Trim().ToUpperInvariant());
        }

        public Pedido ObtenerPorPresupuesto(
            Guid presupuestoId)
        {
            if (presupuestoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del presupuesto no es válido.",
                    nameof(presupuestoId));
            }

            return _pedidoRepository
                .GetByPresupuestoId(
                    presupuestoId);
        }

        public List<Pedido> ObtenerTodos()
        {
            return _pedidoRepository
                .GetAll()
                .OrderByDescending(
                    pedido =>
                        pedido.FechaPedido)
                .ToList();
        }

        public List<Pedido> ObtenerPorCliente(
            Guid clienteId)
        {
            if (clienteId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(clienteId));
            }

            return _pedidoRepository
                .GetByClienteId(
                    clienteId)
                .OrderByDescending(
                    pedido =>
                        pedido.FechaPedido)
                .ToList();
        }

        private Cliente ObtenerClienteValido(
            Guid clienteId)
        {
            if (clienteId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(clienteId));
            }

            Cliente cliente =
                _clienteRepository.GetById(
                    clienteId);

            if (cliente == null)
            {
                throw new InvalidOperationException(
                    "No se encontró el cliente seleccionado.");
            }

            if (!cliente.Activo)
            {
                throw new InvalidOperationException(
                    "El cliente seleccionado no se encuentra activo.");
            }

            return cliente;
        }

        private Presupuesto ObtenerPresupuestoValido(
            Guid presupuestoId,
            Guid clienteId)
        {
            if (presupuestoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del presupuesto no es válido.",
                    nameof(presupuestoId));
            }

            Presupuesto presupuesto =
                _presupuestoRepository.GetById(
                    presupuestoId);

            if (presupuesto == null)
            {
                throw new InvalidOperationException(
                    "No se encontró el presupuesto seleccionado.");
            }

            if (presupuesto.ClienteId !=
                clienteId)
            {
                throw new InvalidOperationException(
                    "El presupuesto no pertenece al cliente seleccionado.");
            }

            if (!presupuesto.PuedeConfirmarse())
            {
                throw new InvalidOperationException(
                    "El presupuesto no puede confirmarse. " +
                    "Debe estar vigente, aceptado y contener detalles.");
            }

            return presupuesto;
        }

        private void ValidarPedidoNoExistente(
            Guid presupuestoId)
        {
            if (_pedidoRepository
                .ExistsByPresupuestoId(
                    presupuestoId))
            {
                throw new InvalidOperationException(
                    "El presupuesto seleccionado ya fue convertido en pedido.");
            }
        }

        private static void ValidarAnticipo(
            Presupuesto presupuesto,
            decimal importeAnticipo,
            string medioPagoAnticipo,
            string comprobanteAnticipo,
            DateTime? fechaAnticipo)
        {
            if (importeAnticipo < 0m)
            {
                throw new InvalidOperationException(
                    "El importe del anticipo no puede ser negativo.");
            }

            decimal anticipoRequerido =
                presupuesto.CalcularAnticipo();

            decimal margenRedondeo =
                0.01m;

            if (!presupuesto.RequiereAnticipo())
            {
                if (importeAnticipo > 0m)
                {
                    throw new InvalidOperationException(
                        "El presupuesto no requiere anticipo.");
                }

                return;
            }

            if (importeAnticipo <
                anticipoRequerido - margenRedondeo)
            {
                throw new InvalidOperationException(
                    "El anticipo ingresado es insuficiente. " +
                    "Debe registrarse exactamente el importe requerido.");
            }

            if (importeAnticipo >
                anticipoRequerido + margenRedondeo)
            {
                throw new InvalidOperationException(
                    "El anticipo ingresado supera el importe requerido. " +
                    "No se permite utilizar una seña mayor para evitar " +
                    "la validación de crédito.");
            }

            if (string.IsNullOrWhiteSpace(
                medioPagoAnticipo))
            {
                throw new InvalidOperationException(
                    "Debe indicar el medio de pago del anticipo.");
            }

            if (string.IsNullOrWhiteSpace(
                comprobanteAnticipo))
            {
                throw new InvalidOperationException(
                    "Debe indicar el comprobante del anticipo.");
            }

            if (!fechaAnticipo.HasValue)
            {
                throw new InvalidOperationException(
                    "Debe indicar la fecha del anticipo.");
            }

            if (fechaAnticipo.Value.Date >
                DateTime.Today)
            {
                throw new InvalidOperationException(
                    "La fecha del anticipo no puede ser futura.");
            }
        }

        private static void ValidarCredito(
            Cliente cliente,
            Presupuesto presupuesto,
            decimal importeAnticipo)
        {
            decimal creditoDisponible =
                cliente.CalcularCreditoDisponible();

            decimal totalEnPesos =
                presupuesto.CalcularTotalEnPesos();

            decimal anticipoPermitido =
                presupuesto.RequiereAnticipo()
                    ? presupuesto.CalcularAnticipo()
                    : 0m;

            if (importeAnticipo >
                anticipoPermitido)
            {
                importeAnticipo =
                    anticipoPermitido;
            }

            decimal anticipoEnPesos =
                importeAnticipo;

            if (presupuesto.Moneda ==
                Moneda.DolaresEstadounidenses)
            {
                if (presupuesto.TipoCambio <= 0m)
                {
                    throw new InvalidOperationException(
                        "El tipo de cambio del presupuesto no es válido.");
                }

                anticipoEnPesos =
                    importeAnticipo *
                    presupuesto.TipoCambio;
            }

            decimal creditoNecesario =
                totalEnPesos -
                anticipoEnPesos;

            if (creditoNecesario < 0m)
            {
                creditoNecesario =
                    0m;
            }

            if (creditoDisponible <
                creditoNecesario)
            {
                throw new InvalidOperationException(
                    "El crédito disponible del cliente no alcanza " +
                    "para confirmar el pedido." +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Crédito disponible: $" +
                    creditoDisponible.ToString("N2") +
                    Environment.NewLine +
                    "Crédito necesario: $" +
                    creditoNecesario.ToString("N2") +
                    Environment.NewLine +
                    "Anticipo permitido: " +
                    ObtenerPrefijoMoneda(
                        presupuesto.Moneda) +
                    anticipoPermitido.ToString("N2"));
            }
        }

        private Pedido CrearPedidoDesdePresupuesto(
            Cliente cliente,
            Presupuesto presupuesto,
            decimal importeAnticipo,
            string medioPagoAnticipo,
            string comprobanteAnticipo,
            DateTime? fechaAnticipo)
        {
            Pedido pedido =
                new Pedido
                {
                    Numero =
                        GenerarNumeroPedido(),

                    ClienteId =
                        cliente.Id,

                    PresupuestoId =
                        presupuesto.Id,

                    FechaPedido =
                        DateTime.Now,

                    Estado =
                        EstadoPedido.Pendiente,

                    Moneda =
                        presupuesto.Moneda,

                    TipoCambio =
                        presupuesto.TipoCambio,

                    PorcentajeIVA =
                        presupuesto.PorcentajeIVA,

                    PorcentajeRecargo =
                        presupuesto.PorcentajeRecargo,

                    PorcentajeDescuentoGeneral =
                        presupuesto
                            .PorcentajeDescuentoGeneral,

                    PorcentajeAnticipo =
                        presupuesto.PorcentajeAnticipo,

                    ImporteAnticipo =
                        importeAnticipo,

                    MedioPagoAnticipo =
                        medioPagoAnticipo
                        ?? string.Empty,

                    ComprobanteAnticipo =
                        comprobanteAnticipo
                        ?? string.Empty,

                    FechaAnticipo =
                        importeAnticipo > 0m
                            ? fechaAnticipo
                            : null,

                    CondicionPago =
                        presupuesto.CondicionPago
                        ?? string.Empty,

                    PlazoEntrega =
                        presupuesto.PlazoEntrega
                        ?? string.Empty,

                    EntregaIncluida =
                        presupuesto.EntregaIncluida,

                    Observaciones =
                        presupuesto.Observaciones
                        ?? string.Empty
                };

            foreach (DetallePresupuesto detalle
                in presupuesto.Detalles)
            {
                DetallePedido detallePedido =
                    new DetallePedido
                    {
                        DetallePresupuestoId =
                            detalle.Id,

                        Codigo =
                            detalle.Codigo
                            ?? string.Empty,

                        Descripcion =
                            detalle.Descripcion
                            ?? string.Empty,

                        DescripcionTecnica =
                            detalle.DescripcionTecnica
                            ?? string.Empty,

                        TipoItem =
                            detalle.TipoItem,

                        Cantidad =
                            detalle.Cantidad,

                        PrecioUnitario =
                            detalle.PrecioUnitario,

                        PorcentajeDescuento =
                            detalle.PorcentajeDescuento,

                        RequiereFabricacion =
                            detalle.TipoItem ==
                            TipoItemPresupuesto
                                .FabricacionEspecial
                    };

                pedido.Detalles.Add(
                    detallePedido);
            }

            return pedido;
        }

        private string GenerarNumeroPedido()
        {
            string numero;

            do
            {
                string codigo =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 6)
                        .ToUpperInvariant();

                numero =
                    string.Format(
                        "PED-{0}-{1}",
                        DateTime.Today.Year,
                        codigo);
            }
            while (_pedidoRepository
                .ExistsByNumero(
                    numero));

            return numero;
        }

        private static string ObtenerPrefijoMoneda(
            Moneda moneda)
        {
            return moneda ==
                Moneda.DolaresEstadounidenses
                    ? "USD "
                    : "$";
        }
    }
}