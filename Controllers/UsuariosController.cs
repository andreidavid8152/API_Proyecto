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

        // POST api/<UsuariosController>/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == loginModel.Username);

            if (usuarioEncontrado == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            if (usuarioEncontrado.Password == loginModel.Password)
            {
                var tokenString = GenerateJwtToken(usuarioEncontrado);
                return Ok(new { token = tokenString });
            }
            else
            {
                return BadRequest("Contraseña incorrecta.");
            }
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // Si tu usuario tiene un campo Id
                new Claim(ClaimTypes.Name, usuario.Username)
                // Aquí puedes agregar más claims si lo necesitas.
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(1), // Este token expira en 1 hora, puedes ajustarlo a tus necesidades.
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UserInputModel usuarioInput)
        {
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == usuarioInput.Username);

            if (usuarioEncontrado == null && usuarioInput != null)
            {
                // Convertimos UserInputModel a Usuario
                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = usuarioInput.Nombre,
                    Email = usuarioInput.Email,
                    Username = usuarioInput.Username,
                    Password = usuarioInput.Password
                };

                await _db.Usuarios.AddAsync(nuevoUsuario);
                await _db.SaveChangesAsync();
                return Ok("Registro exitoso");
            }

            return BadRequest("El usuario ya existe.");
        }

        // PUT api/<UsuariosController>/editarperfil
        [HttpPut("editarperfil")]
        [Authorize]
        public async Task<IActionResult> Editar([FromBody] UserInputModel usuario)
        {
            // Obtiene el claim del usuario actual
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            int userId;
            if (!int.TryParse(claimUserId, out userId))
            {
                return BadRequest("Id de usuario inválido.");
            }

            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == userId);

            if (usuarioEncontrado == null)
            {
                return NotFound("El usuario no ha sido encontrado.");
            }

            // Antes de cambiar el nombre de usuario, verifica si el nuevo nombre de usuario ya está en uso
            // pero excluye al propio usuario de esta comprobación.
            bool usernameExiste = await _db.Usuarios.AnyAsync(x => x.Username == usuario.Username && x.Id != userId);
            if (usernameExiste)
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            usuarioEncontrado.Username = usuario.Username != null ? usuario.Username : usuarioEncontrado.Username;
            usuarioEncontrado.Password = !string.IsNullOrWhiteSpace(usuario.Password) && !usuario.Password.All(c => c == '*')
                ? usuario.Password
                : usuarioEncontrado.Password;
            usuarioEncontrado.Nombre = usuario.Nombre != null ? usuario.Nombre : usuarioEncontrado.Nombre;
            usuarioEncontrado.Email = usuario.Email != null ? usuario.Email : usuarioEncontrado.Email;

            _db.Update(usuarioEncontrado);
            await _db.SaveChangesAsync();

            return Ok(usuarioEncontrado);
        }


        // GET api/<UsuariosController>/miperfil
        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> GetPerfil()
        {
            // Obtiene el claim del usuario actual
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            int userId;
            if (!int.TryParse(claimUserId, out userId))
            {
                return BadRequest("Id de usuario inválido.");
            }

            Usuario usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == userId);
            if (usuario == null)
            {
                return NotFound("El usuario no ha sido encontrado.");
            }
            return Ok(usuario);
        }

    }
}
