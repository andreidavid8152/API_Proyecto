namespace API_Proyecto.DTOs
{
    public class ReservaDTO
    {
        public int ID { get; set; }
        public int LocalID { get; set; }
        public int UsuarioID { get; set; }
        public int HorarioID { get; set; } //anadida
        public DateTime Fecha { get; set; }

        public LocalDTO Local { get; set; } //anadida
        public HorarioDTO Horario { get; set; } //anadida
    }
}
