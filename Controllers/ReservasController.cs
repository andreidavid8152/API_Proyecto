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


        public ReservasController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // GET: api/Reservas
        [HttpGet("Cliente")]
        [Authorize]
        public async Task<IActionResult> ObtenerReservasCliente()
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


            var reservas = await _db.Reservas
               .Where(reserva => reserva.UsuarioID == userId)
               .Include(r => r.Local.Imagenes)
               .Include(r => r.Local.Horarios)
               .ToListAsync();

            return Ok(reservas);
        }


        // POST: api/Reservas
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Reservar([FromBody] Reserva reserva)
        {
            if (reserva == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var existeReserva = await _db.Reservas.AnyAsync(r => r.LocalID == reserva.LocalID && r.HorarioID == reserva.HorarioID && r.Fecha == reserva.Fecha);
            if (existeReserva)
            {
                return BadRequest("Este horario ya está reservado para la fecha seleccionada.");
            }

            _db.Reservas.Add(reserva);
            await _db.SaveChangesAsync();

            return Ok("Reservación realizada con éxito.");
        }


        // GET: api/Reservas/verificarDisponibilidad
        [HttpGet("verificarDisponibilidad")]
        public async Task<IActionResult> VerificarDisponibilidad(int localId, int horarioId, DateTime fecha)
        {
            var existeReserva = await _db.Reservas.AnyAsync(r => r.LocalID == localId && r.HorarioID == horarioId && r.Fecha == fecha);
            return Ok(!existeReserva);
        }


    }
}
