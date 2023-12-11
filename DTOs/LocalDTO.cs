namespace API_Proyecto.DTOs
{
    public class LocalDTO
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public int Capacidad { get; set; }
        public List<HorarioDTO> Horarios { get; set; }
        public List<ReservaDTO> Reservas { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
        public List<ImagenLocalDTO> Imagenes { get; set; }    
    }
}
