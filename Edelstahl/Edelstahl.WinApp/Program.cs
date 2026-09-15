using System;
using System.Windows.Forms;
using Edelstahl.BLL.Services;
using Edelstahl.DAL.EntityFramework;
using Edelstahl.DAL.Factory;
using Edelstahl.Services.Configuration;
using Edelstahl.WinApp.Forms.Startup;
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
                ApplicationMode.Reset();

                using (FrmSeleccionModo selectorModo =
                    new FrmSeleccionModo())
                {
                    if (selectorModo.ShowDialog() !=
                        DialogResult.OK)
                    {
                        return;
                    }
                }

                FactoryDataAccess.Inicializar(
                    ApplicationMode.CurrentMode);

                if (ApplicationMode.UsesSqlServer)
                {
                    InicializadorNegocio.Inicializar();

                    VerificarAdministradorInicial();
                }

                if (!MostrarLogin())
                {
                    return;
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

                ApplicationMode.Reset();
            }
        }

        private static void VerificarAdministradorInicial()
        {
            UsuarioService usuarioService =
                new UsuarioService();

            if (usuarioService.ExisteAdministrador())
            {
                return;
            }

            using (
                FrmCrearAdministradorInicial configuracion =
                    new FrmCrearAdministradorInicial())
            {
                configuracion.ShowDialog();
            }
        }

        private static bool MostrarLogin()
        {
            using (FrmLogin login =
                new FrmLogin())
            {
                return login.ShowDialog() ==
                    DialogResult.OK;
            }
        }
    }
}