using System.ComponentModel.DataAnnotations;

namespace API_Proyecto.Models
{
    public class Horario
    {

        [Key]
        public int ID { get; set; }
        [Required]
        public int LocalID { get; set; }
        [Required]
        public TimeSpan HoraInicio { get; set; }
        [Required]
        public TimeSpan HoraFin { get; set; }

        // Relaciones
        public Local Local { get; set; }
    }
}
