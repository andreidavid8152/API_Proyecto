using API_Proyecto.Data;
using API_Proyecto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly ReservacionesDbContext _db;

        public UsuariosController(ReservacionesDbContext db)
        {
            _db = db;
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

            // Aquí deberías verificar la contraseña. 
            // Por simplicidad, estoy haciendo una comparación directa, pero en un entorno real, deberías usar una verificación más segura.
            if (usuarioEncontrado.Password == loginModel.Password)
            {
                // Inicio de sesión exitoso. Dependiendo de tu estructura, podrías devolver un token o simplemente confirmar el inicio de sesión.
                return Ok("Inicio de sesión exitoso"); // O devuelve el token si implementas esa lógica.
            }
            else
            {
                return BadRequest("Contraseña incorrecta.");
            }
        }


        // GET: api/<UsuariosController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Usuario> usuarios = await _db.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {
            Usuario usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == Id);
            if (usuario == null)
            {
                return BadRequest("El usuario no ha sido encontrado.");
            }
            return Ok(usuario);
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Username == usuario.Username);

            if (usuarioEncontrado == null && usuario != null)
            {

                await _db.Usuarios.AddAsync(usuario);
                await _db.SaveChangesAsync();
                return Ok("Registro exitoso");

            }

            return BadRequest("El usuario ya existe.");
        }

        // PUT api/<UsuariosController>/5
        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(int Id, [FromBody] Usuario usuario)
        {

            Usuario usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == Id);

            if (usuarioEncontrado != null)
            {

                bool usernameExiste = await _db.Usuarios.AnyAsync(x => x.Username == usuario.Username);
                if (usernameExiste)
                {
                    return BadRequest("El nombre de usuario ya está en uso.");
                }

                usuarioEncontrado.Username = usuario.Username != null ? usuario.Username : usuarioEncontrado.Username;
                usuarioEncontrado.Password = usuario.Password != null ? usuario.Password : usuarioEncontrado.Password;
                usuarioEncontrado.Nombre = usuario.Nombre != null ? usuario.Nombre : usuarioEncontrado.Nombre;
                usuarioEncontrado.Email = usuario.Email != null ? usuario.Email : usuarioEncontrado.Email;

                _db.Update(usuarioEncontrado);
                await _db.SaveChangesAsync();
                return Ok(usuarioEncontrado);
            }


            return BadRequest("El usuario no ha sido encontrado.");

        }

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {

            Usuario usuario = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id == Id);


            if (usuario != null)
            {
                _db.Usuarios.Remove(usuario);
                await _db.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest("El usuario no ha sido encontrado.");

        }
    }
}
