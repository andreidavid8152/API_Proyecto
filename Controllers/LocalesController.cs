using API_Proyecto.Data;
using API_Proyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalesController : ControllerBase
    {

        private readonly ReservacionesDbContext _db;
        private readonly IConfiguration _configuration;

        public LocalesController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // GET: api/Locales
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerTodosLocales()
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

            List<Local> locales = await _db.Locales.Where(local => local.PropietarioID != userId).ToListAsync();
            return Ok(locales);
        }

        // GET: api/Locales/5
        [HttpGet("Cliente/{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocalCliente(int id)
        {
            var localEncontrado = await _db.Locales.FindAsync(id);  // Buscar el local por su id. 

            if (localEncontrado != null)
            {
                return Ok(localEncontrado);  // Si encuentras el local, retornas un 200 OK con el local como cuerpo de la respuesta.
            }
            return NotFound();  // Si no encuentras el local, retornas un 404 Not Found.
        }

        // POST: api/Locales
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearLocal([FromBody] Local local)
        {
            await _db.Locales.AddAsync(local);
            await _db.SaveChangesAsync();
            return Ok(local);
        }

        // PUT: api/Locales/5
        [HttpPut("{id}")]
        public IActionResult EditarLocal(int id, [FromBody] Local local)
        {
            // ... Actualizar un local
            return null;
        }

        // DELETE: api/Locales/5
        [HttpDelete("{id}")]
        public IActionResult EliminarLocal(int id)
        {
            // ... Eliminar un local
            return null;
        }
    }
}
