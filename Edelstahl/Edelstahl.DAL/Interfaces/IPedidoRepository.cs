using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.DAL.Interfaces
{
    /// <summary>
    /// Define las operaciones de persistencia
    /// relacionadas con los pedidos.
    /// </summary>
    public interface IPedidoRepository
        : IGenericRepository<Pedido>
    {
        Pedido GetByNumero(
            string numero);

        Pedido GetByPresupuestoId(
            Guid presupuestoId);

        List<Pedido> GetByClienteId(
            Guid clienteId);

        bool ExistsByNumero(
            string numero);

        bool ExistsByPresupuestoId(
            Guid presupuestoId);
    }
}
