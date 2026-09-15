using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using Edelstahl.DAL.EntityFramework;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Comercial;
using Edelstahl.Services.Cryptography;

namespace Edelstahl.DAL.Implementations.EntityFramework
{
    /// <summary>
    /// Persistencia de clientes mediante Entity Framework 6.
    ///
    /// El CUIT se cifra mediante AES antes de almacenarse
    /// y se descifra después de recuperarse.
    /// </summary>
    public class ClienteRepositoryEntityFramework
        : IClienteRepository
    {
        public void Add(Cliente entity)
        {
            ValidarCliente(entity);

            string cuitNormalizado =
                NormalizarCUIT(entity.CUIT);

            if (ExistsByCUIT(cuitNormalizado))
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

            Cliente clientePersistente =
                CrearCopiaParaPersistencia(entity);

            clientePersistente.CUIT =
                CifrarCUIT(cuitNormalizado);

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                context.Clientes.Add(clientePersistente);
                context.SaveChanges();
            }

            // El objeto utilizado por la BLL conserva
            // el CUIT normalizado y legible.
            entity.CUIT = cuitNormalizado;
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

            string cuitNormalizado =
                NormalizarCUIT(entity.CUIT);

            Cliente clienteConMismoCUIT =
                ObtenerPorCUITInterno(
                    cuitNormalizado,
                    entity.Id);

            if (clienteConMismoCUIT != null)
            {
                throw new InvalidOperationException(
                    "Ya existe otro cliente con el CUIT ingresado.");
            }

            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                Cliente existente =
                    context.Clientes.Find(entity.Id);

                if (existente == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró el cliente que se desea modificar.");
                }

                CopiarDatosParaPersistencia(
                    entity,
                    existente,
                    cuitNormalizado);

                context.SaveChanges();
            }

            // El objeto utilizado por la BLL conserva
            // el CUIT normalizado y legible.
            entity.CUIT = cuitNormalizado;
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
                Cliente existente =
                    context.Clientes.Find(id);

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
                Cliente cliente =
                    context.Clientes
                        .AsNoTracking()
                        .FirstOrDefault(
                            item => item.Id == id);

                DescifrarCUITDelCliente(cliente);

                return cliente;
            }
        }

        public List<Cliente> GetAll()
        {
            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                List<Cliente> clientes =
                    context.Clientes
                        .AsNoTracking()
                        .OrderBy(
                            cliente => cliente.RazonSocial)
                        .ToList();

                foreach (Cliente cliente in clientes)
                {
                    DescifrarCUITDelCliente(cliente);
                }

                return clientes;
            }
        }

        public Cliente GetByCUIT(string cuit)
        {
            string cuitNormalizado =
                NormalizarCUIT(cuit);

            if (string.IsNullOrWhiteSpace(
                cuitNormalizado))
            {
                return null;
            }

            return ObtenerPorCUITInterno(
                cuitNormalizado,
                null);
        }

        public bool ExistsByCUIT(string cuit)
        {
            string cuitNormalizado =
                NormalizarCUIT(cuit);

            if (string.IsNullOrWhiteSpace(
                cuitNormalizado))
            {
                return false;
            }

            return ObtenerPorCUITInterno(
                cuitNormalizado,
                null) != null;
        }

        private static Cliente ObtenerPorCUITInterno(
            string cuitNormalizado,
            Guid? idExcluido)
        {
            using (EdelstahlNegocioContext context =
                new EdelstahlNegocioContext())
            {
                List<Cliente> clientes =
                    context.Clientes
                        .AsNoTracking()
                        .ToList();

                foreach (Cliente cliente in clientes)
                {
                    string cuitLegible =
                        ObtenerCUITLegible(cliente.CUIT);

                    string cuitClienteNormalizado =
                        NormalizarCUIT(cuitLegible);

                    bool mismoCUIT =
                        string.Equals(
                            cuitClienteNormalizado,
                            cuitNormalizado,
                            StringComparison.Ordinal);

                    bool registroPermitido =
                        !idExcluido.HasValue ||
                        cliente.Id != idExcluido.Value;

                    if (mismoCUIT &&
                        registroPermitido)
                    {
                        cliente.CUIT = cuitLegible;
                        return cliente;
                    }
                }
            }

            return null;
        }

        private static Cliente CrearCopiaParaPersistencia(
            Cliente origen)
        {
            return new Cliente
            {
                Id = origen.Id,
                CUIT = origen.CUIT,
                RazonSocial =
                    Limpiar(origen.RazonSocial),
                DireccionFacturacion =
                    Limpiar(origen.DireccionFacturacion),
                DireccionEntrega =
                    Limpiar(origen.DireccionEntrega),
                Localidad =
                    Limpiar(origen.Localidad),
                Provincia =
                    Limpiar(origen.Provincia),
                CodigoPostal =
                    Limpiar(origen.CodigoPostal),
                Email =
                    Limpiar(origen.Email),
                Telefono =
                    Limpiar(origen.Telefono),
                LimiteCredito =
                    origen.LimiteCredito,
                DeudaActual =
                    origen.DeudaActual,
                Activo =
                    origen.Activo,
                FechaAlta =
                    origen.FechaAlta
            };
        }

        private static void CopiarDatosParaPersistencia(
            Cliente origen,
            Cliente destino,
            string cuitNormalizado)
        {
            destino.CUIT =
                CifrarCUIT(cuitNormalizado);

            destino.RazonSocial =
                Limpiar(origen.RazonSocial);

            destino.DireccionFacturacion =
                Limpiar(origen.DireccionFacturacion);

            destino.DireccionEntrega =
                Limpiar(origen.DireccionEntrega);

            destino.Localidad =
                Limpiar(origen.Localidad);

            destino.Provincia =
                Limpiar(origen.Provincia);

            destino.CodigoPostal =
                Limpiar(origen.CodigoPostal);

            destino.Email =
                Limpiar(origen.Email);

            destino.Telefono =
                Limpiar(origen.Telefono);

            destino.LimiteCredito =
                origen.LimiteCredito;

            destino.DeudaActual =
                origen.DeudaActual;

            destino.Activo =
                origen.Activo;

            destino.FechaAlta =
                origen.FechaAlta;
        }

        private static string CifrarCUIT(
            string cuitNormalizado)
        {
            if (string.IsNullOrWhiteSpace(
                cuitNormalizado))
            {
                return string.Empty;
            }

            return CryptographyService.Encrypt(
                cuitNormalizado);
        }

        private static void DescifrarCUITDelCliente(
            Cliente cliente)
        {
            if (cliente == null)
            {
                return;
            }

            cliente.CUIT =
                ObtenerCUITLegible(cliente.CUIT);
        }

        private static string ObtenerCUITLegible(
            string valorAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(
                valorAlmacenado))
            {
                return string.Empty;
            }

            // Compatibilidad temporal con clientes
            // almacenados antes de incorporar AES.
            if (EsCUITEnTextoPlano(valorAlmacenado))
            {
                return NormalizarCUIT(
                    valorAlmacenado);
            }

            try
            {
                return CryptographyService.Decrypt(
                    valorAlmacenado);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException(
                    "No fue posible verificar o " +
                    "descifrar el CUIT almacenado.",
                    ex);
            }
        }

        private static bool EsCUITEnTextoPlano(
            string valor)
        {
            string normalizado =
                NormalizarCUIT(valor);

            if (normalizado.Length != 11)
            {
                return false;
            }

            foreach (char caracter in normalizado)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ValidarCliente(
            Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(
                    nameof(cliente));
            }

            if (string.IsNullOrWhiteSpace(
                cliente.CUIT))
            {
                throw new InvalidOperationException(
                    "El CUIT es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(
                cliente.RazonSocial))
            {
                throw new InvalidOperationException(
                    "La razón social es obligatoria.");
            }

            if (cliente.LimiteCredito < 0m ||
                cliente.DeudaActual < 0m)
            {
                throw new InvalidOperationException(
                    "El límite de crédito y la deuda " +
                    "no pueden ser negativos.");
            }
        }

        private static string NormalizarCUIT(
            string cuit)
        {
            return string.IsNullOrWhiteSpace(cuit)
                ? string.Empty
                : cuit.Trim()
                    .Replace("-", string.Empty)
                    .Replace(" ", string.Empty)
                    .Replace(".", string.Empty);
        }

        private static string Limpiar(
            string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }
    }
}