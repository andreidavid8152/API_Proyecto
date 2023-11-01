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

            List<Local> locales = await _db.Locales
                .Where(local => local.PropietarioID == userId)
                .Include(local => local.Imagenes) // Incluye la relación Imagenes
                .ToListAsync();
            return Ok(locales);
        }

        // GET: api/Locales/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocal(int id)
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
        [Authorize]
        public async Task<IActionResult> EditarLocal([FromBody] Local local)
        {
            // Busca el local en la base de datos usando el ID
            var localExistente = await _db.Locales.FirstOrDefaultAsync(x => x.ID == local.ID);

            if (localExistente == null)
            {
                return NotFound(); // Retorna 404 si el local no se encuentra
            }

            // Actualiza sólo los campos que tienen valor
            localExistente.Nombre = local.Nombre != null ? local.Nombre : localExistente.Nombre;
            localExistente.Descripcion = local.Descripcion != null ? local.Descripcion : localExistente.Descripcion;
            localExistente.Direccion = local.Direccion != null ? local.Direccion : localExistente.Direccion;
            localExistente.Capacidad = local.Capacidad != 0 ? local.Capacidad : localExistente.Capacidad;


            // Guarda los cambios
            await _db.SaveChangesAsync();

            return NoContent(); // Retorna 204, indicando que la operación fue exitosa pero no hay contenido para retornar
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

        // PUT: api/Locales/Imagenes/5
        [HttpPut("Imagenes/Edit/{localId}")]
        [Authorize]
        public async Task<IActionResult> EditarImagenesLocal(int localId, [FromBody] List<ImagenLocal> imagenesNuevas)
        {
            var localExistente = await _db.Locales
                                        .Include(l => l.Imagenes) // Cargamos las imágenes existentes
                                        .FirstOrDefaultAsync(l => l.ID == localId);

            if (localExistente == null)
            {
                return NotFound(); // Retorna 404 si el local no se encuentra
            }

            // Elimina las imágenes antiguas del local
            _db.ImagenesLocal.RemoveRange(localExistente.Imagenes);

            // Añade las nuevas imágenes
            foreach (var imagen in imagenesNuevas)
            {
                // Aquí, puedes asignar el ID del local a cada nueva imagen, 
                // aunque no sea estrictamente necesario si usas Entity Framework correctamente.
                imagen.Local = localExistente;

                // Añade cada nueva imagen a la tabla de ImagenesLocales
                await _db.ImagenesLocal.AddAsync(imagen);
            }

            // Guarda los cambios
            await _db.SaveChangesAsync();

            return Ok(); // Retorna 200, indicando éxito en la operación
        }

        // DELETE: api/Locales/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminarLocal(int id)
        {
            // Busca el local en la base de datos usando el ID
            var localExistente = await _db.Locales
                                           .Include(l => l.Reservas)  // Cargamos las reservas existentes para ese local
                                           .FirstOrDefaultAsync(l => l.ID == id);

            // Si el local no se encuentra, retornamos 404
            if (localExistente == null)
            {
                return NotFound();
            }

            // Si el local tiene reservas, no se puede eliminar
            if (localExistente.Reservas != null && localExistente.Reservas.Count > 0)
            {
                return BadRequest("No se puede eliminar el local porque tiene reservas.");
            }

            // Elimina el local
            _db.Locales.Remove(localExistente);

            // Guarda los cambios
            await _db.SaveChangesAsync();

            return Ok("Local eliminado con éxito.");
        }


    }
}
