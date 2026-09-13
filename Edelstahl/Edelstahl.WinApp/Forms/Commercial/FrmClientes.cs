using System;
using System.Windows.Forms;
using Edelstahl.BLL.Exceptions.Base;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Comercial;
using Edelstahl.Services.Localization;



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

                DialogResult =
                    DialogResult.OK;

                Close();
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

                return;
            }

            if (cuit.Length != 11)
            {
                MostrarAdvertencia(
                    "El CUIT debe contener 11 números.",
                    txtCUIT);

                return;
            }

            long cuitNumerico;

            if (!long.TryParse(
                cuit,
                out cuitNumerico))
            {
                MostrarAdvertencia(
                    "El CUIT solamente puede contener números.",
                    txtCUIT);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtRazonSocial.Text))
            {
                MostrarAdvertencia(
                    "Debe ingresar la razón social del cliente.",
                    txtRazonSocial);

                return;
            }

            if (!string.IsNullOrWhiteSpace(
                txtEmail.Text) &&
                !EmailPareceValido(
                    txtEmail.Text))
            {
                MostrarAdvertencia(
                    "El correo electrónico ingresado no parece válido.",
                    txtEmail);

                return;
            }

            if (nudLimiteCredito.Value < 0m)
            {
                MessageBox.Show(
                    "El límite de crédito no puede ser negativo.",
                    "Límite de crédito incorrecto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                nudLimiteCredito.Focus();

                throw new ArgumentException(
                    "El límite de crédito no puede ser negativo.");
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
                RefrescarGrilla();

                MessageBox.Show(
                    "El listado de clientes fue actualizado desde SQL Server.",
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
            dgvClientes.DataSource =
                null;

            dgvClientes.DataSource =
                _clienteService.ObtenerTodos();

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

        private void btnGuardar_Click_1(
            object sender,
            EventArgs e)
        {
        }
    }
}