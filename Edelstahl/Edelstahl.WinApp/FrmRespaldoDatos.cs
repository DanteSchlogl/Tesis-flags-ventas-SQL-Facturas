using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.Domain.Security;

namespace Edelstahl.WinApp
{
    public sealed class FrmRespaldoDatos : Form
    {
        private readonly RespaldoService _respaldoService;
        private TextBox txtCarpeta;
        private TextBox txtResultado;
        private Button btnCrear;
        private Button btnAbrirCarpeta;

        public FrmRespaldoDatos()
        {
            _respaldoService = new RespaldoService();
            CrearInterfaz();
            CargarCarpeta();
        }

        private void CrearInterfaz()
        {
            Text = "Respaldo de datos";
            ClientSize = new Size(720, 430);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(235, 240, 245);

            Label titulo = new Label
            {
                Text = "Respaldo de bases de datos",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(54, 69, 98),
                AutoSize = true,
                Location = new Point(35, 25)
            };
            Controls.Add(titulo);

            Label descripcion = new Label
            {
                Text = "Se crearán copias completas de EdelstahlNegocio y " +
                       "EdelstahlServicios.",
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(38, 70)
            };
            Controls.Add(descripcion);

            Label lblCarpeta = new Label
            {
                Text = "Carpeta administrada por SQL Server",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 110)
            };
            Controls.Add(lblCarpeta);

            txtCarpeta = new TextBox
            {
                Location = new Point(40, 135),
                Size = new Size(640, 28),
                ReadOnly = true
            };
            Controls.Add(txtCarpeta);

            btnCrear = new Button
            {
                Text = "CREAR RESPALDO",
                Location = new Point(40, 185),
                Size = new Size(230, 45),
                BackColor = Color.FromArgb(111, 192, 145),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCrear.FlatAppearance.BorderSize = 0;
            btnCrear.Click += BtnCrear_Click;
            Controls.Add(btnCrear);

            btnAbrirCarpeta = new Button
            {
                Text = "ABRIR CARPETA",
                Location = new Point(290, 185),
                Size = new Size(190, 45),
                BackColor = Color.FromArgb(151, 178, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAbrirCarpeta.FlatAppearance.BorderSize = 0;
            btnAbrirCarpeta.Click += BtnAbrirCarpeta_Click;
            Controls.Add(btnAbrirCarpeta);

            txtResultado = new TextBox
            {
                Location = new Point(40, 255),
                Size = new Size(640, 130),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9)
            };
            Controls.Add(txtResultado);
        }

        private void CargarCarpeta()
        {
            try
            {
                txtCarpeta.Text =
                    _respaldoService.ObtenerCarpetaRespaldos();
            }
            catch (Exception ex)
            {
                txtResultado.Text = ex.Message;
                btnCrear.Enabled = false;
                btnAbrirCarpeta.Enabled = false;
            }
        }

        private void BtnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                btnCrear.Enabled = false;
                txtResultado.Text = "Creando respaldos...";
                Application.DoEvents();

                List<ResultadoRespaldo> resultados =
                    _respaldoService.CrearRespaldoCompleto();

                StringBuilder texto = new StringBuilder();
                texto.AppendLine("Respaldos creados correctamente:");
                texto.AppendLine();

                foreach (ResultadoRespaldo resultado in resultados)
                {
                    texto.AppendLine(resultado.BaseDatos);
                    texto.AppendLine(resultado.RutaArchivo);
                    texto.AppendLine(
                        "Tamaño: " +
                        (resultado.TamanioBytes / 1024d / 1024d)
                            .ToString("N2") +
                        " MB");
                    texto.AppendLine();
                }

                txtResultado.Text = texto.ToString();

                MessageBox.Show(
                    "Las dos bases fueron respaldadas correctamente.",
                    "Respaldo completado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                txtResultado.Text = ex.Message;

                MessageBox.Show(
                    "No fue posible crear el respaldo." +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    "Error de respaldo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnCrear.Enabled = true;
            }
        }

        private void BtnAbrirCarpeta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCarpeta.Text) ||
                !Directory.Exists(txtCarpeta.Text))
            {
                MessageBox.Show(
                    "La carpeta de respaldos no está disponible.",
                    "Carpeta no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = txtCarpeta.Text,
                UseShellExecute = true
            });
        }
    }
}
