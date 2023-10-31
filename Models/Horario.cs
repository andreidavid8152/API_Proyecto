using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Local? Local { get; set; }
    }
}
