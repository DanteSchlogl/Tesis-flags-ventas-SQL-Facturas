using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Edelstahl.Services.Localization;
using Edelstahl.WinApp.Forms.Controls;

namespace Edelstahl.WinApp
{
    public partial class FrmPrincipalV2 : Form
    {
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlWorkspace;
        private Label lblTituloAplicacion;
        private Label lblModulo;
        private Label lblIdioma;
        private ComboBox cboIdioma;
        private Button btnVentasPedidos;
        private Button btnProduccion;
        private Button btnComprasStock;
        private Button btnLogistica;
        private Button btnFinanzas;
        private Button btnAtencionCliente;
        private bool _cambiandoIdioma;

        public FrmPrincipalV2()
        {
            InitializeComponent();
            LanguageService.Inicializar();
            ConfigurarFormulario();
            CrearSidebar();
            CrearHeader();
            CrearWorkspace();
            ConfigurarEventosIdioma();
            CargarIdiomas();
            AplicarIdioma();
            CargarDashboardVentas();
        }

        private void ConfigurarFormulario()
        {
            Text = "Edelstahl ERP";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(235, 240, 245);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1100, 700);
        }

        private void CrearSidebar()
        {
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(103, 120, 166)
            };
            Controls.Add(pnlSidebar);

            lblTituloAplicacion = new Label
            {
                Text = "Edelstahl ERP",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 35)
            };
            pnlSidebar.Controls.Add(lblTituloAplicacion);

            btnVentasPedidos = CrearBotonMenu(120);
            btnVentasPedidos.Click += BtnVentasPedidos_Click;
            btnProduccion = CrearBotonMenu(180);
            btnComprasStock = CrearBotonMenu(240);
            btnLogistica = CrearBotonMenu(300);
            btnFinanzas = CrearBotonMenu(360);
            btnAtencionCliente = CrearBotonMenu(420);
        }

        private Button CrearBotonMenu(int posicionVertical)
        {
            Button boton = new Button
            {
                Width = 220,
                Height = 50,
                Location = new Point(20, posicionVertical),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(103, 120, 166),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.MouseEnter += BotonMenu_MouseEnter;
            boton.MouseLeave += BotonMenu_MouseLeave;
            pnlSidebar.Controls.Add(boton);
            return boton;
        }

        private void BotonMenu_MouseEnter(object sender, EventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                boton.BackColor = Color.FromArgb(168, 188, 235);
            }
        }

        private void BotonMenu_MouseLeave(object sender, EventArgs e)
        {
            Button boton = sender as Button;
            if (boton != null)
            {
                boton.BackColor = Color.FromArgb(103, 120, 166);
            }
        }

        private void CrearHeader()
        {
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(248, 249, 252)
            };
            Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            lblModulo = new Label
            {
                Text = "Gestión Comercial - Ventas",
                ForeColor = Color.FromArgb(54, 69, 98),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 25)
            };
            pnlHeader.Controls.Add(lblModulo);

            lblIdioma = new Label
            {
                Text = "Idioma:",
                ForeColor = Color.FromArgb(54, 69, 98),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlHeader.Controls.Add(lblIdioma);

            cboIdioma = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 24,
                Font = new Font("Segoe UI", 9),
                Size = new Size(250, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cboIdioma.DrawItem += CboIdioma_DrawItem;
            cboIdioma.SelectedIndexChanged += CboIdioma_SelectedIndexChanged;
            pnlHeader.Controls.Add(cboIdioma);

            pnlHeader.Resize += PnlHeader_Resize;
            PnlHeader_Resize(pnlHeader, EventArgs.Empty);
        }

        private void CboIdioma_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0 || e.Index >= cboIdioma.Items.Count)
            {
                return;
            }

            IdiomaItem idioma = cboIdioma.Items[e.Index] as IdiomaItem;
            if (idioma == null)
            {
                return;
            }

            if (idioma.Bandera != null)
            {
                e.Graphics.DrawImage(idioma.Bandera,
                    new Rectangle(e.Bounds.Left + 6, e.Bounds.Top + 4, 24, 16));
            }

            Color colorTexto = (e.State & DrawItemState.Selected) != 0
                ? SystemColors.HighlightText
                : cboIdioma.ForeColor;

            TextRenderer.DrawText(
                e.Graphics,
                idioma.Nombre,
                cboIdioma.Font,
                new Rectangle(e.Bounds.Left + 38, e.Bounds.Top, e.Bounds.Width - 40, e.Bounds.Height),
                colorTexto,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            e.DrawFocusRectangle();
        }

        private void PnlHeader_Resize(object sender, EventArgs e)
        {
            if (pnlHeader == null || lblIdioma == null || cboIdioma == null)
            {
                return;
            }

            cboIdioma.Location = new Point(
                pnlHeader.ClientSize.Width - cboIdioma.Width - 30, 25);
            lblIdioma.Location = new Point(
                cboIdioma.Left - lblIdioma.Width - 15, 30);
        }

        private void CrearWorkspace()
        {
            pnlWorkspace = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(235, 240, 245)
            };
            Controls.Add(pnlWorkspace);
            pnlWorkspace.BringToFront();
        }

        private void ConfigurarEventosIdioma()
        {
            LanguageService.IdiomaCambiado += LanguageService_IdiomaCambiado;
            FormClosed += FrmPrincipalV2_FormClosed;
        }

        private void CargarIdiomas()
        {
            _cambiandoIdioma = true;
            cboIdioma.Items.Clear();
            cboIdioma.Items.Add(new IdiomaItem("es-AR", "Español (Argentina)", "flag_ar.png"));
            cboIdioma.Items.Add(new IdiomaItem("es-ES", "Español (España)", "flag_es.png"));
            cboIdioma.Items.Add(new IdiomaItem("en-US", "English (United States)", "flag_us.png"));
            cboIdioma.Items.Add(new IdiomaItem("en-GB", "English (United Kingdom)", "flag_gb.png"));
            cboIdioma.Items.Add(new IdiomaItem("pt-BR", "Português (Brasil)", "flag_br.png"));
            cboIdioma.Items.Add(new IdiomaItem("de-DE", "Deutsch", "flag_de.png"));
            cboIdioma.Items.Add(new IdiomaItem("ru-RU", "Русский", "flag_ru.png"));
            SeleccionarIdiomaActual();
            _cambiandoIdioma = false;
        }

        private void SeleccionarIdiomaActual()
        {
            for (int i = 0; i < cboIdioma.Items.Count; i++)
            {
                IdiomaItem idioma = cboIdioma.Items[i] as IdiomaItem;
                if (idioma != null && string.Equals(
                    idioma.Codigo,
                    LanguageService.IdiomaActual,
                    StringComparison.OrdinalIgnoreCase))
                {
                    cboIdioma.SelectedIndex = i;
                    return;
                }
            }

            if (cboIdioma.Items.Count > 0)
            {
                cboIdioma.SelectedIndex = 0;
            }
        }

        private void CboIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cambiandoIdioma)
            {
                return;
            }

            IdiomaItem idioma = cboIdioma.SelectedItem as IdiomaItem;
            if (idioma == null)
            {
                return;
            }

            try
            {
                LanguageService.CambiarIdioma(idioma.Codigo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, LanguageService.Translate("Idioma"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _cambiandoIdioma = true;
                SeleccionarIdiomaActual();
                _cambiandoIdioma = false;
            }
        }

        private void LanguageService_IdiomaCambiado(object sender, EventArgs e)
        {
            AplicarIdioma();
        }

        private void AplicarIdioma()
        {
            Text = LanguageService.Translate("TituloAplicacion");
            lblTituloAplicacion.Text = LanguageService.Translate("TituloAplicacion");
            lblModulo.Text = LanguageService.Translate("ModuloVentas");
            lblIdioma.Text = LanguageService.Translate("Idioma") + ":";
            btnVentasPedidos.Text = "📋 " + LanguageService.Translate("VentasPedidos");
            btnProduccion.Text = "🏭 " + LanguageService.Translate("Produccion");
            btnComprasStock.Text = "📦 " + LanguageService.Translate("ComprasStock");
            btnLogistica.Text = "🚚 " + LanguageService.Translate("Logistica");
            btnFinanzas.Text = "💰 " + LanguageService.Translate("Finanzas");
            btnAtencionCliente.Text = "🎧 " + LanguageService.Translate("AtencionCliente");
            PnlHeader_Resize(pnlHeader, EventArgs.Empty);
        }

        private void BtnVentasPedidos_Click(object sender, EventArgs e)
        {
            CargarDashboardVentas();
        }

        private void CargarDashboardVentas()
        {
            pnlWorkspace.Controls.Clear();
            UcVentasDashboard dashboard = new UcVentasDashboard
            {
                Dock = DockStyle.Fill
            };
            pnlWorkspace.Controls.Add(dashboard);
        }

        private void FrmPrincipalV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            LanguageService.IdiomaCambiado -= LanguageService_IdiomaCambiado;
            foreach (object elemento in cboIdioma.Items)
            {
                IdiomaItem idioma = elemento as IdiomaItem;
                if (idioma != null)
                {
                    idioma.Dispose();
                }
            }
        }

        private sealed class IdiomaItem : IDisposable
        {
            public string Codigo { get; private set; }
            public string Nombre { get; private set; }
            public Image Bandera { get; private set; }

            public IdiomaItem(string codigo, string nombre, string nombreArchivo)
            {
                Codigo = codigo;
                Nombre = nombre;
                Bandera = CargarBandera(nombreArchivo);
            }

            private static Image CargarBandera(string nombreArchivo)
            {
                string[] carpetasIniciales =
                {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Application.StartupPath,
                    Environment.CurrentDirectory
                };

                foreach (string carpetaInicial in carpetasIniciales)
                {
                    string rutaEncontrada = BuscarBandera(
                        carpetaInicial,
                        nombreArchivo);

                    if (!string.IsNullOrWhiteSpace(rutaEncontrada))
                    {
                        using (Image temporal = Image.FromFile(rutaEncontrada))
                        {
                            return new Bitmap(temporal);
                        }
                    }
                }

                return null;
            }

            private static string BuscarBandera(
                string carpetaInicial,
                string nombreArchivo)
            {
                if (string.IsNullOrWhiteSpace(carpetaInicial))
                {
                    return null;
                }

                DirectoryInfo carpeta = new DirectoryInfo(carpetaInicial);

                for (int nivel = 0;
                    nivel < 8 && carpeta != null;
                    nivel++)
                {
                    string rutaDirecta = Path.Combine(carpeta.FullName,
                        "Resources",
                        "Flags",
                        nombreArchivo);

                    if (File.Exists(rutaDirecta))
                    {
                        return rutaDirecta;
                    }

                    string rutaProyecto = Path.Combine(
                        carpeta.FullName,
                        "Edelstahl.WinApp",
                        "Resources",
                        "Flags",
                        nombreArchivo);

                    if (File.Exists(rutaProyecto))
                    {
                        return rutaProyecto;
                    }

                    carpeta = carpeta.Parent;
                }

                return null;
            }

            public void Dispose()
            {
                if (Bandera != null)
                {
                    Bandera.Dispose();
                    Bandera = null;
                }
            }

            public override string ToString()
            {
                return Nombre;
            }
        }
    }
}
