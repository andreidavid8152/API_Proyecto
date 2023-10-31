using API_Proyecto.Data;
using API_Proyecto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // POST: api/Reservas
        [HttpPost]
        [Authorize]
        public IActionResult Reservar([FromBody] Reserva reserva)
        {
            if (reserva == null)
            {
                return BadRequest("Datos inválidos.");
            }

            // Verificar si ya existe una reservación con los mismos datos
            var existeReserva = _db.Reservas.Any(r => r.LocalID == reserva.LocalID && r.HorarioID == reserva.HorarioID && r.Fecha == reserva.Fecha);
            if (existeReserva)
            {
                return BadRequest("Este horario ya está reservado para la fecha seleccionada.");
            }

            // Crear la nueva reservación
            _db.Reservas.Add(reserva);
            _db.SaveChanges();

            return Ok("Reservación realizada con éxito.");
        }

        // GET: api/Reservas/verificarDisponibilidad
        [HttpGet("verificarDisponibilidad")]
        public IActionResult VerificarDisponibilidad(int localId, int horarioId, DateTime fecha)
        {
            var existeReserva = _db.Reservas.Any(r => r.LocalID == localId && r.HorarioID == horarioId && r.Fecha == fecha);
            return Ok(!existeReserva);
        }

    }
}
