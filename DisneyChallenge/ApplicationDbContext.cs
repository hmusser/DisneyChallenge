using DisneyChallenge.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DisneyChallenge
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Personaje> Personajes { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
    }
}
