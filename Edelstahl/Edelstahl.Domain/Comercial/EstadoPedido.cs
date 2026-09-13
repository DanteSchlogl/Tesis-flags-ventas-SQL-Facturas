using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edelstahl.Domain.Comercial
{
    public enum EstadoPedido
    {
        Pendiente = 0,
        Confirmado = 1,
        EnPreparacion = 2,
        EnProduccion = 3,
        ListoParaEntrega = 4,
        Entregado = 5,
        Cancelado = 6
    }
}
