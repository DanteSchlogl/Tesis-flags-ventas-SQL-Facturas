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
    public partial class FrmSeleccionCliente : Form
    {
        private readonly ClienteService _clienteService;

        private DataGridView dgvClientes;

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

            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            Text = "Seleccionar Cliente";

            Width = 1000;
            Height = 600;

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor =
                Color.White;

            CrearControles();

            CargarClientes();
        }

        private void CrearControles()
        {
            Label lblTitulo = new Label();

            lblTitulo.Text =
                "Seleccionar Cliente";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Bold);

            lblTitulo.Location =
                new Point(20, 20);

            lblTitulo.AutoSize = true;

            Controls.Add(lblTitulo);

            TextBox txtBuscar =
                new TextBox();

            txtBuscar.Location =
                new Point(20, 70);

            txtBuscar.Width = 500;

            Controls.Add(txtBuscar);

            Button btnBuscar =
                new Button();

            btnBuscar.Text =
                "Buscar";

            btnBuscar.Location =
                new Point(540, 68);

            btnBuscar.Size =
                new Size(120, 32);

            Controls.Add(btnBuscar);

            Button btnNuevoCliente =
                new Button();

            btnNuevoCliente.Text =
                "Nuevo Cliente";

            btnNuevoCliente.Location =
                new Point(780, 68);

            btnNuevoCliente.Size =
                new Size(150, 32);

            btnNuevoCliente.BackColor =
                Color.FromArgb(49, 102, 230);

            btnNuevoCliente.ForeColor =
                Color.White;

            btnNuevoCliente.FlatStyle =
                FlatStyle.Flat;

            btnNuevoCliente.Click +=
                BtnNuevoCliente_Click;

            Controls.Add(btnNuevoCliente);

            dgvClientes =
                new DataGridView();

            dgvClientes.Location =
                new Point(20, 120);

            dgvClientes.Size =
                new Size(920, 360);

            dgvClientes.RowHeadersVisible =
                false;

            dgvClientes.AllowUserToAddRows =
                false;

            dgvClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.MultiSelect =
                false;

            dgvClientes.BackgroundColor =
                Color.White;

            dgvClientes.ColumnCount = 4;

            dgvClientes.Columns[0].Name =
                "Nombre";

            dgvClientes.Columns[1].Name =
                "Email";

            dgvClientes.Columns[2].Name =
                "Ciudad";

            dgvClientes.Columns[3].Name =
                "Teléfono";

            Controls.Add(dgvClientes);

            Button btnSeleccionar =
                new Button();

            btnSeleccionar.Text =
                "Seleccionar Cliente";

            btnSeleccionar.Size =
                new Size(220, 45);

            btnSeleccionar.Location =
                new Point(720, 500);

            btnSeleccionar.BackColor =
                Color.FromArgb(49, 102, 230);

            btnSeleccionar.ForeColor =
                Color.White;

            btnSeleccionar.FlatStyle =
                FlatStyle.Flat;

            btnSeleccionar.Click +=
                BtnSeleccionar_Click;

            Controls.Add(btnSeleccionar);
        }

        private void CargarClientes()
        {
            dgvClientes.Rows.Clear();

            List<Cliente> clientes =
                _clienteService.ObtenerTodos();

            foreach (Cliente cliente in clientes)
            {
                dgvClientes.Rows.Add(
                    cliente.RazonSocial,
                    cliente.Email,
                    cliente.Localidad,
                    cliente.Telefono);
            }
        }

        private void BtnSeleccionar_Click(
            object sender,
            EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un cliente.");

                return;
            }

            string razonSocial =
                dgvClientes.SelectedRows[0]
                    .Cells[0]
                    .Value
                    .ToString();

            ClienteSeleccionado =
                _clienteService
                    .ObtenerTodos()
                    .FirstOrDefault(
                        x => x.RazonSocial ==
                             razonSocial);

            DialogResult =
                DialogResult.OK;

            Close();
        }

        private void BtnNuevoCliente_Click(
            object sender,
            EventArgs e)
        {
            FrmClientes frm =
                new FrmClientes();

            frm.ShowDialog();

            CargarClientes();
        }
    }
}