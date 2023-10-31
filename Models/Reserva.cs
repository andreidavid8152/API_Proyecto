using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("LocalID")]
        public Local Local { get; set; }
        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }
        [ForeignKey("HorarioID")]
        public Horario Horario { get; set; }


    }
}
