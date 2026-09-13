using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Edelstahl.BLL.Services;

namespace Edelstahl.WinApp
{
    public sealed class FrmLogin : Form
    {
        private readonly UsuarioService _usuarioService;
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnIngresar;
        private CheckBox chkMostrarPassword;

        public FrmLogin()
        {
            _usuarioService = new UsuarioService();
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            Text = "Edelstahl ERP - Inicio de sesión";
            ClientSize = new Size(900, 540);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            Panel panelImagen = new Panel
            {
                Dock = DockStyle.Left,
                Width = 450,
                BackColor = Color.FromArgb(103, 120, 166)
            };
            Controls.Add(panelImagen);

            PictureBox logo = new PictureBox
            {
                Location = new Point(30, 45),
                Size = new Size(390, 390),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = CargarLogo()
            };
            panelImagen.Controls.Add(logo);

            Label marca = new Label
            {
                Text = "EDELSTAHL ERP",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(120, 455)
            };
            panelImagen.Controls.Add(marca);

            Label titulo = new Label
            {
                Text = "Iniciar sesión",
                ForeColor = Color.FromArgb(54, 69, 98),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(510, 75)
            };
            Controls.Add(titulo);

            Label subtitulo = new Label
            {
                Text = "Ingresá tus credenciales para continuar.",
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(513, 120)
            };
            Controls.Add(subtitulo);

            Label lblUsuario = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(515, 180)
            };
            Controls.Add(lblUsuario);

            txtUsuario = new TextBox
            {
                Location = new Point(515, 205),
                Size = new Size(320, 32),
                Font = new Font("Segoe UI", 11)
            };
            Controls.Add(txtUsuario);

            Label lblPassword = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(515, 260)
            };
            Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(515, 285),
                Size = new Size(320, 32),
                Font = new Font("Segoe UI", 11),
                UseSystemPasswordChar = true
            };
            Controls.Add(txtPassword);

            chkMostrarPassword = new CheckBox
            {
                Text = "Mostrar contraseña",
                AutoSize = true,
                Location = new Point(515, 330)
            };
            chkMostrarPassword.CheckedChanged +=
                ChkMostrarPassword_CheckedChanged;
            Controls.Add(chkMostrarPassword);

            btnIngresar = new Button
            {
                Text = "INICIAR SESIÓN",
                Location = new Point(515, 385),
                Size = new Size(320, 48),
                BackColor = Color.FromArgb(103, 120, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;
            Controls.Add(btnIngresar);

            AcceptButton = btnIngresar;
            Shown += delegate { txtUsuario.Focus(); };
        }

        private void ChkMostrarPassword_CheckedChanged(
            object sender,
            EventArgs e)
        {
            txtPassword.UseSystemPasswordChar =
                !chkMostrarPassword.Checked;
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                btnIngresar.Enabled = false;

                var usuario = _usuarioService.Autenticar(
                    txtUsuario.Text,
                    txtPassword.Text);

                SesionActual.Iniciar(usuario);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                txtPassword.Clear();
                txtPassword.Focus();

                MessageBox.Show(
                    ex.Message,
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                btnIngresar.Enabled = true;
            }
        }

        private static Image CargarLogo()
        {
            string carpeta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Login");

            if (!Directory.Exists(carpeta))
            {
                return null;
            }

            string[] archivos = Directory.GetFiles(
                carpeta,
                "logo_login.*");

            if (archivos.Length == 0)
            {
                return null;
            }

            using (Image temporal = Image.FromFile(archivos[0]))
            {
                return new Bitmap(temporal);
            }
        }
    }
}
