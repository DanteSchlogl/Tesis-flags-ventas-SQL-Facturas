using System.Data.Entity;

namespace Edelstahl.DAL.EntityFramework
{
    /// <summary>
    /// Inicializa la conexión con la base de datos
    /// comercial administrada mediante Entity Framework 6.
    ///
    /// La estructura de la base EdelstahlNegocio se
    /// administra mediante scripts SQL y cambios
    /// controlados, por lo que Entity Framework no
    /// debe crear, eliminar ni recrear la base.
    /// </summary>
    public static class InicializadorNegocio
    {
        public static void Inicializar()
        {
            /*
             * Se deshabilita el inicializador automático.
             *
             * Esto evita que Entity Framework intente
             * comparar o recrear la base cuando cambia
             * alguna configuración del modelo.
             */
            Database.SetInitializer<
                EdelstahlNegocioContext>(
                    null);

            /*
             * Se abre el contexto y se comprueba que
             * la base de datos sea accesible.
             */
            using (EdelstahlNegocioContext contexto =
                new EdelstahlNegocioContext())
            {
                contexto.Database.Initialize(
                    false);
            }
        }
    }
}