using System;
using System.Collections.Generic;
using Edelstahl.DAL.Factory;
using Edelstahl.DAL.Interfaces;
using Edelstahl.Domain.Security;
using Edelstahl.Services.Security;

namespace Edelstahl.BLL.Services
{
    /// <summary>
    /// Coordina el registro, autenticación y administración
    /// de los usuarios de Edelstahl ERP.
    /// </summary>
    public class UsuarioService
    {
        private const int IntentosMaximos = 5;
        private static readonly TimeSpan DuracionBloqueo =
            TimeSpan.FromMinutes(15);

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;

        public UsuarioService()
        {
            _usuarioRepository =
                FactoryDataAccess.UsuarioRepository;

            _rolRepository =
                FactoryDataAccess.RolRepository;
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
                    "No se encontró el rol seleccionado.");
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

            return usuario;
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
                throw new InvalidOperationException(
                    "Debe ingresar el usuario y la contraseña.");
            }

            Usuario usuario =
                _usuarioRepository.GetByNombreUsuario(
                    usuarioNormalizado);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "El usuario o la contraseña son incorrectos.");
            }

            if (!usuario.Activo)
            {
                throw new InvalidOperationException(
                    "El usuario se encuentra inactivo.");
            }

            if (usuario.EstaBloqueado())
            {
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
                    throw new InvalidOperationException(
                        "Se alcanzó el máximo de intentos. " +
                        "El usuario fue bloqueado durante 15 minutos.");
                }

                int intentosRestantes =
                    IntentosMaximos - usuario.IntentosFallidos;

                throw new InvalidOperationException(
                    "El usuario o la contraseña son incorrectos. " +
                    "Intentos restantes: " +
                    intentosRestantes +
                    ".");
            }

            usuario.RegistrarAccesoCorrecto();
            _usuarioRepository.Update(usuario);

            return usuario;
        }

        public void CambiarPassword(
            Guid usuarioId,
            string passwordActual,
            string passwordNueva)
        {
            Usuario usuario = ObtenerPorId(usuarioId);

            if (!PasswordHasher.Verificar(
                passwordActual,
                usuario.PasswordHash,
                usuario.PasswordSalt,
                usuario.Iteraciones))
            {
                throw new InvalidOperationException(
                    "La contraseña actual es incorrecta.");
            }

            if (string.Equals(
                passwordActual,
                passwordNueva,
                StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "La contraseña nueva debe ser diferente de la actual.");
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
        }

        public Usuario ObtenerPorId(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del usuario no es válido.",
                    nameof(usuarioId));
            }

            Usuario usuario =
                _usuarioRepository.GetById(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException(
                    "No se encontró el usuario solicitado.");
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
        }

        public void CambiarEstado(
            Guid usuarioId,
            bool activo)
        {
            Usuario usuario = ObtenerPorId(usuarioId);
            usuario.Activo = activo;
            _usuarioRepository.Update(usuario);
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
                    "El correo electrónico no es válido.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "La contraseña es obligatoria.",
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
