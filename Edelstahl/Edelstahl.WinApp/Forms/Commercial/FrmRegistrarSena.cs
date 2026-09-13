using System;
using System.Drawing;
using System.Windows.Forms;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.WinApp.Forms.Commercial
{
    public partial class FrmRegistrarSena : Form
    {
        private readonly Cliente _cliente;
        private readonly Presupuesto _presupuesto;

        private NumericUpDown nudImporteRecibido;
        private ComboBox cboMedioPago;
        private TextBox txtComprobante;
        private DateTimePicker dtpFechaPago;
        private Label lblResultado;
        private Button btnRegistrar;

        public decimal ImporteRegistrado
        {
            get;
            private set;
        }

        public string MedioPagoRegistrado
        {
            get;
            private set;
        }

        public string ComprobanteRegistrado
        {
            get;
            private set;
        }

        public DateTime FechaPagoRegistrada
        {
            get;
            private set;
        }

        public FrmRegistrarSena(
            Cliente cliente,
            Presupuesto presupuesto)
        {
            InitializeComponent();

            _cliente =
                cliente
                ?? throw new ArgumentNullException(
                    nameof(cliente));

            _presupuesto =
                presupuesto
                ?? throw new ArgumentNullException(
                    nameof(presupuesto));

            ConfigurarFormulario();
            CrearInterfaz();
            CargarDatos();
        }

        private void ConfigurarFormulario()
        {
            Text =
                "Registrar Seña";

            Width =
                720;

            Height =
                650;

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.FromArgb(231, 236, 242);

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox =
                false;

            MinimizeBox =
                false;

            ShowInTaskbar =
                false;

            AutoScaleMode =
                AutoScaleMode.Font;
        }

        private void CrearInterfaz()
        {
            Controls.Clear();

            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                "Registrar Seña";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    18,
                    FontStyle.Bold);

            lblTitulo.ForeColor =
                Color.FromArgb(54, 69, 98);

            lblTitulo.AutoSize =
                true;

            lblTitulo.Location =
                new Point(30, 25);

            Controls.Add(
                lblTitulo);

            Label lblDescripcion =
                new Label();

            lblDescripcion.Text =
                "Ingrese el anticipo exacto requerido " +
                "para confirmar el pedido.";

            lblDescripcion.Font =
                new Font(
                    "Segoe UI",
                    10);

            lblDescripcion.ForeColor =
                Color.DimGray;

            lblDescripcion.AutoSize =
                true;

            lblDescripcion.Location =
                new Point(32, 70);

            Controls.Add(
                lblDescripcion);

            Panel pnlDatos =
                new Panel();

            pnlDatos.Size =
                new Size(640, 180);

            pnlDatos.Location =
                new Point(30, 110);

            pnlDatos.BackColor =
                Color.FromArgb(222, 229, 238);

            pnlDatos.BorderStyle =
                BorderStyle.None;

            Controls.Add(
                pnlDatos);

            CrearDatoResumen(
                pnlDatos,
                "Cliente:",
                _cliente.RazonSocial,
                20);

            CrearDatoResumen(
                pnlDatos,
                "Presupuesto:",
                _presupuesto.Numero,
                55);

            CrearDatoResumen(
                pnlDatos,
                "Total:",
                FormatearImporte(
                    _presupuesto.CalcularTotal()),
                90);

            CrearDatoResumen(
                pnlDatos,
                "Anticipo requerido:",
                FormatearImporte(
                    _presupuesto.CalcularAnticipo()),
                125);

            Label lblImporte =
                CrearEtiquetaCampo(
                    "Importe recibido",
                    30,
                    320);

            Controls.Add(
                lblImporte);

            nudImporteRecibido =
                new NumericUpDown();

            nudImporteRecibido.Location =
                new Point(30, 350);

            nudImporteRecibido.Size =
                new Size(300, 32);

            nudImporteRecibido.Font =
                new Font(
                    "Segoe UI",
                    11);

            nudImporteRecibido.DecimalPlaces =
                2;

            nudImporteRecibido.ThousandsSeparator =
                true;

            nudImporteRecibido.Minimum =
                0m;

            decimal anticipoRequerido =
                _presupuesto.CalcularAnticipo();

            nudImporteRecibido.Maximum =
                anticipoRequerido > 0m
                    ? anticipoRequerido
                    : 0m;

            nudImporteRecibido.Increment =
                1m;

            Controls.Add(
                nudImporteRecibido);

            Label lblMedioPago =
                CrearEtiquetaCampo(
                    "Medio de pago",
                    360,
                    320);

            Controls.Add(
                lblMedioPago);

            cboMedioPago =
                new ComboBox();

            cboMedioPago.Location =
                new Point(360, 350);

            cboMedioPago.Size =
                new Size(310, 32);

            cboMedioPago.Font =
                new Font(
                    "Segoe UI",
                    10);

            cboMedioPago.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboMedioPago.Items.Add(
                "Transferencia bancaria");

            cboMedioPago.Items.Add(
                "Depósito bancario");

            cboMedioPago.Items.Add(
                "Cheque");

            cboMedioPago.Items.Add(
                "Efectivo");

            cboMedioPago.Items.Add(
                "Tarjeta");

            cboMedioPago.SelectedIndex =
                -1;

            Controls.Add(
                cboMedioPago);

            Label lblComprobante =
                CrearEtiquetaCampo(
                    "Número de comprobante",
                    30,
                    405);

            Controls.Add(
                lblComprobante);

            txtComprobante =
                new TextBox();

            txtComprobante.Location =
                new Point(30, 435);

            txtComprobante.Size =
                new Size(300, 32);

            txtComprobante.Font =
                new Font(
                    "Segoe UI",
                    10);

            txtComprobante.ReadOnly =
                true;

            txtComprobante.BackColor =
                Color.FromArgb(245, 247, 250);

            Controls.Add(
                txtComprobante);

            Label lblFecha =
                CrearEtiquetaCampo(
                    "Fecha de pago",
                    360,
                    405);

            Controls.Add(
                lblFecha);

            dtpFechaPago =
                new DateTimePicker();

            dtpFechaPago.Location =
                new Point(360, 435);

            dtpFechaPago.Size =
                new Size(310, 32);

            dtpFechaPago.Font =
                new Font(
                    "Segoe UI",
                    10);

            dtpFechaPago.Format =
                DateTimePickerFormat.Short;

            dtpFechaPago.Value =
                DateTime.Today;

            dtpFechaPago.MaxDate =
                DateTime.Today;

            Controls.Add(
                dtpFechaPago);

            lblResultado =
                new Label();

            lblResultado.Text =
                "Seña pendiente";

            lblResultado.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            lblResultado.ForeColor =
                Color.FromArgb(90, 90, 90);

            lblResultado.BackColor =
                Color.FromArgb(255, 235, 190);

            lblResultado.TextAlign =
                ContentAlignment.MiddleCenter;

            lblResultado.Location =
                new Point(30, 495);

            lblResultado.Size =
                new Size(640, 40);

            Controls.Add(
                lblResultado);

            Button btnCancelar =
                new Button();

            btnCancelar.Text =
                "Cancelar";

            btnCancelar.Size =
                new Size(150, 45);

            btnCancelar.Location =
                new Point(350, 555);

            btnCancelar.BackColor =
                Color.FromArgb(210, 215, 225);

            btnCancelar.ForeColor =
                Color.FromArgb(50, 50, 50);

            btnCancelar.FlatStyle =
                FlatStyle.Flat;

            btnCancelar.FlatAppearance.BorderSize =
                0;

            btnCancelar.Cursor =
                Cursors.Hand;

            btnCancelar.Click +=
                BtnCancelar_Click;

            Controls.Add(
                btnCancelar);

            btnRegistrar =
                new Button();

            btnRegistrar.Text =
                "REGISTRAR SEÑA";

            btnRegistrar.Size =
                new Size(170, 45);

            btnRegistrar.Location =
                new Point(510, 555);

            btnRegistrar.BackColor =
                Color.FromArgb(111, 192, 145);

            btnRegistrar.ForeColor =
                Color.White;

            btnRegistrar.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            btnRegistrar.FlatStyle =
                FlatStyle.Flat;

            btnRegistrar.FlatAppearance.BorderSize =
                0;

            btnRegistrar.Cursor =
                Cursors.Hand;

            btnRegistrar.Click +=
                BtnRegistrar_Click;

            Controls.Add(
                btnRegistrar);

            AcceptButton =
                btnRegistrar;

            CancelButton =
                btnCancelar;
        }

        private void CargarDatos()
        {
            decimal anticipoRequerido =
                _presupuesto.CalcularAnticipo();

            nudImporteRecibido.Maximum =
                anticipoRequerido;

            nudImporteRecibido.Value =
                anticipoRequerido;

            txtComprobante.Text =
                GenerarNumeroComprobante();

            ImporteRegistrado =
                0m;

            MedioPagoRegistrado =
                string.Empty;

            ComprobanteRegistrado =
                string.Empty;

            FechaPagoRegistrada =
                DateTime.Today;
        }

        private void BtnRegistrar_Click(
            object sender,
            EventArgs e)
        {
            decimal importeRequerido =
                _presupuesto.CalcularAnticipo();

            decimal importeRecibido =
                nudImporteRecibido.Value;

            if (importeRequerido <= 0m)
            {
                MostrarAdvertencia(
                    "El presupuesto seleccionado no requiere anticipo.",
                    "Seña no requerida");

                return;
            }

            if (importeRecibido <
                importeRequerido)
            {
                MostrarAdvertencia(
                    "El importe recibido no alcanza. " +
                    "El anticipo requerido es " +
                    FormatearImporte(
                        importeRequerido) +
                    ".",
                    "Importe insuficiente");

                nudImporteRecibido.Focus();

                return;
            }

            if (importeRecibido >
                importeRequerido)
            {
                MostrarAdvertencia(
                    "El importe recibido no puede superar " +
                    "el anticipo requerido de " +
                    FormatearImporte(
                        importeRequerido) +
                    ".",
                    "Importe superior al requerido");

                nudImporteRecibido.Value =
                    importeRequerido;

                nudImporteRecibido.Focus();

                return;
            }

            if (cboMedioPago.SelectedIndex < 0)
            {
                MostrarAdvertencia(
                    "Debe seleccionar un medio de pago.",
                    "Medio de pago requerido");

                cboMedioPago.Focus();

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtComprobante.Text))
            {
                MostrarAdvertencia(
                    "No se pudo generar el número de comprobante.",
                    "Comprobante requerido");

                return;
            }

            if (dtpFechaPago.Value.Date >
                DateTime.Today)
            {
                MostrarAdvertencia(
                    "La fecha del pago no puede ser futura.",
                    "Fecha incorrecta");

                dtpFechaPago.Focus();

                return;
            }

            ImporteRegistrado =
                importeRecibido;

            MedioPagoRegistrado =
                cboMedioPago.SelectedItem
                    .ToString();

            ComprobanteRegistrado =
                txtComprobante.Text.Trim();

            FechaPagoRegistrada =
                dtpFechaPago.Value.Date;

            lblResultado.Text =
                "SEÑA REGISTRADA CORRECTAMENTE";

            lblResultado.ForeColor =
                Color.White;

            lblResultado.BackColor =
                Color.FromArgb(111, 192, 145);

            btnRegistrar.Enabled =
                false;

            MessageBox.Show(
                "La seña fue registrada correctamente." +
                Environment.NewLine +
                Environment.NewLine +
                "Importe: " +
                FormatearImporte(
                    ImporteRegistrado) +
                Environment.NewLine +
                "Comprobante: " +
                ComprobanteRegistrado,
                "Seña registrada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult =
                DialogResult.OK;

            Close();
        }

        private void BtnCancelar_Click(
            object sender,
            EventArgs e)
        {
            DialogResult =
                DialogResult.Cancel;

            Close();
        }

        private static Label CrearEtiquetaCampo(
            string texto,
            int posicionHorizontal,
            int posicionVertical)
        {
            Label label =
                new Label();

            label.Text =
                texto;

            label.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            label.ForeColor =
                Color.FromArgb(60, 60, 60);

            label.AutoSize =
                true;

            label.Location =
                new Point(
                    posicionHorizontal,
                    posicionVertical);

            return label;
        }

        private static void CrearDatoResumen(
            Panel panel,
            string titulo,
            string valor,
            int posicionVertical)
        {
            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                titulo;

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            lblTitulo.ForeColor =
                Color.FromArgb(60, 60, 60);

            lblTitulo.Location =
                new Point(
                    20,
                    posicionVertical);

            lblTitulo.Size =
                new Size(170, 25);

            panel.Controls.Add(
                lblTitulo);

            Label lblValor =
                new Label();

            lblValor.Text =
                valor;

            lblValor.Font =
                new Font(
                    "Segoe UI",
                    9);

            lblValor.ForeColor =
                Color.FromArgb(40, 40, 40);

            lblValor.Location =
                new Point(
                    190,
                    posicionVertical);

            lblValor.Size =
                new Size(420, 25);

            panel.Controls.Add(
                lblValor);
        }

        private string FormatearImporte(
            decimal importe)
        {
            string moneda =
                _presupuesto.Moneda ==
                Moneda.DolaresEstadounidenses
                    ? "USD "
                    : "$";

            return moneda +
                importe.ToString("N2");
        }

        private static string GenerarNumeroComprobante()
        {
            string codigo =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            return string.Format(
                "SENA-{0}-{1}",
                DateTime.Today.Year,
                codigo);
        }

        private static void MostrarAdvertencia(
            string mensaje,
            string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}