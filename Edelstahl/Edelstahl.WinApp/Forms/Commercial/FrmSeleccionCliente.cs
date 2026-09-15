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
    public partial class FrmSeleccionCliente : Form
    {
        private readonly ClienteService _clienteService;

        private Label lblTitulo;
        private Label lblBuscar;

        private DataGridView dgvClientes;
        private TextBox txtBuscar;

        private Button btnBuscar;
        private Button btnMostrarTodos;
        private Button btnNuevoCliente;
        private Button btnSeleccionar;

        private List<Cliente> _clientesMostrados;

        public Cliente ClienteSeleccionado
        {
            get;
            private set;
        }

        public FrmSeleccionCliente()
        {
            InitializeComponent();

            _clienteService =
                new ClienteService();

            _clientesMostrados =
                new List<Cliente>();

            ConfigurarFormulario();
            ConfigurarEventosIdioma();
            AplicarIdioma();
        }

        private void ConfigurarFormulario()
        {
            Width =
                1000;

            Height =
                640;

            MinimumSize =
                new Size(
                    900,
                    600);

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.White;

            CrearControles();
            CargarClientes();

            txtBuscar.Focus();
        }

        private void ConfigurarEventosIdioma()
        {
            LanguageService.IdiomaCambiado +=
                LanguageService_IdiomaCambiado;

            FormClosed +=
                FrmSeleccionCliente_FormClosed;
        }

        private void CrearControles()
        {
            lblTitulo =
                new Label();

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold);

            lblTitulo.Location =
                new Point(
                    20,
                    20);

            lblTitulo.AutoSize =
                true;

            Controls.Add(
                lblTitulo);

            lblBuscar =
                new Label();

            lblBuscar.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Regular);

            lblBuscar.Location =
                new Point(
                    20,
                    62);

            lblBuscar.AutoSize =
                true;

            Controls.Add(
                lblBuscar);

            txtBuscar =
                new TextBox();

            txtBuscar.Name =
                "txtBuscar";

            txtBuscar.Location =
                new Point(
                    20,
                    85);

            txtBuscar.Size =
                new Size(
                    470,
                    25);

            txtBuscar.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

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
                    510,
                    82);

            btnBuscar.Size =
                new Size(
                    115,
                    32);

            btnBuscar.BackColor =
                Color.FromArgb(
                    49,
                    102,
                    230);

            btnBuscar.ForeColor =
                Color.White;

            btnBuscar.FlatStyle =
                FlatStyle.Flat;

            btnBuscar.FlatAppearance.BorderSize =
                0;

            btnBuscar.Click +=
                BtnBuscar_Click;

            Controls.Add(
                btnBuscar);

            btnMostrarTodos =
                new Button();

            btnMostrarTodos.Name =
                "btnMostrarTodos";

            btnMostrarTodos.Location =
                new Point(
                    640,
                    82);

            btnMostrarTodos.Size =
                new Size(
                    125,
                    32);

            btnMostrarTodos.BackColor =
                Color.FromArgb(
                    225,
                    231,
                    240);

            btnMostrarTodos.ForeColor =
                Color.FromArgb(
                    32,
                    49,
                    79);

            btnMostrarTodos.FlatStyle =
                FlatStyle.Flat;

            btnMostrarTodos.FlatAppearance.BorderColor =
                Color.FromArgb(
                    150,
                    165,
                    190);

            btnMostrarTodos.Click +=
                BtnMostrarTodos_Click;

            Controls.Add(
                btnMostrarTodos);

            btnNuevoCliente =
                new Button();

            btnNuevoCliente.Name =
                "btnNuevoCliente";

            btnNuevoCliente.Location =
                new Point(
                    790,
                    82);

            btnNuevoCliente.Size =
                new Size(
                    150,
                    32);

            btnNuevoCliente.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnNuevoCliente.BackColor =
                Color.FromArgb(
                    49,
                    102,
                    230);

            btnNuevoCliente.ForeColor =
                Color.White;

            btnNuevoCliente.FlatStyle =
                FlatStyle.Flat;

            btnNuevoCliente.FlatAppearance.BorderSize =
                0;

            btnNuevoCliente.Click +=
                BtnNuevoCliente_Click;

            Controls.Add(
                btnNuevoCliente);

            dgvClientes =
                new DataGridView();

            dgvClientes.Name =
                "dgvClientes";

            dgvClientes.Location =
                new Point(
                    20,
                    135);

            dgvClientes.Size =
                new Size(
                    920,
                    370);

            dgvClientes.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvClientes.RowHeadersVisible =
                false;

            dgvClientes.AllowUserToAddRows =
                false;

            dgvClientes.AllowUserToDeleteRows =
                false;

            dgvClientes.AllowUserToResizeRows =
                false;

            dgvClientes.ReadOnly =
                true;

            dgvClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.MultiSelect =
                false;

            dgvClientes.BackgroundColor =
                Color.White;

            dgvClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvClientes.ColumnCount =
                6;

            dgvClientes.Columns[0].Name =
                "Id";

            dgvClientes.Columns[0].Visible =
                false;

            dgvClientes.CellDoubleClick +=
                DgvClientes_CellDoubleClick;

            Controls.Add(
                dgvClientes);

            btnSeleccionar =
                new Button();

            btnSeleccionar.Name =
                "btnSeleccionar";

            btnSeleccionar.Size =
                new Size(
                    220,
                    45);

            btnSeleccionar.Location =
                new Point(
                    720,
                    525);

            btnSeleccionar.Anchor =
                AnchorStyles.Bottom |
                AnchorStyles.Right;

            btnSeleccionar.BackColor =
                Color.FromArgb(
                    49,
                    102,
                    230);

            btnSeleccionar.ForeColor =
                Color.White;

            btnSeleccionar.FlatStyle =
                FlatStyle.Flat;

            btnSeleccionar.FlatAppearance.BorderSize =
                0;

            btnSeleccionar.Click +=
                BtnSeleccionar_Click;

            Controls.Add(
                btnSeleccionar);

            AcceptButton =
                btnBuscar;
        }

        private void AplicarIdioma()
        {
            Text =
                LanguageService.Translate(
                    "SeleccionarCliente");

            lblTitulo.Text =
                LanguageService.Translate(
                    "SeleccionarCliente");

            lblBuscar.Text =
                LanguageService.Translate(
                    "BuscarClientes") +
                ":";

            btnBuscar.Text =
                LanguageService.Translate(
                    "Buscar");

            btnMostrarTodos.Text =
                LanguageService.Translate(
                    "MostrarTodos");

            btnNuevoCliente.Text =
                LanguageService.Translate(
                    "NuevoCliente");

            btnSeleccionar.Text =
                LanguageService.Translate(
                    "SeleccionarCliente");

            dgvClientes.Columns[1].HeaderText =
                LanguageService.Translate(
                    "CUIT");

            dgvClientes.Columns[2].HeaderText =
                LanguageService.Translate(
                    "RazonSocial");

            dgvClientes.Columns[3].HeaderText =
                LanguageService.Translate(
                    "Correo");

            dgvClientes.Columns[4].HeaderText =
                LanguageService.Translate(
                    "Localidad");

            dgvClientes.Columns[5].HeaderText =
                LanguageService.Translate(
                    "Telefono");
        }

        private void LanguageService_IdiomaCambiado(
            object sender,
            EventArgs e)
        {
            AplicarIdioma();
        }

        private void FrmSeleccionCliente_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            LanguageService.IdiomaCambiado -=
                LanguageService_IdiomaCambiado;
        }

        private void CargarClientes()
        {
            List<Cliente> clientes =
                _clienteService.ObtenerTodos();

            MostrarClientes(
                clientes);
        }

        private void MostrarClientes(
            List<Cliente> clientes)
        {
            _clientesMostrados =
                clientes ??
                new List<Cliente>();

            dgvClientes.Rows.Clear();

            foreach (Cliente cliente
                in _clientesMostrados)
            {
                dgvClientes.Rows.Add(
                    cliente.Id,
                    cliente.CUIT,
                    cliente.RazonSocial,
                    cliente.Email,
                    cliente.Localidad,
                    cliente.Telefono);
            }

            dgvClientes.ClearSelection();

            dgvClientes.CurrentCell =
                null;
        }

        private void BtnBuscar_Click(
            object sender,
            EventArgs e)
        {
            BuscarClientes();
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

            BuscarClientes();
        }

        private void BuscarClientes()
        {
            string filtro =
                txtBuscar.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                filtro))
            {
                CargarClientes();

                txtBuscar.Focus();

                return;
            }

            try
            {
                string filtroCUIT =
                    NormalizarCUIT(
                        filtro);

                string filtroTexto =
                    NormalizarTexto(
                        filtro);

                List<Cliente> clientes =
                    _clienteService.ObtenerTodos();

                List<Cliente> resultados =
                    clientes
                        .Where(cliente =>
                        {
                            string cuitCliente =
                                NormalizarCUIT(
                                    cliente.CUIT);

                            string razonSocial =
                                NormalizarTexto(
                                    cliente.RazonSocial);

                            string email =
                                NormalizarTexto(
                                    cliente.Email);

                            string localidad =
                                NormalizarTexto(
                                    cliente.Localidad);

                            string telefono =
                                NormalizarTexto(
                                    cliente.Telefono);

                            bool coincideCUIT =
                                !string.IsNullOrWhiteSpace(
                                    filtroCUIT) &&
                                cuitCliente.Contains(
                                    filtroCUIT);

                            bool coincideRazonSocial =
                                razonSocial.Contains(
                                    filtroTexto);

                            bool coincideEmail =
                                email.Contains(
                                    filtroTexto);

                            bool coincideLocalidad =
                                localidad.Contains(
                                    filtroTexto);

                            bool coincideTelefono =
                                telefono.Contains(
                                    filtroTexto);

                            return
                                coincideCUIT ||
                                coincideRazonSocial ||
                                coincideEmail ||
                                coincideLocalidad ||
                                coincideTelefono;
                        })
                        .OrderBy(
                            cliente =>
                                cliente.RazonSocial)
                        .ToList();

                MostrarClientes(
                    resultados);

                if (resultados.Count == 0)
                {
                    MessageBox.Show(
                        LanguageService.Translate(
                            "BusquedaSinResultados"),
                        LanguageService.Translate(
                            "BuscarClientes"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    txtBuscar.Focus();

                    txtBuscar.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ErrorBaseDatos") +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    LanguageService.Translate(
                        "BuscarClientes"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnMostrarTodos_Click(
            object sender,
            EventArgs e)
        {
            txtBuscar.Clear();

            CargarClientes();

            txtBuscar.Focus();
        }

        private void BtnSeleccionar_Click(
            object sender,
            EventArgs e)
        {
            SeleccionarClienteActual();
        }

        private void DgvClientes_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            SeleccionarClienteActual();
        }

        private void SeleccionarClienteActual()
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClienteRequerido"),
                    LanguageService.Translate(
                        "SeleccionarCliente"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            object valorId =
                dgvClientes
                    .SelectedRows[0]
                    .Cells[0]
                    .Value;

            Guid clienteId;

            if (valorId == null ||
                !Guid.TryParse(
                    valorId.ToString(),
                    out clienteId))
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClienteNoEncontrado"),
                    LanguageService.Translate(
                        "SeleccionarCliente"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ClienteSeleccionado =
                _clientesMostrados
                    .FirstOrDefault(
                        cliente =>
                            cliente.Id == clienteId);

            if (ClienteSeleccionado == null)
            {
                MessageBox.Show(
                    LanguageService.Translate(
                        "ClienteNoEncontrado"),
                    LanguageService.Translate(
                        "SeleccionarCliente"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult =
                DialogResult.OK;

            Close();
        }

        private void BtnNuevoCliente_Click(
            object sender,
            EventArgs e)
        {
            using (FrmClientes formulario =
                new FrmClientes())
            {
                formulario.ShowDialog(
                    this);
            }

            txtBuscar.Clear();

            CargarClientes();

            txtBuscar.Focus();
        }

        private static string NormalizarCUIT(
            string cuit)
        {
            return string.IsNullOrWhiteSpace(
                cuit)
                ? string.Empty
                : cuit
                    .Trim()
                    .Replace(
                        "-",
                        string.Empty)
                    .Replace(
                        " ",
                        string.Empty)
                    .Replace(
                        ".",
                        string.Empty);
        }

        private static string NormalizarTexto(
            string valor)
        {
            return string.IsNullOrWhiteSpace(
                valor)
                ? string.Empty
                : valor
                    .Trim()
                    .ToLowerInvariant();
        }
    }
}