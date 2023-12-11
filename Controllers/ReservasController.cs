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
    public class ReservasController : ControllerBase
    {

        private readonly ReservacionesDbContext _db;
        private readonly IConfiguration _configuration;

        // Constructor: Aquí se inyectan la base de datos y configuración.
        public ReservasController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Ruta para obtener todas las reservas
        [HttpGet("Cliente")]
        [Authorize]
        public async Task<IActionResult> ObtenerReservasCliente()
        {

            // Obtenemos el id del usuario que está haciendo la petición
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Si no se encuentra el id del usuario, se regresa un error 401
            if (string.IsNullOrEmpty(claimUserId))
            {
                return Unauthorized();
            }

            int userId;

            // Si el id del usuario no es un número, se regresa un error 400
            if (!int.TryParse(claimUserId, out userId))
            {
                return BadRequest("Id de usuario inválido.");
            }

            // Obtenemos las reservas del usuario
            var reservas = await _db.Reservas
               .Where(reserva => reserva.UsuarioID == userId)
               .Include(r => r.Local.Imagenes)
               .Include(r => r.Local.Horarios)
               .ToListAsync();

            return Ok(reservas);
        }

        // Ruta para crear una nueva reserva
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Reservar([FromBody] Reserva reserva)
        {

            // Comprobamos que los datos de la reserva sean válidos
            if (reserva == null)
            {
                return BadRequest("Datos inválidos.");
            }

            // Realizamos una peticion a la base de datos para ver si la reserva ya existe
            var existeReserva = await _db.Reservas.AnyAsync(r => r.LocalID == reserva.LocalID && r.HorarioID == reserva.HorarioID && r.Fecha == reserva.Fecha);

            // Si la reserva ya existe, regresamos un error 400 con un mensaje
            if (existeReserva)
            {
                return BadRequest("Este horario ya está reservado para la fecha seleccionada.");
            }

            // Agregamos la reserva a la base de datos
            _db.Reservas.Add(reserva);

            // Guardamos los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok("Reservación realizada con éxito.");
        }

        // Ruta para verificar la disponibilidad de una reserva
        [HttpGet("verificarDisponibilidad")]
        public async Task<IActionResult> VerificarDisponibilidad(int localId, int horarioId, DateTime fecha)
        {
            // Realizamos una peticion a la base de datos para ver si la reserva ya existe
            var existeReserva = await _db.Reservas.AnyAsync(r => r.LocalID == localId && r.HorarioID == horarioId && r.Fecha == fecha);

            // Devuelve true si la reserva no existe, false si ya existe
            return Ok(!existeReserva);
        }

        // Ruta para comentar una reserva
        [HttpPost("comentar")]
        [Authorize]
        public async Task<IActionResult> ComentarReserva([FromBody] Comentario comentario)
        {
            // Verificamos que el comentario no sea nulo
            if (comentario == null)
            {
                return BadRequest("Datos inválidos.");
            }

            // Añadimos el comentario a la base de datos
            await _db.Comentarios.AddAsync(comentario);

            // Guardamos los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok("Comentario añadido con éxito.");

        }

        // Ruta para eliminar una reserva
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminarReserva(int id)
        {
            // Buscamos la reserva por ID
            var reserva = await _db.Reservas.FindAsync(id);

            // Si no encontramos la reserva, devolvemos un error 404
            if (reserva == null)
            {
                return NotFound("Reserva no encontrada.");
            }

            // Eliminamos la reserva de la base de datos
            _db.Reservas.Remove(reserva);

            // Guardamos los cambios en la base de datos
            await _db.SaveChangesAsync();

            return Ok("Reserva eliminada con éxito.");
        }

    }
}
