namespace API_Proyecto.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public List<LocalDTO> Locales { get; set; }
        public List<ReservaDTO> Reservas { get; set; }
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
