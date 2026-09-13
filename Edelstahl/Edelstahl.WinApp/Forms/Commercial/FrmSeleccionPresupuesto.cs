using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.WinApp.Forms.Commercial
{
    public partial class FrmSeleccionPresupuesto : Form
    {
        private readonly PresupuestoService _presupuestoService;

        private DataGridView dgvPresupuestos;
        private TextBox txtBuscar;
        private Label lblResultado;

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
        }

        private void ConfigurarFormulario()
        {
            Text =
                "Seleccionar Presupuesto";

            Width =
                1100;

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

            CrearControles();
        }

        private void CrearControles()
        {
            Controls.Clear();

            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                "Seleccionar Presupuesto";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold);

            lblTitulo.ForeColor =
                Color.FromArgb(54, 69, 98);

            lblTitulo.AutoSize =
                true;

            lblTitulo.Location =
                new Point(25, 20);

            Controls.Add(
                lblTitulo);

            Label lblSubtitulo =
                new Label();

            lblSubtitulo.Text =
                "Presupuestos disponibles para el cliente seleccionado";

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            lblSubtitulo.ForeColor =
                Color.FromArgb(70, 70, 70);

            lblSubtitulo.AutoSize =
                true;

            lblSubtitulo.Location =
                new Point(25, 75);

            Controls.Add(
                lblSubtitulo);

            txtBuscar =
                new TextBox();

            txtBuscar.Location =
                new Point(25, 115);

            txtBuscar.Size =
                new Size(500, 30);

            txtBuscar.Font =
                new Font(
                    "Segoe UI",
                    10);

            txtBuscar.KeyDown +=
                TxtBuscar_KeyDown;

            Controls.Add(
                txtBuscar);

            Button btnBuscar =
                new Button();

            btnBuscar.Text =
                "Buscar";

            btnBuscar.Location =
                new Point(545, 112);

            btnBuscar.Size =
                new Size(130, 35);

            btnBuscar.BackColor =
                Color.FromArgb(173, 196, 255);

            btnBuscar.ForeColor =
                Color.FromArgb(40, 40, 40);

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

            Button btnActualizar =
                new Button();

            btnActualizar.Text =
                "Actualizar";

            btnActualizar.Location =
                new Point(690, 112);

            btnActualizar.Size =
                new Size(130, 35);

            btnActualizar.BackColor =
                Color.FromArgb(211, 207, 239);

            btnActualizar.ForeColor =
                Color.FromArgb(40, 40, 40);

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

            dgvPresupuestos.Location =
                new Point(25, 175);

            dgvPresupuestos.Size =
                new Size(1025, 330);

            dgvPresupuestos.RowHeadersVisible =
                false;

            dgvPresupuestos.AllowUserToAddRows =
                false;

            dgvPresupuestos.AllowUserToDeleteRows =
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
                Color.FromArgb(246, 248, 252);

            dgvPresupuestos.BorderStyle =
                BorderStyle.None;

            dgvPresupuestos.EnableHeadersVisualStyles =
                false;

            dgvPresupuestos.ColumnHeadersHeight =
                42;

            dgvPresupuestos.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(173, 196, 255);

            dgvPresupuestos.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(40, 40, 40);

            dgvPresupuestos.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);

            dgvPresupuestos.DefaultCellStyle.BackColor =
                Color.FromArgb(246, 248, 252);

            dgvPresupuestos.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(205, 219, 250);

            dgvPresupuestos.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(30, 30, 30);

            dgvPresupuestos.ColumnCount =
                5;

            dgvPresupuestos.Columns[0].Name =
                "Número";

            dgvPresupuestos.Columns[1].Name =
                "Fecha";

            dgvPresupuestos.Columns[2].Name =
                "Estado";

            dgvPresupuestos.Columns[3].Name =
                "Total";

            dgvPresupuestos.Columns[4].Name =
                "Vencimiento";

            dgvPresupuestos.CellDoubleClick +=
                DgvPresupuestos_CellDoubleClick;

            Controls.Add(
                dgvPresupuestos);

            lblResultado =
                new Label();

            lblResultado.Text =
                "Seleccione un cliente desde el panel principal.";

            lblResultado.ForeColor =
                Color.DimGray;

            lblResultado.Font =
                new Font(
                    "Segoe UI",
                    9);

            lblResultado.AutoSize =
                true;

            lblResultado.Location =
                new Point(25, 525);

            Controls.Add(
                lblResultado);

            Button btnCancelar =
                new Button();

            btnCancelar.Text =
                "Cancelar";

            btnCancelar.Size =
                new Size(150, 45);

            btnCancelar.Location =
                new Point(730, 545);

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

            Button btnSeleccionar =
                new Button();

            btnSeleccionar.Text =
                "Seleccionar Presupuesto";

            btnSeleccionar.Size =
                new Size(170, 45);

            btnSeleccionar.Location =
                new Point(890, 545);

            btnSeleccionar.BackColor =
                Color.FromArgb(92, 126, 215);

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
        }

        public void CargarPresupuestos(
            Guid clienteId)
        {
            if (clienteId == Guid.Empty)
            {
                MessageBox.Show(
                    "El cliente seleccionado no tiene un identificador válido.",
                    "Cliente inválido",
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
                    presupuestos
                    ?? new List<Presupuesto>();

                MostrarPresupuestos(
                    presupuestosCargados);
            }
            catch (Exception ex)
            {
                dgvPresupuestos.Rows.Clear();

                lblResultado.Text =
                    "No fue posible cargar los presupuestos.";

                lblResultado.ForeColor =
                    Color.Firebrick;

                MessageBox.Show(
                    ex.Message,
                    "Error al cargar presupuestos",
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
                    presupuesto.Numero,
                    presupuesto.FechaEmision
                        .ToShortDateString(),
                    presupuesto.Estado
                        .ToString(),
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

            lblResultado.Text =
                presupuestos.Count == 0
                    ? "No se encontraron presupuestos."
                    : "Presupuestos encontrados: " +
                      presupuestos.Count;

            lblResultado.ForeColor =
                presupuestos.Count == 0
                    ? Color.Firebrick
                    : Color.ForestGreen;
        }

        private void BuscarPresupuestos()
        {
            if (ClienteId == Guid.Empty)
            {
                MessageBox.Show(
                    "Debe seleccionar un cliente primero.",
                    "Cliente requerido",
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
                            (presupuesto.Numero ??
                             string.Empty)
                            .ToLowerInvariant()
                            .Contains(filtro)
                            ||
                            presupuesto.Estado
                            .ToString()
                            .ToLowerInvariant()
                            .Contains(filtro))
                    .ToList();
            }

            MostrarPresupuestos(
                resultados);
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

            CargarPresupuestos(
                ClienteId);
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
                    "Debe seleccionar un presupuesto.",
                    "Presupuesto requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            object valorNumero =
                dgvPresupuestos
                    .SelectedRows[0]
                    .Cells[0]
                    .Value;

            string numero =
                valorNumero == null
                    ? string.Empty
                    : valorNumero.ToString();

            PresupuestoSeleccionado =
                presupuestosCargados
                    .FirstOrDefault(
                        presupuesto =>
                            string.Equals(
                                presupuesto.Numero,
                                numero,
                                StringComparison
                                    .OrdinalIgnoreCase));

            if (PresupuestoSeleccionado == null)
            {
                MessageBox.Show(
                    "No fue posible recuperar el presupuesto seleccionado.",
                    "Presupuesto no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!PresupuestoSeleccionado
                .PuedeConfirmarse())
            {
                MessageBox.Show(
                    "El presupuesto seleccionado no puede confirmarse. " +
                    "Debe estar aceptado, vigente y contener detalles.",
                    "Presupuesto no confirmable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

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
    }
}