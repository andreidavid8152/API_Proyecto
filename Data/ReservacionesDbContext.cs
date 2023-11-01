using API_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Proyecto.Data
{
    public class ReservacionesDbContext : DbContext
    {

        public ReservacionesDbContext(DbContextOptions<ReservacionesDbContext> options) : base(options)
        { }

        //Tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Local> Locales { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ImagenLocal> ImagenesLocal { get; set; }
        public DbSet<Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "admin",
                    Password = "2003",
                    Nombre = "Andrei",
                    Email = "andrei@gmail.com"
                }
            );

            modelBuilder.Entity<Local>().HasData(
                new Local
                {
                    ID = 1,  // Puedes elegir un número de ID que no esté en uso
                    PropietarioID = 1,  // Puedes usar el ID del usuario 'admin' que ya está definido
                    Nombre = "Mi Local",  // Elige el nombre que quieras
                    Descripcion = "Una descripción para mi local",  // Añade una descripción
                    Direccion = "Calle Falsa 123",  // Añade una dirección
                    Capacidad = 50  // Añade la capacidad que desees
                }
            );

            // Relación entre Comentario y Usuario
            modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)  // Aquí especificas la relación inversa
            .HasForeignKey(c => c.UsuarioID)
            .OnDelete(DeleteBehavior.Cascade);

            // Relación entre Local y Usuario
            modelBuilder.Entity<Local>()
            .HasOne(l => l.Propietario)
            .WithMany(u => u.Locales)  // Aquí especificas la relación inversa
            .HasForeignKey(l => l.PropietarioID)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Reserva y Usuario
            modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.Reservas)
            .HasForeignKey(r => r.UsuarioID)
            .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
