using API_Proyecto.Data;
using API_Proyecto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using API_Proyecto.DTOs;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly ReservacionesDbContext _db;
        private readonly IConfiguration _configuration;

        public UsuariosController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            // Peticion que busca al usuario por el username
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == loginModel.Username);

            // Si no se encuentra el usuario, se retorna un error
            if (usuarioEncontrado == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            if (usuarioEncontrado.Username != "admin")
            {
                return BadRequest("Acceso restringido.");
            }

            // Si se encuentra el usuario, se verifica que la contraseña sea correcta
            if (usuarioEncontrado.Password == loginModel.Password)
            {
                // Si la contraseña es correcta, se genera el token y se retorna
                var tokenString = GenerateJwtToken(usuarioEncontrado);
                return Ok(new { token = tokenString });
            }
            else
            {
                return BadRequest("Contraseña incorrecta.");
            }

        }

        [HttpPost("loginApp")]
        public async Task<IActionResult> LoginApp([FromBody] LoginModel loginModel)
        {

            // Peticion que busca al usuario por el username
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == loginModel.Username);

            // Si no se encuentra el usuario, se retorna un error
            if (usuarioEncontrado == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            // Si se encuentra el usuario, se verifica que la contraseña sea correcta
            if (usuarioEncontrado.Password == loginModel.Password)
            {
                // Si la contraseña es correcta, se genera el token y se retorna
                var tokenString = GenerateJwtToken(usuarioEncontrado);
                return Ok(new { token = tokenString });
            }
            else
            {
                return BadRequest("Contraseña incorrecta.");
            }

        }

        // Metodo que genera el token
        private string GenerateJwtToken(Usuario usuario)
        {

            // Se crean los claims del token
            var claims = new List<Claim>
            {
                // Se agrega el id y el username del usuario
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username)
            };

            // Se crea la llave de seguridad
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // Se crean las credenciales
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Se crea el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Issuer"]
            };

            // Se crea el token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Se crea el token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Se retorna el token
            return tokenHandler.WriteToken(token);

        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UserInputModel usuarioInput)
        {
            // Peticion que busca al usuario por el username
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == usuarioInput.Username);

            // Si no se encuentra el usuario, se crea
            if (usuarioEncontrado == null && usuarioInput != null)
            {

                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = usuarioInput.Nombre,
                    Email = usuarioInput.Email,
                    Username = usuarioInput.Username,
                    Password = usuarioInput.Password
                };

                // Se agrega el usuario a la base de datos y luego guarda
                await _db.Usuarios.AddAsync(nuevoUsuario);
                await _db.SaveChangesAsync();
                return Ok("Registro exitoso");
            }

            // Si se encuentra el usuario, se retorna un error
            return BadRequest("El usuario ya existe.");

        }

        [HttpPut("editarperfil")]
        [Authorize]
        public async Task<IActionResult> Editar([FromBody] UserInputModel usuario)
        {

            // Se obtiene el id del usuario que esta haciendo la peticion
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si no se encuentra el id, se retorna un error
            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            int userId;

            // Si el id no es un numero, se retorna un error
            if (!int.TryParse(claimUserId, out userId))
            {
                return BadRequest("Id de usuario inválido.");
            }

            if (usuario.Id != 0)
            {
                userId = usuario.Id;
            }

            // Se busca el usuario en la base de datos
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == userId);

            // Si no se encuentra el usuario, se retorna un error
            if (usuarioEncontrado == null)
            {
                return NotFound("El usuario no ha sido encontrado.");
            }

            // Se verifica que el username no este en uso
            bool usernameExiste = await _db.Usuarios.AnyAsync(x => x.Username == usuario.Username && x.Id != userId);

            if (usernameExiste)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            // Se actualiza el usuario
            usuarioEncontrado.Username = usuario.Username != null ? usuario.Username : usuarioEncontrado.Username;
            usuarioEncontrado.Password = !string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.All(c => c == '*') ? usuario.Password : usuarioEncontrado.Password;
            usuarioEncontrado.Nombre = usuario.Nombre != null ? usuario.Nombre : usuarioEncontrado.Nombre;
            usuarioEncontrado.Email = usuario.Email != null ? usuario.Email : usuarioEncontrado.Email;

            // Se hace el update y se guarda
            _db.Update(usuarioEncontrado);
            await _db.SaveChangesAsync();

            return Ok(usuarioEncontrado);

        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> GetPerfil()
        {

            // Se obtiene el id del usuario que esta haciendo la peticion
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si no se encuentra el id, se retorna un error
            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            int userId;

            // Si el id no es un numero, se retorna un error
            if (!int.TryParse(claimUserId, out userId))
            {
                return BadRequest("Id de usuario inválido.");
            }

            // Se busca el usuario en la base de datos
            Usuario usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == userId);

            // Si no se encuentra el usuario, se retorna un error
            if (usuario == null)
            {
                return NotFound("El usuario no ha sido encontrado.");
            }

            return Ok(usuario);
        }

        [HttpGet("usuarios")]
        [Authorize]
        public async Task<IActionResult> GetUsuarios()
        {

            // Se obtiene el id del usuario que esta haciendo la peticion
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si no se encuentra el id, se retorna un error
            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            // Se busca el usuario en la base de datos
            List<Usuario> usuarios = await _db.Usuarios.Where(u => u.Id != 1).ToListAsync();

            // Si no se encuentra el usuario, se retorna un error
            if (usuarios == null)
            {
                return NotFound("No hay usuarios.");
            }

            return Ok(usuarios);
        }

        [HttpGet("informacionUsuario/{idUsuario}")]
        [Authorize]
        public async Task<IActionResult> GetInformacionUsuario(int idUsuario)
        {
            // Se busca el usuario en la base de datos
            Usuario? usuario = await _db.Usuarios
                            .Include(u => u.Locales)
                                .ThenInclude(l => l.Horarios)
                            .Include(u => u.Locales)
                                .ThenInclude(l => l.Reservas)
                                    .ThenInclude(r => r.Horario)
                            .Include(u => u.Locales)
                                .ThenInclude(l => l.Comentarios)
                            .Include(u => u.Locales)
                                .ThenInclude(l => l.Imagenes)
                            .Include(u => u.Reservas)
                                .ThenInclude(r => r.Local)
                                    .ThenInclude(l => l.Imagenes) // Asegúrate de cargar las imágenes del Local de la Reserva
                            .Include(u => u.Reservas)
                                .ThenInclude(r => r.Horario)
                            .Include(u => u.Comentarios)
                                .ThenInclude(c => c.Local)
                                    .ThenInclude(l => l.Imagenes)
                            .FirstOrDefaultAsync(x => x.Id == idUsuario);



            // Si no se encuentra el usuario, se retorna un error
            if (usuario == null)
            {
                return NotFound("El usuario no ha sido encontrado.");
            }

            // Mapeo a DTO
            var usuarioDTO = new UsuarioDTO
            {
                Id = usuario.Id,
                Username = usuario.Username,
                Password = usuario.Password,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Locales = usuario.Locales.Select(l => new LocalDTO
                {
                    ID = l.ID,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    Direccion = l.Direccion,
                    Capacidad = l.Capacidad,
                    Horarios = l.Horarios.Select(h => new HorarioDTO
                    {
                        ID = h.ID,
                        HoraInicio = h.HoraInicio,
                        HoraFin = h.HoraFin
                    }).ToList(),
                    Reservas = l.Reservas.Select(r => new ReservaDTO
                    {
                        ID = r.ID,
                        LocalID = r.LocalID,
                        UsuarioID = r.UsuarioID,
                        HorarioID = r.HorarioID,
                        Fecha = r.Fecha,
                        // Nota: No es necesario mapear el Local dentro de Reservas aquí, ya que es redundante
                        Horario = new HorarioDTO
                        {
                            ID = r.Horario.ID,
                            HoraInicio = r.Horario.HoraInicio,
                            HoraFin = r.Horario.HoraFin
                        }
                    }).ToList(),
                    Comentarios = l.Comentarios.Select(c => new ComentarioDTO
                    {
                        ID = c.ID,
                        UsuarioID = c.UsuarioID,
                        LocalID = c.LocalID,
                        Texto = c.Texto,
                        Fecha = c.Fecha,
                        Calificacion = c.Calificacion
                    }).ToList(),
                    Imagenes = l.Imagenes?.Select(i => new ImagenLocalDTO
                    {
                        ID = i.ID,
                        Url = i.Url
                    }).ToList() ?? new List<ImagenLocalDTO>(), // Verifica si Imagenes es null
                }).ToList(),
                Reservas = usuario.Reservas.Select(r => new ReservaDTO
                {
                    ID = r.ID,
                    LocalID = r.LocalID,
                    UsuarioID = r.UsuarioID,
                    HorarioID = r.HorarioID,
                    Fecha = r.Fecha,
                    Local = r.Local != null ? new LocalDTO
                    {
                        ID = r.Local.ID,
                        Nombre = r.Local.Nombre,
                        Descripcion = r.Local.Descripcion,
                        Direccion = r.Local.Direccion,
                        Capacidad = r.Local.Capacidad,
                        Imagenes = r.Local.Imagenes?.Select(img => new ImagenLocalDTO
                        {
                            ID = img.ID,
                            Url = img.Url
                        }).ToList() ?? new List<ImagenLocalDTO>(), // Verifica si Imagenes es null
                    } : null,
                    Horario = r.Horario != null ? new HorarioDTO
                    {
                        ID = r.Horario.ID,
                        HoraInicio = r.Horario.HoraInicio,
                        HoraFin = r.Horario.HoraFin
                    } : null,
                }).ToList(),
                Comentarios = usuario.Comentarios.Select(c => new ComentarioDTO
                {
                    ID = c.ID,
                    UsuarioID = c.UsuarioID,
                    LocalID = c.LocalID,
                    Texto = c.Texto,
                    Fecha = c.Fecha,
                    Calificacion = c.Calificacion,
                    Local = c.Local != null ? new LocalDTO
                    {
                        ID = c.Local.ID,
                        Nombre = c.Local.Nombre,
                        Descripcion = c.Local.Descripcion,
                        Direccion = c.Local.Direccion,
                        Capacidad = c.Local.Capacidad,
                        Imagenes = c.Local.Imagenes?.Select(img => new ImagenLocalDTO
                        {
                            ID = img.ID,
                            Url = img.Url
                        }).ToList() ?? new List<ImagenLocalDTO>(), // Asegúrate de que esto no esté devolviendo una lista vacía por error
                    } : null,
                }).ToList(),
            };



            return Ok(usuarioDTO);
        }
    }
}
