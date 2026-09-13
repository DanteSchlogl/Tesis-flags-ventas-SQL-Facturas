using System;
using Edelstahl.Domain.Security;

namespace Edelstahl.BLL.Services
{
    /// <summary>
    /// Mantiene los datos del usuario autenticado durante la ejecución.
    /// </summary>
    public static class SesionActual
    {
        public static Usuario Usuario { get; private set; }

        public static bool EstaIniciada
        {
            get { return Usuario != null; }
        }

        public static void Iniciar(Usuario usuario)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }

        public static void Cerrar()
        {
            Usuario = null;
        }

        public static bool TieneRol(string nombreRol)
        {
            return EstaIniciada &&
                   Usuario.Rol != null &&
                   string.Equals(
                       Usuario.Rol.Nombre,
                       nombreRol,
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
