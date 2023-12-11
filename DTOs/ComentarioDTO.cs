using API_Proyecto.Models;

namespace API_Proyecto.DTOs
{
    public class ComentarioDTO
    {
        public int ID { get; set; }
        public int LocalID { get; set; }
        public int UsuarioID { get; set; }
        public string Texto { get; set; }
        public DateTime Fecha { get; set; }
        public int Calificacion { get; set; }
        public LocalDTO Local { get; set; }
    }
}
