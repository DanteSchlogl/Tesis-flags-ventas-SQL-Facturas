using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Edelstahl.DAL.EntityFramework;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Comercial;

namespace Edelstahl.DAL.Implementations.EntityFramework
{
    /// <summary>
    /// Persistencia de clientes mediante Entity Framework 6.
    /// </summary>
    public class ClienteRepositoryEntityFramework
        : IClienteRepository
    {
        public void Add(Cliente entity)
        {
            ValidarCliente(entity);

            entity.CUIT = NormalizarCUIT(entity.CUIT);

            if (ExistsByCUIT(entity.CUIT))
            {
                throw new InvalidOperationException(
                    "Ya existe un cliente con el CUIT ingresado.");
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            if (entity.FechaAlta == default(DateTime))
            {
                entity.FechaAlta = DateTime.Now;
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                context.Clientes.Add(entity);
                context.SaveChanges();
            }
        }

        public void Update(Cliente entity)
        {
            ValidarCliente(entity);

            if (entity.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(entity));
            }

            entity.CUIT = NormalizarCUIT(entity.CUIT);

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                bool cuitDuplicado = context.Clientes.Any(
                    cliente =>
                        cliente.CUIT == entity.CUIT &&
                        cliente.Id != entity.Id);

                if (cuitDuplicado)
                {
                    throw new InvalidOperationException(
                        "Ya existe otro cliente con el CUIT ingresado.");
                }

                Cliente existente = context.Clientes.Find(entity.Id);

                if (existente == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró el cliente que se desea modificar.");
                }

                CopiarDatos(entity, existente);
                context.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del cliente no es válido.",
                    nameof(id));
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                Cliente existente = context.Clientes.Find(id);

                if (existente == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró el cliente que se desea eliminar.");
                }

                existente.Activo = false;
                context.SaveChanges();
            }
        }

        public Cliente GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                return context.Clientes
                    .AsNoTracking()
                    .FirstOrDefault(cliente => cliente.Id == id);
            }
        }

        public List<Cliente> GetAll()
        {
            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                return context.Clientes
                    .AsNoTracking()
                    .OrderBy(cliente => cliente.RazonSocial)
                    .ToList();
            }
        }

        public Cliente GetByCUIT(string cuit)
        {
            string cuitNormalizado = NormalizarCUIT(cuit);

            if (string.IsNullOrWhiteSpace(cuitNormalizado))
            {
                return null;
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                return context.Clientes
                    .AsNoTracking()
                    .FirstOrDefault(
                        cliente => cliente.CUIT == cuitNormalizado);
            }
        }

        public bool ExistsByCUIT(string cuit)
        {
            string cuitNormalizado = NormalizarCUIT(cuit);

            if (string.IsNullOrWhiteSpace(cuitNormalizado))
            {
                return false;
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                return context.Clientes.Any(
                    cliente => cliente.CUIT == cuitNormalizado);
            }
        }

        private static void CopiarDatos(
            Cliente origen,
            Cliente destino)
        {
            destino.CUIT = origen.CUIT;
            destino.RazonSocial = Limpiar(origen.RazonSocial);
            destino.DireccionFacturacion = Limpiar(origen.DireccionFacturacion);
            destino.DireccionEntrega = Limpiar(origen.DireccionEntrega);
            destino.Localidad = Limpiar(origen.Localidad);
            destino.Provincia = Limpiar(origen.Provincia);
            destino.CodigoPostal = Limpiar(origen.CodigoPostal);
            destino.Email = Limpiar(origen.Email);
            destino.Telefono = Limpiar(origen.Telefono);
            destino.LimiteCredito = origen.LimiteCredito;
            destino.DeudaActual = origen.DeudaActual;
            destino.Activo = origen.Activo;
            destino.FechaAlta = origen.FechaAlta;
        }

        private static void ValidarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente));
            }

            if (string.IsNullOrWhiteSpace(cliente.CUIT))
            {
                throw new InvalidOperationException(
                    "El CUIT es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                throw new InvalidOperationException(
                    "La razón social es obligatoria.");
            }

            if (cliente.LimiteCredito < 0m ||
                cliente.DeudaActual < 0m)
            {
                throw new InvalidOperationException(
                    "El límite de crédito y la deuda no pueden ser negativos.");
            }
        }

        private static string NormalizarCUIT(string cuit)
        {
            return string.IsNullOrWhiteSpace(cuit)
                ? string.Empty
                : cuit.Trim()
                    .Replace("-", string.Empty)
                    .Replace(" ", string.Empty)
                    .Replace(".", string.Empty);
        }

        private static string Limpiar(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }
    }
}
