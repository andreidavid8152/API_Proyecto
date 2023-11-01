using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Proyecto.Models
{
    public class Local
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int PropietarioID { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public int Capacidad { get; set; }
        

        // Relaciones
        public Usuario? Propietario { get; set; }
        public ICollection<Horario>? Horarios { get; set; }
        [JsonIgnore]
        public ICollection<Reserva>? Reservas { get; set; }
        public ICollection<Comentario>? Comentarios { get; set; }
        public ICollection<ImagenLocal>? Imagenes { get; set; }

    }
}
