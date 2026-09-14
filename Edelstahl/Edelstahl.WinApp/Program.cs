using System;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.DAL.EntityFramework;
using Edelstahl.WinApp.Infrastructure;

namespace Edelstahl.WinApp
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal de Edelstahl ERP.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(
                false);

            GlobalExceptionHandler.Registrar();

            try
            {
                InicializadorNegocio.Inicializar();

                UsuarioService usuarioService =
                    new UsuarioService();

                if (!usuarioService.ExisteAdministrador())
                {
                    using (
                        FrmCrearAdministradorInicial configuracion =
                            new FrmCrearAdministradorInicial())
                    {
                        if (configuracion.ShowDialog() !=
                            DialogResult.OK)
                        {
                            return;
                        }
                    }
                }

                using (FrmLogin login =
                    new FrmLogin())
                {
                    if (login.ShowDialog() !=
                        DialogResult.OK)
                    {
                        return;
                    }
                }

                Application.Run(
                    new FrmPrincipalV2());
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.ProcesarErrorInicio(
                    ex);
            }
            finally
            {
                SesionActual.Cerrar();
            }
        }
    }
}
