using System;
using System.Drawing;
using System.Windows.Forms;
using Edelstahl.BLL.Services;

namespace Edelstahl.WinApp
{
    public sealed class FrmCrearAdministradorInicial : Form
    {
        private readonly UsuarioService _usuarioService;
        private TextBox txtUsuario;
        private TextBox txtNombreCompleto;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtRepetirPassword;
        private Button btnCrear;

        public FrmCrearAdministradorInicial()
        {
            _usuarioService = new UsuarioService();
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Text = "Configuración inicial";
            ClientSize = new Size(520, 470);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(235, 240, 245);

            Label titulo = new Label
            {
                Text = "Crear administrador inicial",
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(54, 69, 98),
                Location = new Point(45, 30)
            };
            Controls.Add(titulo);

            Label ayuda = new Label
            {
                Text = "Este usuario tendrá acceso completo al sistema.",
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(48, 72)
            };
            Controls.Add(ayuda);

            txtUsuario = CrearCampo("Usuario", 110, false);
            txtNombreCompleto = CrearCampo("Nombre completo", 175, false);
            txtEmail = CrearCampo("Correo electrónico", 240, false);
            txtPassword = CrearCampo("Contraseña", 305, true);
            txtRepetirPassword = CrearCampo("Repetir contraseña", 370, true);

            btnCrear = new Button
            {
                Text = "CREAR ADMINISTRADOR",
                Size = new Size(220, 45),
                Location = new Point(250, 415),
                BackColor = Color.FromArgb(111, 192, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCrear.FlatAppearance.BorderSize = 0;
            btnCrear.Click += BtnCrear_Click;
            Controls.Add(btnCrear);
            AcceptButton = btnCrear;
        }

        private TextBox CrearCampo(string etiqueta, int top, bool password)
        {
            Label label = new Label
            {
                Text = etiqueta,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(50, top)
            };
            Controls.Add(label);

            TextBox campo = new TextBox
            {
                Size = new Size(420, 30),
                Location = new Point(50, top + 24),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = password
            };
            Controls.Add(campo);
            return campo;
        }

        private void BtnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtRepetirPassword.Text)
                {
                    throw new InvalidOperationException(
                        "Las contraseñas ingresadas no coinciden.");
                }

                _usuarioService.CrearAdministradorInicial(
                    txtUsuario.Text,
                    txtNombreCompleto.Text,
                    txtEmail.Text,
                    txtPassword.Text);

                MessageBox.Show(
                    "El administrador inicial fue creado correctamente.",
                    "Configuración completada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No fue posible crear el administrador",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
