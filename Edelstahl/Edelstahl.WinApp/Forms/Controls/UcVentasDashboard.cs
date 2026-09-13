using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Comercial;
using Edelstahl.Services.Documents;
using Edelstahl.Services.Localization;
using Edelstahl.WinApp.Forms.Commercial;

namespace Edelstahl.WinApp.Forms.Controls
{
    public partial class UcVentasDashboard : UserControl
    {
        private readonly PedidoService _pedidoService;
        private readonly ComprobantePedidoPdfService _pdfService;

        private Label lblTitulo;
        private Label lblEstado;
        private Label lblClienteActual;
        private Label lblPresupuestoActual;
        private Label lblCredito;
        private Label lblFecha;
        private Label lblUsuario;
        private Label lblResumen;
        private Label lblSubtotal;
        private Label lblIva;
        private Label lblAnticipo;
        private Label lblSaldo;
        private Label lblItems;
        private Label lblTotal;
        private Label lblAyuda;

        private DataGridView dgvDetalle;
        private Button btnCliente;
        private Button btnPresupuesto;
        private Button btnNuevaOperacion;
        private Button btnConfirmar;
        private Button btnVerComprobante;
        private Button btnQuitarPresupuesto;

        private Cliente clienteSeleccionado;
        private Presupuesto presupuestoSeleccionado;
        private Pedido pedidoConfirmado;
        private string rutaComprobanteGenerado;

        public UcVentasDashboard()
        {
            InitializeComponent();

            _pedidoService = new PedidoService();
            _pdfService = new ComprobantePedidoPdfService();

            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(231, 236, 242);

            CrearInterfaz();
            SuscribirEventosIdioma();
            LimpiarOperacion();
            AplicarIdioma();
        }

        private string T(string clave)
        {
            return LanguageService.Translate(clave);
        }

        private void SuscribirEventosIdioma()
        {
            LanguageService.IdiomaCambiado += LanguageService_IdiomaCambiado;
            Disposed += UcVentasDashboard_Disposed;
        }

        private void LanguageService_IdiomaCambiado(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            AplicarIdioma();
        }

        private void UcVentasDashboard_Disposed(object sender, EventArgs e)
        {
            LanguageService.IdiomaCambiado -= LanguageService_IdiomaCambiado;
        }

        private void CrearInterfaz()
        {
            Controls.Clear();

            Panel pnlPedido = new Panel
            {
                Size = new Size(900, 550),
                Location = new Point(30, 30),
                BackColor = Color.FromArgb(222, 229, 238),
                BorderStyle = BorderStyle.None
            };

            Controls.Add(pnlPedido);

            CrearEncabezadoPedido(pnlPedido);
            CrearBotonesSeleccion(pnlPedido);
            CrearGrillaDetalle(pnlPedido);
            CrearPanelResumen();
        }

        private void CrearEncabezadoPedido(Panel pnlPedido)
        {
            lblTitulo = new Label
            {
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            pnlPedido.Controls.Add(lblTitulo);

            lblEstado = new Label
            {
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(25, 60),
                AutoSize = true
            };
            pnlPedido.Controls.Add(lblEstado);

            lblClienteActual = CrearLabel(pnlPedido, new Point(25, 95));
            lblPresupuestoActual = CrearLabel(pnlPedido, new Point(25, 120));

            lblCredito = CrearLabel(pnlPedido, new Point(25, 145));
            lblCredito.ForeColor = Color.ForestGreen;
            lblCredito.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            lblFecha = CrearLabel(pnlPedido, new Point(25, 170));
            lblUsuario = CrearLabel(pnlPedido, new Point(25, 195));
        }

        private static Label CrearLabel(Control padre, Point ubicacion)
        {
            Label label = new Label
            {
                Location = ubicacion,
                AutoSize = true
            };

            padre.Controls.Add(label);
            return label;
        }

        private void CrearBotonesSeleccion(Panel pnlPedido)
        {
            btnCliente = CrearBoton(
                new Point(20, 230),
                Color.FromArgb(173, 196, 255),
                Color.FromArgb(40, 40, 40));
            btnCliente.Click += BtnCliente_Click;
            pnlPedido.Controls.Add(btnCliente);

            btnPresupuesto = CrearBoton(
                new Point(220, 230),
                Color.FromArgb(190, 210, 255),
                Color.FromArgb(40, 40, 40));
            btnPresupuesto.Click += BtnPresupuesto_Click;
            pnlPedido.Controls.Add(btnPresupuesto);

            btnQuitarPresupuesto = CrearBoton(
                new Point(420, 230),
                Color.FromArgb(255, 205, 205),
                Color.FromArgb(90, 45, 45));
            btnQuitarPresupuesto.Enabled = false;
            btnQuitarPresupuesto.Click += BtnQuitarPresupuesto_Click;
            pnlPedido.Controls.Add(btnQuitarPresupuesto);

            btnNuevaOperacion = CrearBoton(
                new Point(620, 230),
                Color.FromArgb(211, 207, 239),
                Color.FromArgb(40, 40, 40));
            btnNuevaOperacion.Click += BtnNuevaOperacion_Click;
            pnlPedido.Controls.Add(btnNuevaOperacion);
        }

        private static Button CrearBoton(
            Point ubicacion,
            Color colorFondo,
            Color colorTexto)
        {
            return new Button
            {
                Size = new Size(180, 40),
                Location = ubicacion,
                BackColor = colorFondo,
                ForeColor = colorTexto,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        private void CrearGrillaDetalle(Panel pnlPedido)
        {
            dgvDetalle = new DataGridView
            {
                Location = new Point(20, 290),
                Size = new Size(850, 220),
                BackgroundColor = Color.FromArgb(246, 248, 252),
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvDetalle.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(173, 196, 255);
            dgvDetalle.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(40, 40, 40);
            dgvDetalle.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvDetalle.DefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);
            dgvDetalle.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(205, 219, 250);
            dgvDetalle.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(30, 30, 30);

            dgvDetalle.ColumnCount = 5;
            dgvDetalle.Columns[0].FillWeight = 70;
            dgvDetalle.Columns[1].FillWeight = 180;
            dgvDetalle.Columns[2].FillWeight = 65;
            dgvDetalle.Columns[3].FillWeight = 80;
            dgvDetalle.Columns[4].FillWeight = 80;

            pnlPedido.Controls.Add(dgvDetalle);
        }

        private void CrearPanelResumen()
        {
            Panel pnlResumen = new Panel
            {
                Size = new Size(320, 550),
                Location = new Point(960, 30),
                BackColor = Color.FromArgb(233, 243, 229),
                BorderStyle = BorderStyle.None
            };
            Controls.Add(pnlResumen);

            lblResumen = new Label
            {
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            pnlResumen.Controls.Add(lblResumen);

            lblSubtotal = CrearLabelResumen(pnlResumen, 80);
            lblIva = CrearLabelResumen(pnlResumen, 110);
            lblAnticipo = CrearLabelResumen(pnlResumen, 140);
            lblSaldo = CrearLabelResumen(pnlResumen, 170);
            lblItems = CrearLabelResumen(pnlResumen, 210);

            lblTotal = new Label
            {
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 250),
                AutoSize = true
            };
            pnlResumen.Controls.Add(lblTotal);

            lblAyuda = new Label
            {
                ForeColor = Color.DimGray,
                Location = new Point(20, 300),
                Size = new Size(275, 65)
            };
            pnlResumen.Controls.Add(lblAyuda);

            btnVerComprobante = new Button
            {
                Size = new Size(250, 45),
                Location = new Point(30, 385),
                BackColor = Color.FromArgb(151, 178, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnVerComprobante.FlatAppearance.BorderSize = 0;
            btnVerComprobante.Click += BtnVerComprobante_Click;
            pnlResumen.Controls.Add(btnVerComprobante);

            btnConfirmar = new Button
            {
                Size = new Size(250, 50),
                Location = new Point(30, 450),
                BackColor = Color.FromArgb(170, 178, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;
            pnlResumen.Controls.Add(btnConfirmar);
        }

        private static Label CrearLabelResumen(Panel panel, int posicionVertical)
        {
            return CrearLabel(panel, new Point(20, posicionVertical));
        }

        private void AplicarIdioma()
        {
            if (lblTitulo == null)
            {
                return;
            }

            lblTitulo.Text = T("ConfirmarPedido");
            lblResumen.Text = T("Resultado");
            btnCliente.Text = T("Cliente");
            btnPresupuesto.Text = T("Presupuesto");
            btnQuitarPresupuesto.Text = T("QuitarPresupuesto");
            btnNuevaOperacion.Text = T("NuevaOperacion");
            btnVerComprobante.Text = T("VerComprobante").ToUpperInvariant();

            dgvDetalle.Columns[0].Name = T("Codigo");
            dgvDetalle.Columns[1].Name = T("Descripcion");
            dgvDetalle.Columns[2].Name = T("Cantidad");
            dgvDetalle.Columns[3].Name = T("Precio");
            dgvDetalle.Columns[4].Name = T("Subtotal");

            lblFecha.Text = T("Fecha") + ": " +
                DateTime.Now.ToShortDateString();
            lblUsuario.Text = T("Usuario") + ": Dante Orihuela";

            ActualizarTextosEstado();
        }

        private void ActualizarTextosEstado()
        {
            if (pedidoConfirmado != null)
            {
                MostrarDatosPedidoConfirmado(false);
                return;
            }

            if (presupuestoSeleccionado != null)
            {
                MostrarDatosPresupuestoSeleccionado();
                return;
            }

            lblEstado.Text = T("Estado") + ": " + T("EstadoPendiente");
            lblEstado.ForeColor = Color.DarkOrange;

            lblClienteActual.Text = T("Cliente") + ": " +
                (clienteSeleccionado == null
                    ? T("SinSeleccionar")
                    : clienteSeleccionado.RazonSocial);

            lblPresupuestoActual.Text = T("Presupuesto") + ": " +
                T("SinSeleccionar");

            ActualizarCreditoCliente();
            LimpiarDatosPresupuesto();

            lblAyuda.Text = clienteSeleccionado == null
                ? T("SeleccionarCliente") + ". " + T("SeleccionarPresupuesto") + "."
                : T("SeleccionarPresupuesto") + ".";
        }

        private void BtnCliente_Click(object sender, EventArgs e)
        {
            using (FrmSeleccionCliente frm = new FrmSeleccionCliente())
            {
                if (frm.ShowDialog() != DialogResult.OK ||
                    frm.ClienteSeleccionado == null)
                {
                    return;
                }

                ReiniciarDatosOperacionAnterior();
                clienteSeleccionado = frm.ClienteSeleccionado;
                ActualizarTextosEstado();
            }
        }

        private void ReiniciarDatosOperacionAnterior()
        {
            presupuestoSeleccionado = null;
            pedidoConfirmado = null;
            rutaComprobanteGenerado = string.Empty;

            btnQuitarPresupuesto.Enabled = false;
            btnVerComprobante.Enabled = false;
            btnConfirmar.Enabled = false;
            btnConfirmar.BackColor = Color.FromArgb(170, 178, 185);
        }

        private void BtnPresupuesto_Click(object sender, EventArgs e)
        {
            if (clienteSeleccionado == null)
            {
                MostrarAdvertencia(
                    T("SeleccionarCliente") + ".",
                    T("Cliente"));
                return;
            }

            if (pedidoConfirmado != null)
            {
                MostrarAdvertencia(
                    T("PedidoConfirmado") + ". " + T("NuevaOperacion") + ".",
                    T("PedidoConfirmado"));
                return;
            }

            using (FrmSeleccionPresupuesto frm = new FrmSeleccionPresupuesto())
            {
                frm.CargarPresupuestos(clienteSeleccionado.Id);

                if (frm.ShowDialog() != DialogResult.OK ||
                    frm.PresupuestoSeleccionado == null)
                {
                    return;
                }

                presupuestoSeleccionado = frm.PresupuestoSeleccionado;
                pedidoConfirmado = null;
                rutaComprobanteGenerado = string.Empty;
                btnVerComprobante.Enabled = false;
                btnQuitarPresupuesto.Enabled = true;
                MostrarDatosPresupuestoSeleccionado();
            }
        }

        private void MostrarDatosPresupuestoSeleccionado()
        {
            if (presupuestoSeleccionado == null)
            {
                ActualizarTextosEstado();
                return;
            }

            lblClienteActual.Text = T("Cliente") + ": " +
                clienteSeleccionado.RazonSocial;
            lblPresupuestoActual.Text = T("Presupuesto") + ": " +
                presupuestoSeleccionado.Numero;
            lblEstado.Text = T("Estado") + ": " +
                (presupuestoSeleccionado.RequiereAnticipo()
                    ? T("AnticipoRequerido")
                    : T("ListoConfirmar"));
            lblEstado.ForeColor = presupuestoSeleccionado.RequiereAnticipo()
                ? Color.DarkOrange
                : Color.DarkGoldenrod;

            ActualizarCreditoCliente();
            CargarDatosPresupuesto();
        }

        private void BtnQuitarPresupuesto_Click(object sender, EventArgs e)
        {
            if (presupuestoSeleccionado == null)
            {
                MostrarAdvertencia(
                    T("Presupuesto") + ": " + T("SinSeleccionar"),
                    T("Presupuesto"));
                return;
            }

            if (pedidoConfirmado != null)
            {
                MostrarAdvertencia(
                    T("PedidoConfirmado"),
                    T("PedidoConfirmado"));
                return;
            }

            DialogResult resultado = MessageBox.Show(
                T("QuitarPresupuesto") + "?",
                T("QuitarPresupuesto"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                presupuestoSeleccionado = null;
                pedidoConfirmado = null;
                rutaComprobanteGenerado = string.Empty;
                btnQuitarPresupuesto.Enabled = false;
                btnVerComprobante.Enabled = false;
                ActualizarTextosEstado();
            }
        }

        private void CargarDatosPresupuesto()
        {
            dgvDetalle.Rows.Clear();

            if (presupuestoSeleccionado == null)
            {
                LimpiarDatosPresupuesto();
                return;
            }

            foreach (DetallePresupuesto detalle in presupuestoSeleccionado.Detalles)
            {
                dgvDetalle.Rows.Add(
                    detalle.Codigo,
                    detalle.Descripcion,
                    detalle.Cantidad.ToString("N2"),
                    FormatearImporte(detalle.PrecioUnitario),
                    FormatearImporte(detalle.CalcularSubtotal()));
            }

            lblSubtotal.Text = T("Subtotal") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularSubtotal());
            lblIva.Text = T("IVA") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularIVA());
            lblAnticipo.Text = T("Anticipo") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularAnticipo());
            lblSaldo.Text = T("SaldoFacturar") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularSaldoPendiente());
            lblItems.Text = T("Items") + ": " +
                presupuestoSeleccionado.Detalles.Count;
            lblTotal.Text = T("Total") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularTotal());

            lblAyuda.Text = presupuestoSeleccionado.RequiereAnticipo()
                ? T("AnticipoRequerido") + ". " + T("RegistrarSena") + "."
                : T("ListoConfirmar") + ".";

            btnConfirmar.Text = T("ConfirmarPedido").ToUpperInvariant();
            btnConfirmar.Enabled = presupuestoSeleccionado.PuedeConfirmarse();
            btnConfirmar.BackColor = btnConfirmar.Enabled
                ? Color.FromArgb(111, 192, 145)
                : Color.FromArgb(170, 178, 185);
        }

        private void ActualizarCreditoCliente()
        {
            decimal credito = clienteSeleccionado == null
                ? 0m
                : clienteSeleccionado.CalcularCreditoDisponible();

            lblCredito.Text = T("CreditoDisponible") + ": " +
                credito.ToString("C2");
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (clienteSeleccionado == null)
            {
                MostrarAdvertencia(T("SeleccionarCliente") + ".", T("Cliente"));
                return;
            }

            if (presupuestoSeleccionado == null)
            {
                MostrarAdvertencia(
                    T("SeleccionarPresupuesto") + ".",
                    T("Presupuesto"));
                return;
            }

            if (!presupuestoSeleccionado.PuedeConfirmarse())
            {
                MostrarAdvertencia(T("Validacion"), T("Presupuesto"));
                return;
            }

            decimal importeAnticipo = 0m;
            string medioPagoAnticipo = string.Empty;
            string comprobanteAnticipo = string.Empty;
            DateTime? fechaAnticipo = null;

            if (presupuestoSeleccionado.RequiereAnticipo())
            {
                using (FrmRegistrarSena frmSena = new FrmRegistrarSena(
                    clienteSeleccionado,
                    presupuestoSeleccionado))
                {
                    if (frmSena.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    importeAnticipo = frmSena.ImporteRegistrado;
                    medioPagoAnticipo = frmSena.MedioPagoRegistrado;
                    comprobanteAnticipo = frmSena.ComprobanteRegistrado;
                    fechaAnticipo = frmSena.FechaPagoRegistrada;
                }
            }

            DialogResult confirmacion = MessageBox.Show(
                CrearMensajeConfirmacion(importeAnticipo, medioPagoAnticipo),
                T("ConfirmarPedido"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                pedidoConfirmado = _pedidoService.ConfirmarDesdePresupuesto(
                    clienteSeleccionado.Id,
                    presupuestoSeleccionado.Id,
                    importeAnticipo,
                    medioPagoAnticipo,
                    comprobanteAnticipo,
                    fechaAnticipo);

                MostrarDatosPedidoConfirmado(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    T("ConfirmarPedido"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string CrearMensajeConfirmacion(
            decimal importeAnticipo,
            string medioPagoAnticipo)
        {
            string mensaje =
                T("ConfirmarPedido") + "?" + Environment.NewLine +
                Environment.NewLine +
                T("Cliente") + ": " + clienteSeleccionado.RazonSocial +
                Environment.NewLine +
                T("Presupuesto") + ": " + presupuestoSeleccionado.Numero +
                Environment.NewLine +
                T("Total") + ": " +
                FormatearImporte(presupuestoSeleccionado.CalcularTotal());

            if (importeAnticipo > 0m)
            {
                mensaje += Environment.NewLine +
                    T("AnticipoRegistrado") + ": " +
                    FormatearImporte(importeAnticipo) +
                    Environment.NewLine +
                    T("MedioPago") + ": " + medioPagoAnticipo;
            }

            return mensaje;
        }

        private void MostrarDatosPedidoConfirmado(bool mostrarMensaje)
        {
            if (pedidoConfirmado == null)
            {
                return;
            }

            lblEstado.Text = T("Estado") + ": " + T("PedidoConfirmado");
            lblEstado.ForeColor = Color.DarkGreen;
            lblClienteActual.Text = T("Cliente") + ": " +
                clienteSeleccionado.RazonSocial;
            lblPresupuestoActual.Text = T("Presupuesto") + ": " +
                presupuestoSeleccionado.Numero;
            lblAnticipo.Text = T("AnticipoRegistrado") + ": " +
                FormatearImporte(pedidoConfirmado.ImporteAnticipo);
            lblSaldo.Text = T("SaldoFacturar") + ": " +
                FormatearImporte(pedidoConfirmado.CalcularSaldoPendiente());
            lblSubtotal.Text = T("Subtotal") + ": " +
                FormatearImporte(pedidoConfirmado.CalcularSubtotal());
            lblIva.Text = T("IVA") + ": " +
                FormatearImporte(pedidoConfirmado.CalcularIVA());
            lblItems.Text = T("Items") + ": " + pedidoConfirmado.Detalles.Count;
            lblTotal.Text = T("Total") + ": " +
                FormatearImporte(pedidoConfirmado.CalcularTotal());
            lblAyuda.Text = T("PedidoConfirmado") + ": " +
                pedidoConfirmado.Numero + Environment.NewLine +
                T("VerComprobante") + ".";

            btnQuitarPresupuesto.Enabled = false;
            btnConfirmar.Enabled = false;
            btnConfirmar.Text = T("PedidoConfirmado").ToUpperInvariant();
            btnConfirmar.BackColor = Color.FromArgb(105, 166, 125);
            btnVerComprobante.Enabled = true;
            btnVerComprobante.Text = T("VerComprobante").ToUpperInvariant();

            if (mostrarMensaje)
            {
                MessageBox.Show(
                    T("PedidoConfirmado") + Environment.NewLine +
                    T("Numero") + ": " + pedidoConfirmado.Numero +
                    Environment.NewLine +
                    T("Total") + ": " +
                    FormatearImporte(pedidoConfirmado.CalcularTotal()),
                    T("PedidoConfirmado"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnVerComprobante_Click(object sender, EventArgs e)
        {
            if (pedidoConfirmado == null || clienteSeleccionado == null)
            {
                MostrarAdvertencia(T("ConfirmarPedido") + ".", T("VerComprobante"));
                return;
            }

            try
            {
                rutaComprobanteGenerado = _pdfService.Generar(
                    pedidoConfirmado,
                    clienteSeleccionado);

                ProcessStartInfo informacionProceso = new ProcessStartInfo
                {
                    FileName = rutaComprobanteGenerado,
                    UseShellExecute = true
                };

                Process.Start(informacionProceso);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    T("VerComprobante"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnNuevaOperacion_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                T("NuevaOperacion") + "?",
                T("NuevaOperacion"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                LimpiarOperacion();
            }
        }

        private void LimpiarOperacion()
        {
            clienteSeleccionado = null;
            presupuestoSeleccionado = null;
            pedidoConfirmado = null;
            rutaComprobanteGenerado = string.Empty;

            if (lblClienteActual == null)
            {
                return;
            }

            btnQuitarPresupuesto.Enabled = false;
            btnVerComprobante.Enabled = false;
            btnConfirmar.Enabled = false;
            btnConfirmar.BackColor = Color.FromArgb(170, 178, 185);

            ActualizarTextosEstado();
        }

        private void LimpiarDatosPresupuesto()
        {
            if (dgvDetalle != null)
            {
                dgvDetalle.Rows.Clear();
            }

            if (lblSubtotal == null)
            {
                return;
            }

            lblSubtotal.Text = T("Subtotal") + ": $0,00";
            lblIva.Text = T("IVA") + ": $0,00";
            lblAnticipo.Text = T("Anticipo") + ": $0,00";
            lblSaldo.Text = T("SaldoFacturar") + ": $0,00";
            lblItems.Text = T("Items") + ": 0";
            lblTotal.Text = T("Total") + ": $0,00";
            btnConfirmar.Text = T("ConfirmarPedido").ToUpperInvariant();
            btnVerComprobante.Text = T("VerComprobante").ToUpperInvariant();
        }

        private string FormatearImporte(decimal importe)
        {
            string moneda = presupuestoSeleccionado != null &&
                presupuestoSeleccionado.Moneda == Moneda.DolaresEstadounidenses
                    ? "USD "
                    : "$";

            return moneda + importe.ToString("N2");
        }

        private static void MostrarAdvertencia(string mensaje, string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
