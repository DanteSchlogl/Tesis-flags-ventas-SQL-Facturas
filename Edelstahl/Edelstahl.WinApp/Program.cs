using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edelstahl.BLL.Services;

namespace Edelstahl.WinApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                UsuarioService usuarioService = new UsuarioService();

                if (!usuarioService.ExisteAdministrador())
                {
                    using (FrmCrearAdministradorInicial configuracion =
                        new FrmCrearAdministradorInicial())
                    {
                        if (configuracion.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }
                    }
                }

                using (FrmLogin login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                }

                Application.Run(new FrmPrincipalV2());
                SesionActual.Cerrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible iniciar Edelstahl ERP." +
                    Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    "Error de inicio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

