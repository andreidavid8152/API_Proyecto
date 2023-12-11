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
            // Datos iniciales para Usuario
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "admin",
                    Password = "2003", // Asumiendo que esto es una contraseña hash en un caso real
                    Nombre = "administrador",
                    Email = "admin@gmail.com"
                },
                // Agregar un nuevo usuario
                new Usuario
                {
                    Id = 2, // Asegúrate de que el ID sea único
                    Username = "andrei",
                    Password = "2003", // Asegúrate de almacenar contraseñas de forma segura (hash + salt)
                    Nombre = "andrei",
                    Email = "andrei@gmail.com"
                }
             // ... puedes agregar más usuarios si es necesario
             );

            // Datos iniciales para Local
            modelBuilder.Entity<Local>().HasData(
                new Local
                {
                    ID = 1,
                    PropietarioID = 1,
                    Nombre = "Café Central",
                    Descripcion = "Cafetería con ambiente acogedor y música en vivo.",
                    Direccion = "Avenida Siempre Viva 742",
                    Capacidad = 50
                },
                new Local
                {
                    ID = 2,
                    PropietarioID = 2,
                    Nombre = "Librería Letras",
                    Descripcion = "Espacio cultural con selección de libros de autores independientes.",
                    Direccion = "Calle Literaria 101",
                    Capacidad = 20
                }
            );

            // Datos iniciales para Horario - Asegúrate de que los IDs coincidan con los locales
            modelBuilder.Entity<Horario>().HasData(
                // Horarios para el local con ID 1
                new Horario
                {
                    ID = 1,
                    LocalID = 1,
                    HoraInicio = new TimeSpan(8, 0, 0), // 8:00 AM
                    HoraFin = new TimeSpan(12, 0, 0)    // 12:00 PM
                },
                new Horario
                {
                    ID = 2,
                    LocalID = 1,
                    HoraInicio = new TimeSpan(14, 0, 0), // 14:00 PM
                    HoraFin = new TimeSpan(16, 0, 0)    // 4:00 PM
                },
                new Horario
                {
                    ID = 3,
                    LocalID = 1,
                    HoraInicio = new TimeSpan(18, 0, 0), // 6:00 PM
                    HoraFin = new TimeSpan(20, 0, 0)    // 8:00 PM
                },
                new Horario
                {
                    ID = 4,
                    LocalID = 1,
                    HoraInicio = new TimeSpan(22, 0, 0), // 22:00 PM
                    HoraFin = new TimeSpan(23, 59, 0)   // 11:59 PM
                },
                // Horarios para el local con ID 2
                new Horario
                {
                    ID = 5,
                    LocalID = 2,
                    HoraInicio = new TimeSpan(8, 0, 0), // 8:00 AM
                    HoraFin = new TimeSpan(12, 0, 0)    // 12:00 PM
                },
                new Horario
                {
                    ID = 6,
                    LocalID = 2,
                    HoraInicio = new TimeSpan(14, 0, 0), // 14:00 PM
                    HoraFin = new TimeSpan(16, 0, 0)    // 4:00 PM
                },
                new Horario
                {
                    ID = 7,
                    LocalID = 2,
                    HoraInicio = new TimeSpan(18, 0, 0), // 6:00 PM
                    HoraFin = new TimeSpan(20, 0, 0)    // 8:00 PM
                },
                new Horario
                {
                    ID = 8,
                    LocalID = 2,
                    HoraInicio = new TimeSpan(22, 0, 0), // 22:00 PM
                    HoraFin = new TimeSpan(23, 59, 0)   // 11:59 PM
                }
            // Agrega más horarios si tienes más locales
            );


            // Datos iniciales para ImagenLocal - Asegúrate de que los IDs coincidan con los locales
            modelBuilder.Entity<ImagenLocal>().HasData(
                new ImagenLocal
                {
                    ID = 1,
                    LocalID = 1,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234069.jpg"
                },
                new ImagenLocal
                {
                    ID = 2,
                    LocalID = 1,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234070.jpg"
                }, new ImagenLocal
                {
                    ID = 3,
                    LocalID = 1,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234071.jpg"
                },
                new ImagenLocal
                {
                    ID = 4,
                    LocalID = 2,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170509.jpg"
                },
                new ImagenLocal
                {
                    ID = 5,
                    LocalID = 2,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170504.jpg"
                },
                new ImagenLocal
                {
                    ID = 6,
                    LocalID = 2,
                    Url = "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170505.jpg"
                }
            // ... Agrega más imágenes según sea necesario
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
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Local)
            .WithMany(l => l.Reservas)
            .HasForeignKey(r => r.LocalID)
            .OnDelete(DeleteBehavior.NoAction); // Cambia a NoAction

        }

    }
}
