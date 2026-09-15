using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Edelstahl.BLL.Exceptions.Base;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.WinApp.Forms.Commercial
{
    public partial class FrmClientes : Form
    {
        private readonly ClienteService _clienteService;

        public Cliente ClienteRegistrado
        {
            get;
            private set;
        }

        public FrmClientes()
        {
            InitializeComponent();

            _clienteService =
                new ClienteService();

            ConfigurarGrilla();
            ConfigurarEventos();
            RefrescarGrilla();

            txtCUIT.Focus();
        }

        private void ConfigurarEventos()
        {
            btnGuardar.Click -=
                btnGuardar_Click;

            btnGuardar.Click +=
                btnGuardar_Click;

            btnRefrescar.Click -=
                btnRefrescar_Click;

            btnRefrescar.Click +=
                btnRefrescar_Click;

            btnLimpiar.Click -=
                btnLimpiar_Click;

            btnLimpiar.Click +=
                btnLimpiar_Click;

            btnBuscarCliente.Click -=
                btnBuscarCliente_Click;

            btnBuscarCliente.Click +=
                btnBuscarCliente_Click;

            btnMostrarTodos.Click -=
                btnMostrarTodos_Click;

            btnMostrarTodos.Click +=
                btnMostrarTodos_Click;

            txtBuscarCliente.KeyDown -=
                txtBuscarCliente_KeyDown;

            txtBuscarCliente.KeyDown +=
                txtBuscarCliente_KeyDown;
        }

        private void ConfigurarGrilla()
        {
            dgvClientes.AutoGenerateColumns =
                false;

            dgvClientes.ReadOnly =
                true;

            dgvClientes.MultiSelect =
                false;

            dgvClientes.AllowUserToAddRows =
                false;

            dgvClientes.AllowUserToDeleteRows =
                false;

            dgvClientes.AllowUserToResizeRows =
                false;

            dgvClientes.RowHeadersVisible =
                false;

            dgvClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                ValidarFormulario();

                Cliente nuevoCliente =
                    CrearClienteDesdeFormulario();

                ClienteRegistrado =
                    _clienteService.Registrar(
                        nuevoCliente);

                RefrescarGrilla();

                MessageBox.Show(
                    "El cliente '" +
                    ClienteRegistrado.RazonSocial +
                    "' fue registrado correctamente en SQL Server.",
                    "Cliente registrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LimpiarCampos();

                txtCUIT.Focus();
            }
            catch (BusinessRuleException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Regla de negocio: " +
                    ex.Code,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Datos incorrectos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No fue posible registrar el cliente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el cliente." +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBuscarCliente_Click(
            object sender,
            EventArgs e)
        {
            BuscarClientes();
        }

        private void btnMostrarTodos_Click(
            object sender,
            EventArgs e)
        {
            txtBuscarCliente.Clear();

            RefrescarGrilla();

            txtBuscarCliente.Focus();
        }

        private void txtBuscarCliente_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;

            BuscarClientes();
        }

        private void BuscarClientes()
        {
            try
            {
                string filtro =
                    txtBuscarCliente.Text.Trim();

                if (string.IsNullOrWhiteSpace(filtro))
                {
                    RefrescarGrilla();
                    return;
                }

                string filtroCUIT =
                    NormalizarCUIT(filtro);

                string filtroTexto =
                    filtro.ToLowerInvariant();

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
                                (cliente.RazonSocial ??
                                 string.Empty)
                                .ToLowerInvariant();

                            bool coincideCUIT =
                                !string.IsNullOrWhiteSpace(
                                    filtroCUIT) &&
                                cuitCliente.Contains(
                                    filtroCUIT);

                            bool coincideRazonSocial =
                                razonSocial.Contains(
                                    filtroTexto);

                            return coincideCUIT ||
                                   coincideRazonSocial;
                        })
                        .OrderBy(
                            cliente =>
                                cliente.RazonSocial)
                        .ToList();

                MostrarClientesEnGrilla(
                    resultados);

                if (resultados.Count == 0)
                {
                    MessageBox.Show(
                        "No se encontraron clientes que coincidan " +
                        "con el criterio ingresado.",
                        "Búsqueda sin resultados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    txtBuscarCliente.Focus();
                    txtBuscarCliente.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo realizar la búsqueda." +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    "Error de búsqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ValidarFormulario()
        {
            string cuit =
                NormalizarCUIT(
                    txtCUIT.Text);

            if (string.IsNullOrWhiteSpace(cuit))
            {
                MostrarAdvertencia(
                    "Debe ingresar el CUIT del cliente.",
                    txtCUIT);
            }

            if (cuit.Length != 11)
            {
                MostrarAdvertencia(
                    "El CUIT debe contener 11 números.",
                    txtCUIT);
            }

            long cuitNumerico;

            if (!long.TryParse(
                cuit,
                out cuitNumerico))
            {
                MostrarAdvertencia(
                    "El CUIT solamente puede contener números.",
                    txtCUIT);
            }

            if (string.IsNullOrWhiteSpace(
                txtRazonSocial.Text))
            {
                MostrarAdvertencia(
                    "Debe ingresar la razón social del cliente.",
                    txtRazonSocial);
            }

            if (!string.IsNullOrWhiteSpace(
                    txtEmail.Text) &&
                !EmailPareceValido(
                    txtEmail.Text))
            {
                MostrarAdvertencia(
                    "El correo electrónico ingresado no parece válido.",
                    txtEmail);
            }

            if (nudLimiteCredito.Value < 0m)
            {
                MostrarAdvertencia(
                    "El límite de crédito no puede ser negativo.",
                    nudLimiteCredito);
            }
        }

        private static void MostrarAdvertencia(
            string mensaje,
            Control control)
        {
            MessageBox.Show(
                mensaje,
                "Datos requeridos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            control.Focus();

            throw new ArgumentException(
                mensaje);
        }

        private Cliente CrearClienteDesdeFormulario()
        {
            Cliente cliente =
                new Cliente();

            cliente.Id =
                Guid.NewGuid();

            cliente.CUIT =
                NormalizarCUIT(
                    txtCUIT.Text);

            cliente.RazonSocial =
                txtRazonSocial.Text.Trim();

            cliente.Email =
                txtEmail.Text.Trim();

            cliente.Telefono =
                txtTelefono.Text.Trim();

            cliente.Localidad =
                txtLocalidad.Text.Trim();

            cliente.LimiteCredito =
                nudLimiteCredito.Value;

            cliente.DeudaActual =
                0m;

            cliente.DireccionFacturacion =
                string.Empty;

            cliente.DireccionEntrega =
                string.Empty;

            cliente.Provincia =
                string.Empty;

            cliente.CodigoPostal =
                string.Empty;

            cliente.Activo =
                true;

            cliente.FechaAlta =
                DateTime.Now;

            return cliente;
        }

        private void btnRefrescar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                txtBuscarCliente.Clear();

                RefrescarGrilla();

                MessageBox.Show(
                    "El listado de clientes fue actualizado " +
                    "desde SQL Server.",
                    "Listado actualizado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el listado de clientes." +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarCampos();

            txtCUIT.Focus();
        }

        private void RefrescarGrilla()
        {
            List<Cliente> clientes =
                _clienteService.ObtenerTodos();

            MostrarClientesEnGrilla(
                clientes);
        }

        private void MostrarClientesEnGrilla(
            List<Cliente> clientes)
        {
            dgvClientes.DataSource =
                null;

            dgvClientes.DataSource =
                clientes;

            dgvClientes.ClearSelection();

            dgvClientes.CurrentCell =
                null;
        }

        private void LimpiarCampos()
        {
            txtCUIT.Clear();
            txtRazonSocial.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtLocalidad.Clear();

            nudLimiteCredito.Value =
                0m;

            ClienteRegistrado =
                null;
        }

        private static string NormalizarCUIT(
            string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
            {
                return string.Empty;
            }

            return cuit
                .Trim()
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty);
        }

        private static bool EmailPareceValido(
            string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            string emailLimpio =
                email.Trim();

            int posicionArroba =
                emailLimpio.IndexOf('@');

            int posicionPunto =
                emailLimpio.LastIndexOf('.');

            return posicionArroba > 0 &&
                   posicionPunto >
                   posicionArroba + 1 &&
                   posicionPunto <
                   emailLimpio.Length - 1;
        }

        /*
         * Este método se mantiene temporalmente porque
         * puede continuar conectado desde el Designer.
         * No contiene lógica para evitar registrar dos veces.
         */
        private void btnGuardar_Click_1(
     object sender,
     EventArgs e)
        {
        }

        private void lblListado_Click(
            object sender,
            EventArgs e)
        {
            // Evento conservado porque está conectado
            // desde FrmClientes.Designer.cs.
        }
    }
}