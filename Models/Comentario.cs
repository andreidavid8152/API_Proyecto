using System.ComponentModel.DataAnnotations;

namespace API_Proyecto.Models
{
    public class Comentario
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int LocalID { get; set; }
        [Required]
        public int UsuarioID { get; set; }
        [Required]
        public string Texto { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public int Calificacion { get; set; }

        // Relaciones
        public Local? Local { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
