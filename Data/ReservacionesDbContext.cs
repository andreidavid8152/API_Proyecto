using API_Proyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Proyecto.Data
{
    public class ReservacionesDbContext : DbContext
    {

        public ReservacionesDbContext(DbContextOptions<ReservacionesDbContext> options) : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }

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

        }

    }
}
