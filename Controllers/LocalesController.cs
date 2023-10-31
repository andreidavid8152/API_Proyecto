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

            List<Local> locales = await _db.Locales
                .Where(local => local.PropietarioID != userId)
                .Include(local => local.Imagenes) // Incluye la relación Imagenes
                .ToListAsync();
            return Ok(locales);
        }

        // GET: api/Locales/Arrendador/5
        [HttpGet("Arrendador")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocalesArrendador()
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

            List<Local> locales = await _db.Locales.Where(local => local.PropietarioID == userId).ToListAsync();
            return Ok(locales);
        }

        // GET: api/Locales/Cliente/5
        [HttpGet("Cliente/{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocalCliente(int id)
        {
            var localEncontrado = await _db.Locales
                                           .Include(l => l.Horarios)  // Aquí traes también los horarios relacionados con ese local
                                           .Include(l => l.Imagenes)
                                           .FirstOrDefaultAsync(l => l.ID == id);

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

        [HttpPatch("AddHorarios/{localId}")]
        [Authorize]
        public async Task<IActionResult> AddHorarios(int localId, [FromBody] List<Horario> horarios)
        {
            var localExistente = await _db.Locales
                .Include(l => l.Horarios) // Asegúrate de cargar las relaciones existentes
                .FirstOrDefaultAsync(l => l.ID == localId);

            if (localExistente == null)
            {
                return NotFound();
            }

            foreach (var horario in horarios)
            {
                horario.Local = localExistente; // Asignas el local al horario

                // Aquí añades cada nuevo horario a la tabla de horarios
                await _db.Horarios.AddAsync(horario);

                // Y aquí agregas la relación al Local
                localExistente.Horarios.Add(horario);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("AddImagenes/{localId}")]
        [Authorize]
        public async Task<IActionResult> AddImagenesLocal(int localId, [FromBody] List<ImagenLocal> imagenes)
        {

            var localExistente = await _db.Locales
                                        .Include(l => l.Imagenes) // Cargamos las relaciones de imágenes existentes
                                        .FirstOrDefaultAsync(l => l.ID == localId);

            if (localExistente == null)
            {
                return NotFound();
            }

            foreach (var imagen in imagenes)
            {
                imagen.Local = localExistente; // Asignas el local a la imagen

                // Aquí añades cada nueva imagen a la tabla de ImagenesLocales
                await _db.ImagenesLocal.AddAsync(imagen);

                // Y aquí agregas la relación al Local
                localExistente.Imagenes.Add(imagen);
            }

            await _db.SaveChangesAsync();

            return Ok();

        }

    }
}
