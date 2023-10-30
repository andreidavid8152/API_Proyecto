using System.ComponentModel.DataAnnotations;

namespace API_Proyecto.Models
{
    public class Reserva
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int LocalID { get; set; }
        [Required]
        public int UsuarioID { get; set; }
        [Required]
        public int HorarioID { get; set; } // Relación con Horario
        [Required]
        public DateTime Fecha { get; set; }

        // Relaciones
        public Local Local { get; set; }
        public Usuario Usuario { get; set; }

    }
}
