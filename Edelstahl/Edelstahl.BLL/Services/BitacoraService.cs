using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Edelstahl.DAL.Factory;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Security;

namespace Edelstahl.BLL.Services
{
    public class BitacoraService
    {
        private readonly IBitacoraRepository _repository;

        public BitacoraService()
        {
            _repository = FactoryDataAccess.BitacoraRepository;
        }

        public BitacoraService(IBitacoraRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Registrar(
            string modulo,
            string accion,
            string entidad,
            Guid? entidadId,
            string descripcion,
            string resultado,
            string tipoEvento,
            string detalleTecnico = "",
            string nombreUsuarioAlternativo = "")
        {
            try
            {
                RegistroBitacora registro = new RegistroBitacora
                {
                    Id = Guid.NewGuid(),
                    FechaHora = DateTime.Now,
                    UsuarioId = SesionActual.EstaIniciada
                        ? (Guid?)SesionActual.Usuario.Id : null,
                    NombreUsuario = SesionActual.EstaIniciada
                        ? SesionActual.Usuario.NombreUsuario
                        : NormalizarUsuario(nombreUsuarioAlternativo),
                    Modulo = Limitar(modulo, 100),
                    Accion = Limitar(accion, 150),
                    Entidad = Limitar(entidad, 100),
                    EntidadId = entidadId,
                    Descripcion = Limitar(descripcion, 1000),
                    Resultado = resultado,
                    DireccionEquipo = ObtenerDireccionEquipo(),
                    NombreEquipo = Limitar(Environment.MachineName, 150),
                    TipoEvento = tipoEvento,
                    DetalleTecnico = detalleTecnico ?? string.Empty
                };

                _repository.Add(registro);
            }
            catch
            {
                // La bitácora nunca debe interrumpir la operación principal.
            }
        }

        public void RegistrarInformacion(
            string modulo,
            string accion,
            string descripcion)
        {
            Registrar(modulo, accion, string.Empty, null, descripcion,
                "Correcto", "Informacion");
        }

        public void RegistrarAuditoria(
            string modulo,
            string accion,
            string entidad,
            Guid? entidadId,
            string descripcion)
        {
            Registrar(modulo, accion, entidad, entidadId, descripcion,
                "Correcto", "Auditoria");
        }

        public void RegistrarSeguridad(
            string accion,
            string descripcion,
            bool correcto,
            string nombreUsuario = "")
        {
            Registrar("Seguridad", accion, "Usuario", null, descripcion,
                correcto ? "Correcto" : "Fallido",
                "Seguridad", string.Empty, nombreUsuario);
        }

        public void RegistrarError(
            string modulo,
            string accion,
            Exception exception,
            string descripcion = "")
        {
            Registrar(modulo, accion, string.Empty, null,
                string.IsNullOrWhiteSpace(descripcion)
                    ? "Se produjo un error en la operación." : descripcion,
                "Fallido", "Error",
                exception == null ? string.Empty : exception.ToString());
        }

        public List<RegistroBitacora> ObtenerTodos()
        {
            return _repository.GetAll();
        }

        public List<RegistroBitacora> ObtenerPorFecha(
            DateTime desde,
            DateTime hasta)
        {
            return _repository.GetByFecha(desde, hasta);
        }

        private static string ObtenerDireccionEquipo()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress direccion in host.AddressList)
                {
                    if (direccion.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return direccion.ToString();
                    }
                }
            }
            catch
            {
            }

            return "No disponible";
        }

        private static string NormalizarUsuario(string usuario)
        {
            return string.IsNullOrWhiteSpace(usuario)
                ? "ANONIMO"
                : Limitar(usuario.Trim().ToLowerInvariant(), 100);
        }

        private static string Limitar(string valor, int maximo)
        {
            string limpio = string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();

            return limpio.Length <= maximo
                ? limpio
                : limpio.Substring(0, maximo);
        }
    }
}
