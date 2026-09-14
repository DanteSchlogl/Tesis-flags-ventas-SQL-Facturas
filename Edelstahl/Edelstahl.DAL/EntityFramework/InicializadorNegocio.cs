using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace Edelstahl.DAL.EntityFramework
{
    /// <summary>
    /// Inicializa la base de datos comercial administrada
    /// mediante Entity Framework 6.
    /// </summary>
    public static class InicializadorNegocio
    {
        public static void Inicializar()
        {
            Database.SetInitializer(
                new CreateDatabaseIfNotExists<
                    EdelstahlNegocioContext>());

            using (EdelstahlNegocioContext contexto =
                new EdelstahlNegocioContext())
            {
                contexto.Database.Initialize(false);
            }
        }
    }
}


