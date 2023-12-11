using API_Proyecto.Data;
using API_Proyecto.DTOs;
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

        // Constructor: Aquí se inyectan la base de datos y configuración.
        public LocalesController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Obtiene todos los locales que NO son del usuario actual.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerTodosLocales()
        {

            // Busca el ID del usuario actual en los claims
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si el ID no existe o está vacío se devuelve "Unauthorized"
            if (string.IsNullOrEmpty(claimUserId)) return Unauthorized();

            int userId;

            // Intenta convertir el ID del usuario a un número entero, si falla, devuelve un BadRequest
            if (!int.TryParse(claimUserId, out userId)) return BadRequest("Id de usuario inválido.");

            // Busca en la base de datos todos los locales que no sean propiedad del usuario actual, también incluye las imágenes asociadas a esos locales
            List<Local> locales = await _db.Locales
                .Where(local => local.PropietarioID != userId)
                .Include(local => local.Imagenes)
                .ToListAsync();

            // Devuelve los locales como respuesta válida
            return Ok(locales);
        }

        // Obtiene los locales que SÍ son del usuario actual.
        [HttpGet("Arrendador")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocalesArrendador()
        {

            // Busca el ID del usuario actual en los claims
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si el ID no existe o está vacío, devuelve un Unauthorized
            if (string.IsNullOrEmpty(claimUserId)) return Unauthorized();

            int userId;

            // Intenta convertir el ID del usuario a un número entero, si falla, devuelve BadRequest
            if (!int.TryParse(claimUserId, out userId)) return BadRequest("Id de usuario inválido.");

            // Ahora busca en la base de datos todos los locales que SI son propiedad del usuario actual, también incluye las imágenes asociadas a esos locales
            List<Local> locales = await _db.Locales
                .Where(local => local.PropietarioID == userId)
                .Include(local => local.Imagenes)
                .ToListAsync();

            // Devuelve esos locales como respuesta válida
            return Ok(locales);

        }

        // Obtiene un local específico por su ID.
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerLocal(int id)
        {

            // Busca en la base de datos un local con el ID específico, incluye los horarios e imágenes del local en la búsqueda
            var localEncontrado = await _db.Locales
                .Include(l => l.Horarios)
                .Include(l => l.Imagenes)
                .FirstOrDefaultAsync(l => l.ID == id);

            // Si encuentra el local, lo devuelve como respuesta válida
            if (localEncontrado != null) return Ok(localEncontrado);

            // Si no encuentra el local, devuelve "NotFound"
            return NotFound();

        }

        // Crea un nuevo local.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearLocal([FromBody] Local local)
        {

            // Agrega un nuevo local a la base de datos
            await _db.Locales.AddAsync(local);

            // Guarda los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok(local);

        }

        // Edita los datos de un local existente.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditarLocal([FromBody] Local local)
        {

            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales.FirstOrDefaultAsync(x => x.ID == local.ID);
            if (localExistente == null) return NotFound();

            // Actualiza los datos del local con los datos nuevos
            localExistente.Nombre = local.Nombre != null ? local.Nombre : localExistente.Nombre;
            localExistente.Descripcion = local.Descripcion != null ? local.Descripcion : localExistente.Descripcion;
            localExistente.Direccion = local.Direccion != null ? local.Direccion : localExistente.Direccion;
            localExistente.Capacidad = local.Capacidad != 0 ? local.Capacidad : localExistente.Capacidad;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Añade horarios al local.
        [HttpPost("AddHorarios/{localId}")]
        [Authorize]
        public async Task<IActionResult> AddHorarios(int localId, [FromBody] List<Horario> horarios)
        {

            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales
                .Include(l => l.Horarios)
                .FirstOrDefaultAsync(l => l.ID == localId);

            // Si no encuentra el local, devuelve NotFound
            if (localExistente == null) return NotFound();

            // Agrega los horarios al local
            foreach (var horario in horarios)
            {
                horario.Local = localExistente;
                await _db.Horarios.AddAsync(horario);
                localExistente.Horarios.Add(horario);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        // Obtiene las imagenes de un local específico por su ID.
        [HttpGet("imageneslocal/{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerImagenesLocal(int id)
        {

            // Busca en la base de datos un local con el ID específico, incluye los horarios e imágenes del local en la búsqueda
            var imagenesLocal = await _db.Locales
                .Include(l => l.Imagenes)
                .FirstOrDefaultAsync(l => l.ID == id);

            // Si no encuentra el local, devuelve "NotFound"
            if (imagenesLocal == null) return NotFound();

            // Mapea las entidades Imagen a ImagenLocalDTO
            var imagenesDTO = imagenesLocal.Imagenes
                .Select(i => new ImagenLocalDTO
                {
                    ID = i.ID,
                    Url = i.Url
                    // Añade aquí más propiedades si tu ImagenLocalDTO tiene más campos
                }).ToList();

            // Si encuentra el local, lo devuelve como respuesta válida
            return Ok(imagenesDTO);

        }

        // Obtiene los horarios de un local específico por su ID.
        [HttpGet("horarioslocal/{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerHorariosLocal(int id)
        {

            // Busca en la base de datos un local con el ID específico, incluye los horarios en la búsqueda
            var localConHorarios = await _db.Locales
                .Include(l => l.Horarios)
                .FirstOrDefaultAsync(l => l.ID == id);

            // Si no encuentra el local, devuelve "NotFound"
            if (localConHorarios == null)
            {
                return NotFound($"No se encontró un local con el ID {id}.");
            }

            // Mapea las entidades horarios de los locales a HorarioDTO
            var horariosDTO = localConHorarios.Horarios.Select(h => new HorarioDTO
            {
                ID = h.ID,
                HoraInicio = h.HoraInicio,
                HoraFin = h.HoraFin
                // Añade aquí más propiedades si tu DTO las requiere
            }).ToList();

            // Si encuentra el local, lo devuelve como respuesta válida
            return Ok(horariosDTO);
        }

        // Edita las imágenes del local.
        [HttpPut("HorariosLocal/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditarHorariosLocal(int id, [FromBody] List<HorarioDTO> nuevosHorariosDTO)
        {

            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales
                                .Include(l => l.Horarios)
                                .Include(l => l.Reservas) // Incluye las reservas en la consulta
                                .FirstOrDefaultAsync(l => l.ID == id);

            // Si no encuentra el local, devuelve NotFound
            if (localExistente == null) return NotFound($"No se encontró un local con el ID {id}.");

            // Verifica si hay reservas existentes
            if (localExistente.Reservas.Any())
            {
                return BadRequest("No se pueden modificar los horarios ya que existen reservas.");
            }

            var horariosList = localExistente.Horarios.ToList();

            for (int i = 0; i < horariosList.Count; i++)
            {
                // Ahora puedes indexar porque horariosList es una List<Horario>
                horariosList[i].HoraInicio = nuevosHorariosDTO[i].HoraInicio;
                horariosList[i].HoraFin = nuevosHorariosDTO[i].HoraFin;
                // Actualiza cualquier otra propiedad necesaria aquí
            }

            // Guarda los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok();
        }

        // Añade imágenes al local.
        [HttpPost("AddImagenes/{localId}")]
        [Authorize]
        public async Task<IActionResult> AddImagenesLocal(int localId, [FromBody] List<ImagenLocal> imagenes)
        {
            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales
                .Include(l => l.Imagenes)
                .FirstOrDefaultAsync(l => l.ID == localId);

            // Si no encuentra el local, devuelve NotFound
            if (localExistente == null) return NotFound();

            // Agrega las imágenes al local
            foreach (var imagen in imagenes)
            {
                imagen.Local = localExistente;
                await _db.ImagenesLocal.AddAsync(imagen);
                localExistente.Imagenes.Add(imagen);
            }

            await _db.SaveChangesAsync();

            return Ok();

        }

        // Edita las imágenes del local.
        [HttpPut("Imagenes/Edit/{localId}")]
        [Authorize]
        public async Task<IActionResult> EditarImagenesLocal(int localId, [FromBody] List<ImagenLocalDTO> imagenesNuevasDTO)
        {
            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales
                .Include(l => l.Imagenes)
                .FirstOrDefaultAsync(l => l.ID == localId);

            // Si no encuentra el local, devuelve NotFound
            if (localExistente == null) return NotFound($"No se encontró un local con el ID {localId}.");

            // Verifica que la cantidad de imágenes existentes y las nuevas sea la misma
            if (localExistente.Imagenes.Count != imagenesNuevasDTO.Count)
                return BadRequest("La cantidad de imágenes proporcionadas no coincide con las existentes.");

            // Actualiza las imágenes existentes
            var imagenesList = localExistente.Imagenes.ToList();
            for (int i = 0; i < imagenesList.Count; i++)
            {
                // Aquí asumimos que ImagenLocalDTO tiene las propiedades necesarias para actualizar ImagenLocal
                imagenesList[i].Url = imagenesNuevasDTO[i].Url;
            }

            // Guarda los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok();
        }

        // Elimina un local por su ID.
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminarLocal(int id)
        {

            // Busca un local en la base de datos por su ID
            var localExistente = await _db.Locales
                .Include(l => l.Reservas)
                .FirstOrDefaultAsync(l => l.ID == id);

            // Si no encuentra el local, devuelve NotFound
            if (localExistente == null) return NotFound();

            // Si el local tiene reservas, devuelve BadRequest
            if (localExistente.Reservas != null && localExistente.Reservas.Count > 0)
            {
                return BadRequest("No se puede eliminar el local porque tiene reservas.");
            }

            // Elimina el local de la base de datos
            _db.Locales.Remove(localExistente);

            await _db.SaveChangesAsync();

            // Devuelve un mensaje de éxito
            return Ok("Local eliminado con éxito.");
        }


    }
}
