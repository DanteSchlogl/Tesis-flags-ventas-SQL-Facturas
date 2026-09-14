using System;
using System.Collections.Generic;
using Edelstahl.DAL.Factory;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Security;
using Edelstahl.Services.Security;

namespace Edelstahl.BLL.Services
{
    /// <summary>
    /// Coordina el registro, autenticacion y administracion
    /// de los usuarios de Edelstahl ERP.
    /// </summary>
    public class UsuarioService
    {
        private const int IntentosMaximos = 5;

        private static readonly TimeSpan DuracionBloqueo =
            TimeSpan.FromMinutes(15);

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;
        private readonly BitacoraService _bitacoraService;

        public UsuarioService()
        {
            _usuarioRepository =
                FactoryDataAccess.UsuarioRepository;

            _rolRepository =
                FactoryDataAccess.RolRepository;

            _bitacoraService =
                new BitacoraService();
        }

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository)
        {
            _usuarioRepository = usuarioRepository
                ?? throw new ArgumentNullException(
                    nameof(usuarioRepository));

            _rolRepository = rolRepository
                ?? throw new ArgumentNullException(
                    nameof(rolRepository));

            _bitacoraService =
                new BitacoraService();
        }

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository,
            BitacoraService bitacoraService)
        {
            _usuarioRepository = usuarioRepository
                ?? throw new ArgumentNullException(
                    nameof(usuarioRepository));

            _rolRepository = rolRepository
                ?? throw new ArgumentNullException(
                    nameof(rolRepository));

            _bitacoraService = bitacoraService
                ?? throw new ArgumentNullException(
                    nameof(bitacoraService));
        }

        public Usuario Registrar(
            string nombreUsuario,
            string nombreCompleto,
            string email,
            string password,
            string nombreRol,
            bool debeCambiarPassword = true)
        {
            string usuarioNormalizado =
                NormalizarNombreUsuario(nombreUsuario);

            string emailNormalizado =
                NormalizarEmail(email);

            try
            {
                ValidarDatosRegistro(
                    usuarioNormalizado,
                    nombreCompleto,
                    emailNormalizado,
                    password,
                    nombreRol);

                if (_usuarioRepository.ExistsByNombreUsuario(
                    usuarioNormalizado))
                {
                    throw new InvalidOperationException(
                        "Ya existe un usuario con el nombre ingresado.");
                }

                if (_usuarioRepository.ExistsByEmail(
                    emailNormalizado))
                {
                    throw new InvalidOperationException(
                        "Ya existe un usuario con el correo ingresado.");
                }

                Rol rol = _rolRepository.GetByNombre(
                    nombreRol.Trim());

                if (rol == null)
                {
                    throw new InvalidOperationException(
                        "No se encontro el rol seleccionado.");
                }

                if (!rol.Activo)
                {
                    throw new InvalidOperationException(
                        "El rol seleccionado no se encuentra activo.");
                }

                PasswordHashData datosPassword =
                    PasswordHasher.CrearHash(password);

                Usuario usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    RolId = rol.Id,
                    NombreUsuario = usuarioNormalizado,
                    NombreCompleto = nombreCompleto.Trim(),
                    Email = emailNormalizado,
                    PasswordHash = datosPassword.Hash,
                    PasswordSalt = datosPassword.Salt,
                    Iteraciones = datosPassword.Iteraciones,
                    Activo = true,
                    DebeCambiarPassword = debeCambiarPassword,
                    IntentosFallidos = 0,
                    BloqueadoHasta = null,
                    FechaAlta = DateTime.Now,
                    UltimoAcceso = null,
                    Rol = rol
                };

                _usuarioRepository.Add(usuario);

                _bitacoraService.Registrar(
                    "Seguridad",
                    "Alta de usuario",
                    "Usuario",
                    usuario.Id,
                    "Se registro el usuario " +
                    usuario.NombreUsuario +
                    " con el rol " +
                    rol.Nombre +
                    ".",
                    "Correcto",
                    "Auditoria",
                    string.Empty,
                    usuario.NombreUsuario);

                return usuario;
            }
            catch (Exception ex)
            {
                _bitacoraService.Registrar(
                    "Seguridad",
                    "Alta de usuario",
                    "Usuario",
                    null,
                    "No fue posible registrar el usuario.",
                    "Fallido",
                    "Error",
                    ex.ToString(),
                    usuarioNormalizado);

                throw;
            }
        }

        public Usuario Autenticar(
            string nombreUsuario,
            string password)
        {
            string usuarioNormalizado =
                NormalizarNombreUsuario(nombreUsuario);

            if (string.IsNullOrWhiteSpace(usuarioNormalizado) ||
                string.IsNullOrEmpty(password))
            {
                _bitacoraService.RegistrarSeguridad(
                    "Inicio de sesion",
                    "Se intento iniciar sesion sin completar " +
                    "el usuario o la contrasena.",
                    false,
                    usuarioNormalizado);

                throw new InvalidOperationException(
                    "Debe ingresar el usuario y la contrasena.");
            }

            Usuario usuario =
                _usuarioRepository.GetByNombreUsuario(
                    usuarioNormalizado);

            if (usuario == null)
            {
                _bitacoraService.RegistrarSeguridad(
                    "Inicio de sesion",
                    "Intento de acceso con un usuario inexistente.",
                    false,
                    usuarioNormalizado);

                throw new InvalidOperationException(
                    "El usuario o la contrasena son incorrectos.");
            }

            if (!usuario.Activo)
            {
                _bitacoraService.Registrar(
                    "Seguridad",
                    "Inicio de sesion",
                    "Usuario",
                    usuario.Id,
                    "Intento de acceso con un usuario inactivo.",
                    "Fallido",
                    "Seguridad",
                    string.Empty,
                    usuario.NombreUsuario);

                throw new InvalidOperationException(
                    "El usuario se encuentra inactivo.");
            }

            if (usuario.EstaBloqueado())
            {
                _bitacoraService.Registrar(
                    "Seguridad",
                    "Inicio de sesion",
                    "Usuario",
                    usuario.Id,
                    "Intento de acceso con un usuario bloqueado hasta " +
                    usuario.BloqueadoHasta.Value.ToString(
                        "dd/MM/yyyy HH:mm") +
                    ".",
                    "Fallido",
                    "Seguridad",
                    string.Empty,
                    usuario.NombreUsuario);

                throw new InvalidOperationException(
                    "El usuario se encuentra bloqueado hasta " +
                    usuario.BloqueadoHasta.Value.ToString(
                        "dd/MM/yyyy HH:mm") +
                    ".");
            }

            bool passwordValido = PasswordHasher.Verificar(
                password,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.Iteraciones);

            if (!passwordValido)
            {
                usuario.RegistrarAccesoFallido(
                    IntentosMaximos,
                    DuracionBloqueo);

                _usuarioRepository.Update(usuario);

                if (usuario.EstaBloqueado())
                {
                    _bitacoraService.Registrar(
                        "Seguridad",
                        "Bloqueo de usuario",
                        "Usuario",
                        usuario.Id,
                        "El usuario fue bloqueado durante 15 minutos " +
                        "al alcanzar el maximo de intentos fallidos.",
                        "Advertencia",
                        "Seguridad",
                        string.Empty,
                        usuario.NombreUsuario);

                    throw new InvalidOperationException(
                        "Se alcanzo el maximo de intentos. " +
                        "El usuario fue bloqueado durante 15 minutos.");
                }

                int intentosRestantes =
                    IntentosMaximos - usuario.IntentosFallidos;

                _bitacoraService.Registrar(
                    "Seguridad",
                    "Inicio de sesion",
                    "Usuario",
                    usuario.Id,
                    "Contrasena incorrecta. Intentos restantes: " +
                    intentosRestantes +
                    ".",
                    "Fallido",
                    "Seguridad",
                    string.Empty,
                    usuario.NombreUsuario);

                throw new InvalidOperationException(
                    "El usuario o la contrasena son incorrectos. " +
                    "Intentos restantes: " +
                    intentosRestantes +
                    ".");
            }

            usuario.RegistrarAccesoCorrecto();
            _usuarioRepository.Update(usuario);

            _bitacoraService.Registrar(
                "Seguridad",
                "Inicio de sesion",
                "Usuario",
                usuario.Id,
                "Inicio de sesion correcto.",
                "Correcto",
                "Seguridad",
                string.Empty,
                usuario.NombreUsuario);

            return usuario;
        }

        public void CambiarPassword(
            Guid usuarioId,
            string passwordActual,
            string passwordNueva)
        {
            Usuario usuario = ObtenerPorId(usuarioId);

            try
            {
                if (!PasswordHasher.Verificar(
                    passwordActual,
                    usuario.PasswordHash,
                    usuario.PasswordSalt,
                    usuario.Iteraciones))
                {
                    _bitacoraService.Registrar(
                        "Seguridad",
                        "Cambio de contrasena",
                        "Usuario",
                        usuario.Id,
                        "La contrasena actual ingresada es incorrecta.",
                        "Fallido",
                        "Seguridad",
                        string.Empty,
                        usuario.NombreUsuario);

                    throw new InvalidOperationException(
                        "La contrasena actual es incorrecta.");
                }

                if (string.Equals(
                    passwordActual,
                    passwordNueva,
                    StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "La contrasena nueva debe ser diferente de la actual.");
                }

                PasswordHashData datosPassword =
                    PasswordHasher.CrearHash(passwordNueva);

                usuario.PasswordHash = datosPassword.Hash;
                usuario.PasswordSalt = datosPassword.Salt;
                usuario.Iteraciones = datosPassword.Iteraciones;
                usuario.DebeCambiarPassword = false;
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta = null;

                _usuarioRepository.Update(usuario);

                _bitacoraService.Registrar(
                    "Seguridad",
                    "Cambio de contrasena",
                    "Usuario",
                    usuario.Id,
                    "La contrasena fue modificada correctamente.",
                    "Correcto",
                    "Auditoria",
                    string.Empty,
                    usuario.NombreUsuario);
            }
            catch (Exception ex)
            {
                if (!(ex is InvalidOperationException) ||
                    ex.Message != "La contrasena actual es incorrecta.")
                {
                    _bitacoraService.Registrar(
                        "Seguridad",
                        "Cambio de contrasena",
                        "Usuario",
                        usuario.Id,
                        "No fue posible modificar la contrasena.",
                        "Fallido",
                        "Error",
                        ex.ToString(),
                        usuario.NombreUsuario);
                }

                throw;
            }
        }

        public Usuario ObtenerPorId(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del usuario no es valido.",
                    nameof(usuarioId));
            }

            Usuario usuario =
                _usuarioRepository.GetById(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "No se encontro el usuario solicitado.");
            }

            return usuario;
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarioRepository.GetAll();
        }

        public List<Rol> ObtenerRoles()
        {
            return _rolRepository.GetAll();
        }

        public bool ExisteAdministrador()
        {
            Rol rolAdministrador =
                _rolRepository.GetByNombre("Administrador");

            if (rolAdministrador == null)
            {
                return false;
            }

            List<Usuario> administradores =
                _usuarioRepository.GetByRolId(
                    rolAdministrador.Id);

            foreach (Usuario usuario in administradores)
            {
                if (usuario.Activo)
                {
                    return true;
                }
            }

            return false;
        }

        public Usuario CrearAdministradorInicial(
            string nombreUsuario,
            string nombreCompleto,
            string email,
            string password)
        {
            if (ExisteAdministrador())
            {
                throw new InvalidOperationException(
                    "Ya existe un usuario administrador activo.");
            }

            return Registrar(
                nombreUsuario,
                nombreCompleto,
                email,
                password,
                "Administrador",
                false);
        }

        public void Desbloquear(Guid usuarioId)
        {
            Usuario usuario = ObtenerPorId(usuarioId);

            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;

            _usuarioRepository.Update(usuario);

            _bitacoraService.RegistrarAuditoria(
                "Seguridad",
                "Desbloqueo de usuario",
                "Usuario",
                usuario.Id,
                "El usuario " +
                usuario.NombreUsuario +
                " fue desbloqueado.");
        }

        public void CambiarEstado(
            Guid usuarioId,
            bool activo)
        {
            Usuario usuario = ObtenerPorId(usuarioId);
            usuario.Activo = activo;
            _usuarioRepository.Update(usuario);

            _bitacoraService.RegistrarAuditoria(
                "Seguridad",
                activo
                    ? "Activacion de usuario"
                    : "Desactivacion de usuario",
                "Usuario",
                usuario.Id,
                "El usuario " +
                usuario.NombreUsuario +
                (activo
                    ? " fue activado."
                    : " fue desactivado."));
        }

        private static void ValidarDatosRegistro(
            string nombreUsuario,
            string nombreCompleto,
            string email,
            string password,
            string nombreRol)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException(
                    "El nombre de usuario es obligatorio.",
                    nameof(nombreUsuario));
            }

            if (nombreUsuario.Length < 4)
            {
                throw new ArgumentException(
                    "El nombre de usuario debe contener al menos 4 caracteres.",
                    nameof(nombreUsuario));
            }

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new ArgumentException(
                    "El nombre completo es obligatorio.",
                    nameof(nombreCompleto));
            }

            if (string.IsNullOrWhiteSpace(email) ||
                !EmailPareceValido(email))
            {
                throw new ArgumentException(
                    "El correo electronico no es valido.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "La contrasena es obligatoria.",
                    nameof(password));
            }

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                throw new ArgumentException(
                    "El rol es obligatorio.",
                    nameof(nombreRol));
            }
        }

        private static string NormalizarNombreUsuario(
            string nombreUsuario)
        {
            return string.IsNullOrWhiteSpace(nombreUsuario)
                ? string.Empty
                : nombreUsuario.Trim().ToLowerInvariant();
        }

        private static string NormalizarEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? string.Empty
                : email.Trim().ToLowerInvariant();
        }

        private static bool EmailPareceValido(string email)
        {
            int arroba = email.IndexOf('@');
            int punto = email.LastIndexOf('.');

            return arroba > 0 &&
                   punto > arroba + 1 &&
                   punto < email.Length - 1;
        }
    }
}
