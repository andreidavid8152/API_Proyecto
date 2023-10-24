using System.ComponentModel.DataAnnotations;

namespace API_Proyecto.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; } // Un identificador único para cada usuario.
        [Required]
        public string Username { get; set; } // El nombre de usuario.
        [Required]
        public string Password { get; set; } // La contraseña del usuario.
        [Required]
        public string Nombre { get; set; } // El nombre real del usuario.
        [Required]
        public string Email { get; set; } // El correo electrónico del usuario.
    }
}
