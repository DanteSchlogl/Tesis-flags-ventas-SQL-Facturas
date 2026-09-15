using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Comercial;
using Edelstahl.Services.Localization;

namespace Edelstahl.WinApp.Forms.Commercial
{
    public partial class FrmSeleccionPresupuesto : Form
    {
        private readonly PresupuestoService _presupuestoService;

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblResultado;

        private TextBox txtBuscar;

        private DataGridView dgvPresupuestos;

        private Button btnBuscar;
        private Button btnActualizar;
        private Button btnCancelar;
        private Button btnSeleccionar;

        private List<Presupuesto> presupuestosCargados;

        public Presupuesto PresupuestoSeleccionado
        {
            get;
            private set;
        }

        public Guid ClienteId
        {
            get;
            private set;
        }

        public FrmSeleccionPresupuesto()
        {
            InitializeComponent();

            _presupuestoService =
                new PresupuestoService();

            presupuestosCargados =
                new List<Presupuesto>();

            ConfigurarFormulario();
            ConfigurarEventosIdioma();
            AplicarIdioma();
        }

        private void ConfigurarFormulario()
        {
            Width =
                1100;

            Height =
                650;

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.FromArgb(
                    231,
                    236,
                    242);

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox =
                false;

            MinimizeBox =
                false;

            CrearControles();

            txtBuscar.Focus();
        }

        private void ConfigurarEventosIdioma()
        {
            LanguageService.IdiomaCambiado +=
                LanguageService_IdiomaCambiado;

            FormClosed +=
                FrmSeleccionPresupuesto_FormClosed;
        }

        private void CrearControles()
        {
            Controls.Clear();

            lblTitulo =
                new Label();

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold);

            lblTitulo.ForeColor =
                Color.FromArgb(
                    54,
                    69,
                    98);

            lblTitulo.AutoSize =
                true;

            lblTitulo.Location =
                new Point(
                    25,
                    20);

            Controls.Add(
                lblTitulo);

            lblSubtitulo =
                new Label();

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            lblSubtitulo.ForeColor =
                Color.FromArgb(
                    70,
                    70,
                    70);

            lblSubtitulo.AutoSize =
                true;

            lblSubtitulo.Location =
                new Point(
                    25,
                    75);

            Controls.Add(
                lblSubtitulo);

            txtBuscar =
                new TextBox();

            txtBuscar.Name =
                "txtBuscar";

            txtBuscar.Location =
                new Point(
                    25,
                    115);

            txtBuscar.Size =
                new Size(
                    500,
                    30);

            txtBuscar.Font =
                new Font(
                    "Segoe UI",
                    10);

            txtBuscar.KeyDown +=
                TxtBuscar_KeyDown;

            Controls.Add(
                txtBuscar);

            btnBuscar =
                new Button();

            btnBuscar.Name =
                "btnBuscar";

            btnBuscar.Location =
                new Point(
                    545,
                    112);

            btnBuscar.Size =
                new Size(
                    130,
                    35);

            btnBuscar.BackColor =
                Color.FromArgb(
                    173,
                    196,
                    255);

            btnBuscar.ForeColor =
                Color.FromArgb(
                    40,
                    40,
                    40);

            btnBuscar.FlatStyle =
                FlatStyle.Flat;

            btnBuscar.FlatAppearance.BorderSize =
                0;

            btnBuscar.Cursor =
                Cursors.Hand;

            btnBuscar.Click +=
                BtnBuscar_Click;

            Controls.Add(
                btnBuscar);

            btnActualizar =
                new Button();

            btnActualizar.Name =
                "btnActualizar";

            btnActualizar.Location =
                new Point(
                    690,
                    112);

            btnActualizar.Size =
                new Size(
                    130,
                    35);

            btnActualizar.BackColor =
                Color.FromArgb(
                    211,
                    207,
                    239);

            btnActualizar.ForeColor =
                Color.FromArgb(
                    40,
                    40,
                    40);

            btnActualizar.FlatStyle =
                FlatStyle.Flat;

            btnActualizar.FlatAppearance.BorderSize =
                0;

            btnActualizar.Cursor =
                Cursors.Hand;

            btnActualizar.Click +=
                BtnActualizar_Click;

            Controls.Add(
                btnActualizar);

            dgvPresupuestos =
                new DataGridView();

            dgvPresupuestos.Name =
                "dgvPresupuestos";

            dgvPresupuestos.Location =
                new Point(
                    25,
                    175);

            dgvPresupuestos.Size =
                new Size(
                    1025,
                    330);

            dgvPresupuestos.RowHeadersVisible =
                false;

            dgvPresupuestos.AllowUserToAddRows =
                false;

            dgvPresupuestos.AllowUserToDeleteRows =
                false;

            dgvPresupuestos.AllowUserToResizeRows =
                false;

            dgvPresupuestos.ReadOnly =
                true;

            dgvPresupuestos.MultiSelect =
                false;

            dgvPresupuestos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvPresupuestos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvPresupuestos.BackgroundColor =
                Color.FromArgb(
                    246,
                    248,
                    252);

            dgvPresupuestos.BorderStyle =
                BorderStyle.None;

            dgvPresupuestos.EnableHeadersVisualStyles =
                false;

            dgvPresupuestos.ColumnHeadersHeight =
                42;

            dgvPresupuestos
                .ColumnHeadersDefaultCellStyle
                .BackColor =
                    Color.FromArgb(
                        173,
                        196,
                        255);

            dgvPresupuestos
                .ColumnHeadersDefaultCellStyle
                .ForeColor =
                    Color.FromArgb(
                        40,
                        40,
                        40);

            dgvPresupuestos
                .ColumnHeadersDefaultCellStyle
                .Font =
                    new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold);

            dgvPresupuestos
                .DefaultCellStyle
                .BackColor =
                    Color.FromArgb(
                        246,
                        248,
                        252);

            dgvPresupuestos
                .DefaultCellStyle
                .SelectionBackColor =
                    Color.FromArgb(
                        205,
                        219,
                        250);

            dgvPresupuestos
                .DefaultCellStyle
                .SelectionForeColor =
                    Color.FromArgb(
                        30,
                        30,
                        30);

            /*
             * La primera columna contiene el Id
             * interno del presupuesto.
             *
             * Permanece oculta porque solamente se
             * utiliza para identificar correctamente
             * el presupuesto seleccionado.
             */
            dgvPresupuestos.ColumnCount =
                6;

            dgvPresupuestos.Columns[0].Name =
                "Id";

            dgvPresupuestos.Columns[0].Visible =
                false;

            dgvPresupuestos.Columns[1].Name =
                "Numero";

            dgvPresupuestos.Columns[2].Name =
                "Fecha";

            dgvPresupuestos.Columns[3].Name =
                "Estado";

            dgvPresupuestos.Columns[4].Name =
                "Total";

            dgvPresupuestos.Columns[5].Name =
                "Vencimiento";

            dgvPresupuestos.CellDoubleClick +=
                DgvPresupuestos_CellDoubleClick;

            Controls.Add(
                dgvPresupuestos);

            lblResultado =
                new Label();

            lblResultado.ForeColor =
                Color.DimGray;

            lblResultado.Font =
                new Font(
                    "Segoe UI",
                    9);

            lblResultado.AutoSize =
                true;

            lblResultado.Location =
                new Point(
                    25,
                    525);

            Controls.Add(
                lblResultado);

            btnCancelar =
                new Button();

            btnCancelar.Name =
                "btnCancelar";

            btnCancelar.Size =
                new Size(
                    150,
                    45);

            btnCancelar.Location =
                new Point(
                    730,
                    545);

            btnCancelar.BackColor =
                Color.FromArgb(
                    210,
                    215,
                    225);

            btnCancelar.ForeColor =
                Color.FromArgb(
                    50,
                    50,
                    50);

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

            btnSeleccionar =
                new Button();

            btnSeleccionar.Name =
                "btnSeleccionar";

            btnSeleccionar.Size =
                new Size(
                    170,
                    45);

            btnSeleccionar.Location =
                new Point(
                    890,
                    545);

            btnSeleccionar.BackColor =
                Color.FromArgb(
                    92,
                    126,
                    215);

            btnSeleccionar.ForeColor =
                Color.White;

            btnSeleccionar.FlatStyle =
                FlatStyle.Flat;

            btnSeleccionar.FlatAppearance.BorderSize =
                0;

            btnSeleccionar.Cursor =
                Cursors.Hand;

            btnSeleccionar.Click +=
                BtnSeleccionar_Click;

            Controls.Add(
                btnSeleccionar);

            AcceptButton =
                btnBuscar;

            CancelButton =
                btnCancelar;
        }

        private void AplicarIdioma()
        {
            Text =
                LanguageService.Translate(
                    "SeleccionarPresupuesto");

            lblTitulo.Text =
                LanguageService.Translate(
                    "SeleccionarPresupuesto");

            lblSubtitulo.Text =
                LanguageService.Translate(
                    "PresupuestosDisponiblesCliente");

            btnBuscar.Text =
                LanguageService.Translate(
                    "Buscar");

            btnActualizar.Text =
                LanguageService.Translate(
                    "Actualizar");

            btnCancelar.Text =
                LanguageService.Translate(
                    "Cancelar");

            btnSeleccionar.Text =
                LanguageService.Translate(
                    "SeleccionarPresupuesto");

            dgvPresupuestos
                .Columns[1]
                .HeaderText =
                    LanguageService.Translate(
                        "Numero");

            dgvPresupuestos
                .Columns[2]
                .HeaderText =
                    LanguageService.Translate(
                        "Fecha");

            dgvPresupuestos
                .Columns[3]
                .HeaderText =
                    LanguageService.Translate(
                        "Estado");

            dgvPresupuestos
                .Columns[4]
                .HeaderText =
                    LanguageService.Translate(
                        "Total");

            dgvPresupuestos
                .Columns[5]
                .HeaderText =
                    LanguageService.Translate(
                        "Vencimiento");

            if (ClienteId == Guid.Empty)
            {
                lblResultado.Text =
                    LanguageService.Translate(
                        "SeleccioneClientePrincipal");

                lblResultado.ForeColor =
                    Color.DimGray;
            }
            else
            {
                ActualizarTextoResultado(
                    presupuestosCargados.Count);
            }
        }

        private void LanguageService_IdiomaCambiado(
            object sender,
            EventArgs e)
        {
            AplicarIdioma();

            if (ClienteId != Guid.Empty)
            {
                MostrarPresupuestos(
                    presupuestosCargados);
            }
        }

        private void FrmSeleccionPresupuesto_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            LanguageService.IdiomaCambiado -=
                LanguageService_IdiomaCambiado;
        }

        public void CargarPresupuestos(
            Guid clienteId)
        {
            if (clienteId == Guid.Empty)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClienteNoEncontrado"),
                    LanguageService.Translate(
                        "ClienteInvalido"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ClienteId =
                clienteId;

            try
            {
                List<Presupuesto> presupuestos =
                    _presupuestoService
                        .ObtenerPorCliente(
                            clienteId);

                if (presupuestos == null ||
                    presupuestos.Count == 0)
                {
                    _presupuestoService
                        .CrearPresupuestosDemostracion(
                            clienteId);

                    presupuestos =
                        _presupuestoService
                            .ObtenerPorCliente(
                                clienteId);
                }

                presupuestosCargados =
                    presupuestos ??
                    new List<Presupuesto>();

                MostrarPresupuestos(
                    presupuestosCargados);
            }
            catch (Exception ex)
            {
                dgvPresupuestos.Rows.Clear();

                lblResultado.Text =
                    LanguageService.Translate(
                        "ErrorCargarPresupuestos");

                lblResultado.ForeColor =
                    Color.Firebrick;

                MessageBox.Show(
                    LanguageService.Translate(
                        "ErrorCargarPresupuestos") +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    LanguageService.Translate(
                        "ErrorBaseDatos"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MostrarPresupuestos(
            List<Presupuesto> presupuestos)
        {
            dgvPresupuestos.Rows.Clear();

            foreach (Presupuesto presupuesto
                in presupuestos)
            {
                string moneda =
                    presupuesto.Moneda ==
                    Moneda.DolaresEstadounidenses
                        ? "USD "
                        : "$";

                dgvPresupuestos.Rows.Add(
                    presupuesto.Id,
                    presupuesto.Numero,
                    presupuesto.FechaEmision
                        .ToShortDateString(),
                    ObtenerEstadoTraducido(
                        presupuesto.Estado),
                    moneda +
                    presupuesto
                        .CalcularTotal()
                        .ToString("N2"),
                    presupuesto.FechaVencimiento
                        .ToShortDateString());
            }

            dgvPresupuestos.ClearSelection();

            dgvPresupuestos.CurrentCell =
                null;

            ActualizarTextoResultado(
                presupuestos.Count);
        }

        private void ActualizarTextoResultado(
            int cantidad)
        {
            if (cantidad == 0)
            {
                lblResultado.Text =
                    LanguageService.Translate(
                        "NoPresupuestosEncontrados");

                lblResultado.ForeColor =
                    Color.Firebrick;

                return;
            }

            lblResultado.Text =
                LanguageService.Translate(
                    "PresupuestosEncontrados") +
                ": " +
                cantidad;

            lblResultado.ForeColor =
                Color.ForestGreen;
        }

        private void BuscarPresupuestos()
        {
            if (ClienteId == Guid.Empty)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClientePrimero"),
                    LanguageService.Translate(
                        "ClienteRequerido"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string filtro =
                txtBuscar.Text
                    .Trim()
                    .ToLowerInvariant();

            List<Presupuesto> resultados =
                presupuestosCargados;

            if (!string.IsNullOrWhiteSpace(
                filtro))
            {
                resultados =
                    presupuestosCargados
                        .Where(
                            presupuesto =>
                                CoincideConFiltro(
                                    presupuesto,
                                    filtro))
                        .ToList();
            }

            MostrarPresupuestos(
                resultados);
        }

        private static bool CoincideConFiltro(
            Presupuesto presupuesto,
            string filtro)
        {
            string numero =
                (presupuesto.Numero ??
                 string.Empty)
                    .ToLowerInvariant();

            string estadoOriginal =
                presupuesto.Estado
                    .ToString()
                    .ToLowerInvariant();

            string estadoTraducido =
                ObtenerEstadoTraducido(
                    presupuesto.Estado)
                    .ToLowerInvariant();

            return
                numero.Contains(filtro) ||
                estadoOriginal.Contains(filtro) ||
                estadoTraducido.Contains(filtro);
        }

        private void BtnBuscar_Click(
            object sender,
            EventArgs e)
        {
            BuscarPresupuestos();
        }

        private void BtnActualizar_Click(
            object sender,
            EventArgs e)
        {
            txtBuscar.Clear();

            if (ClienteId == Guid.Empty)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClientePrimero"),
                    LanguageService.Translate(
                        "ClienteRequerido"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            CargarPresupuestos(
                ClienteId);

            txtBuscar.Focus();
        }

        private void TxtBuscar_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress =
                true;

            e.Handled =
                true;

            BuscarPresupuestos();
        }

        private void DgvPresupuestos_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            dgvPresupuestos.Rows[e.RowIndex]
                .Selected = true;

            SeleccionarPresupuestoActual();
        }

        private void BtnSeleccionar_Click(
            object sender,
            EventArgs e)
        {
            SeleccionarPresupuestoActual();
        }

        private void SeleccionarPresupuestoActual()
        {
            if (dgvPresupuestos.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "PresupuestoRequerido"),
                    LanguageService.Translate(
                        "SeleccionarPresupuesto"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            object valorId =
                dgvPresupuestos
                    .SelectedRows[0]
                    .Cells[0]
                    .Value;

            Guid presupuestoId;

            if (valorId == null ||
                !Guid.TryParse(
                    valorId.ToString(),
                    out presupuestoId))
            {
                MostrarPresupuestoNoEncontrado();

                return;
            }

            PresupuestoSeleccionado =
                presupuestosCargados
                    .FirstOrDefault(
                        presupuesto =>
                            presupuesto.Id ==
                            presupuestoId);

            if (PresupuestoSeleccionado == null)
            {
                MostrarPresupuestoNoEncontrado();

                return;
            }

            if (!PresupuestoSeleccionado
                .PuedeConfirmarse())
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "DetallePresupuestoNoConfirmable"),
                    LanguageService.Translate(
                        "PresupuestoNoConfirmable"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult =
                DialogResult.OK;

            Close();
        }

        private static string ObtenerEstadoTraducido(
            EstadoPresupuesto estado)
        {
            string clave =
                "EstadoPresupuesto_" +
                estado;

            string traduccion =
                LanguageService.Translate(
                    clave);

            if (string.Equals(
                traduccion,
                clave,
                StringComparison.OrdinalIgnoreCase))
            {
                return estado.ToString();
            }

            return traduccion;
        }

        private static void MostrarPresupuestoNoEncontrado()
        {
            MessageBox.Show(
                LanguageService.Translate(
                    "PresupuestoNoEncontrado"),
                LanguageService.Translate(
                    "SeleccionarPresupuesto"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void BtnCancelar_Click(
            object sender,
            EventArgs e)
        {
            DialogResult =
                DialogResult.Cancel;

            Close();
        }
    }
}