using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Proyecto.Models
{
    public class ImagenLocal
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Url { get; set; } // Ruta o URL de la imagen.
        [Required]
        public int LocalID { get; set; } // El ID del local al que pertenece la imagen.

        // Relación
        [JsonIgnore]
        public Local? Local { get; set; }
    }
}
