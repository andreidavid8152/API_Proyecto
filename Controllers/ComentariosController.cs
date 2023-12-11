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
    public class ComentariosController : ControllerBase
    {
        private readonly ReservacionesDbContext _db;
        private readonly IConfiguration _configuration;

        // Constructor: Aquí se inyectan la base de datos y configuración.
        public ComentariosController(ReservacionesDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Ruta para obtener los comentarios de un usuario
        [HttpGet("{usuarioId}")]
        [Authorize]
        public async Task<IActionResult> ObtenerComentariosUsuario(int usuarioId)
        {
            // Obtenemos los comentarios del usuario
            var comentarios = await _db.Comentarios
                            .Include(comentario => comentario.Local)
                                .ThenInclude(local => local.Imagenes) // Añadimos esta línea
                            .Where(comentario => comentario.UsuarioID == usuarioId)
                            .ToListAsync();



            if (!comentarios.Any())
            {
                return NotFound("No se encontraron comentarios para el usuario especificado.");
            }

            var comentariosDto = comentarios.Select(comentario => new ComentarioDTO
            {
                ID = comentario.ID,
                LocalID = comentario.LocalID,
                UsuarioID = comentario.UsuarioID,
                Texto = comentario.Texto,
                Fecha = comentario.Fecha,
                Calificacion = comentario.Calificacion,
                Local = new LocalDTO
                {
                    Nombre = comentario.Local.Nombre,
                    Imagenes = comentario.Local.Imagenes.Select(imagen => new ImagenLocalDTO
                    {
                        ID = imagen.ID,
                        Url = imagen.Url
                    }).ToList()
                }
            }).ToList();


            return Ok(comentariosDto);
        }

        [HttpDelete("{comentarioId}")]
        [Authorize]
        public async Task<IActionResult> EliminarComentario(int comentarioId)
        {
            var comentario = await _db.Comentarios.FindAsync(comentarioId);
            if (comentario == null)
            {
                return NotFound("Comentario no encontrado.");
            }

            _db.Comentarios.Remove(comentario);
            await _db.SaveChangesAsync();

            return Ok("Comentario eliminado exitosamente.");
        }

    }
}
