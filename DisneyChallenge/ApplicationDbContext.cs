using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DisneyChallenge.Entidades;

namespace DisneyChallenge
{
    //(Punto 1)
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //Llaves primarias compuestas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PeliculasPersonajes>()
            .HasKey(x => new { x.PersonajeId, x.PeliculaId });

            modelBuilder.Entity<PeliculasGeneros>()
            .HasKey(x => new { x.GeneroId, x.PeliculaId });

            SeedData(modelBuilder);

        }
        //Se cargan algunos datos de prueba mínimos en la BD.
        private void SeedData(ModelBuilder modelBuilder)
        {
            //Creamos un rol Admin por defecto.
            var rolAdminId = "f1f61f1b-5722-4ef5-9783-d4913577eb3e";
            var usuarioAdminId = "69f512cb-b8f4-4baa-b8d5-849375a6a59a";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var username = "musserhoracio@gmail.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "hH123456$")
            };
            //Agregamos el usuario
            modelBuilder.Entity<IdentityUser>().HasData(usuarioAdmin);
           // Creamos el rol
            modelBuilder.Entity<IdentityRole>().HasData(rolAdmin);
            //Asignacion del Rol al Usuario
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = usuarioAdminId,
                    ClaimValue = "Admin"
                });

            //Generos
            var drama = new Genero() { Id = 1, Nombre = "Drama", Imagen = "https://definicion.de/wp-content/uploads/2009/04/drama.png" };//Marley y yo
            var animacion = new Genero() { Id = 2, Nombre = "Animación", Imagen = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR6RyzavxY4KZjVvtEmroKRAXKTOUdBq8Ewgg&usqp=CAU" };//El Rey Leon
            var fantasia = new Genero() { Id = 3, Nombre = "Fantasía", Imagen = "https://t1.uc.ltmcdn.com/es/posts/1/2/6/como_escribir_un_libro_de_fantasia_toma_nota_47621_600_square.jpg" };//Avatar
            var musical = new Genero() { Id = 4, Nombre = "Musical", Imagen = "https://www.elindependiente.com/wp-content/uploads/2022/03/fama-musical-980x550.jpg" };//High School Musical
            var comedia = new Genero() { Id = 5, Nombre = "Comedia", Imagen = "https://static6.depositphotos.com/1000975/567/i/600/depositphotos_5670602-stock-photo-actor-with-maks-in-a.jpg" };//Luca
            modelBuilder.Entity<Genero>()
                .HasData(new List<Genero>
                {
                    drama, animacion, fantasia, musical, comedia
                });

            //Personajes
            var simba = new Personaje() { Id = 1, Edad = 25, Historia = "Historia de Simba", Nombre = "Simba", Peso = 120, Imagen = "https://static.wikia.nocookie.net/videojuego/images/8/8e/Simba_adulto.jpg/revision/latest/top-crop/width/360/height/450?cb=20130814122207" };
            var zazu = new Personaje() { Id = 2, Edad = 12, Historia = "Historia de zazu", Nombre = "Zazu", Peso = 3, Imagen = "https://sm.ign.com/t/ign_latam/news/j/john-olive/john-oliver-to-reportedly-play-zazu-in-the-lion-king_rp97.h720.jpg" };
            var owenWilson = new Personaje() { Id = 3, Edad = 53, Historia = "Historia del personaje de Owen Wilson", Nombre = "John Grogan", Peso = 80, Imagen = "https://upload.wikimedia.org/wikipedia/commons/b/b7/Owen_Wilson_Cannes_2011_%28cropped%29.jpg" };
            var jenniferAniston = new Personaje() { Id = 4, Edad = 53, Historia = "Historia del personaje de Jennifer Aniston", Nombre = "Jenny Grogan", Peso = 65, Imagen = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/16/JenniferAnistonHWoFFeb2012.jpg/640px-JenniferAnistonHWoFFeb2012.jpg" };
            var samWorthington = new Personaje() { Id = 5, Edad = 49, Historia = "Historia del personaje de Sam Worthington", Nombre = "Jake Sully", Peso = 80, Imagen = "https://m.media-amazon.com/images/M/MV5BMTc5NTMyMjIwMV5BMl5BanBnXkFtZTcwNTMyNjYwMw@@._V1_UY317_CR6,0,214,317_AL_.jpg" };
            var sigourneyWeaver = new Personaje() { Id = 6, Edad = 68, Historia = "Historia del personaje de Sigourney Weaver", Nombre = "Grace Augustine", Peso = 70, Imagen = "https://m.media-amazon.com/images/M/MV5BMTk1MTcyNTE3OV5BMl5BanBnXkFtZTcwMTA0MTMyMw@@._V1_UY317_CR12,0,214,317_AL_.jpg" };
            var zacEfron = new Personaje() { Id = 7, Edad = 35, Historia = "Historia del personaje de Zac Efron", Nombre = "Troy Bolton", Peso = 75, Imagen = "https://m.media-amazon.com/images/M/MV5BMTUxNzY3NzYwOV5BMl5BanBnXkFtZTgwNzQ3Mzc4MTI@._V1_UX214_CR0,0,214,317_AL_.jpg" };
            var vanessaHudgens = new Personaje() { Id = 8, Edad = 27, Historia = "Historia del personaje de Vanessa Hudgens", Nombre = "Gabriela Montez", Peso = 64, Imagen = "https://m.media-amazon.com/images/M/MV5BZGY4NGU0NjgtNjc0Mi00OTk3LWFmMzktNjY4M2JlMDkzOTFkXkEyXkFqcGdeQXVyMTExNzQ3MzAw._V1_UY317_CR12,0,214,317_AL_.jpg" };
            var jacobTremblay = new Personaje() { Id = 9, Edad = 19, Historia = "Historia del personaje de Jacob Tremblay", Nombre = "Luca Paguro", Peso = 65, Imagen = "https://m.media-amazon.com/images/M/MV5BOGNkMjdjOGYtYTRmZS00N2RlLTkyOWUtNDlmNjA3NTkxYjhkXkEyXkFqcGdeQXVyMzM0MDI2MTE@._V1_UX214_CR0,0,214,317_AL_.jpg" };
            var emmaBerman = new Personaje() { Id = 10, Edad = 21, Historia = "Historia del personaje de Emma Berman", Nombre = "Giulia Marcovaldo", Peso = 60, Imagen = "https://m.media-amazon.com/images/M/MV5BMTk3NmI4OGItNTA2ZC00NDhjLTkzMWEtNmU4NzhhZGE5OWRhXkEyXkFqcGdeQXVyMTAzNzYxMjUz._V1_UX214_CR0,0,214,317_AL_.jpg" };
            modelBuilder.Entity<Personaje>()
                .HasData(new List<Personaje>
                {
                    simba, zazu, owenWilson, jenniferAniston, samWorthington, sigourneyWeaver, zacEfron, vanessaHudgens, jacobTremblay, emmaBerman
                });

            //Peliculas
            var elReyLeon = new Pelicula() { Id = 1, Calificacion = 4, FechaCreacion = new DateTime(1994, 01, 01), Titulo = "The Lion King", Imagen = "https://static.wikia.nocookie.net/disneyypixar/images/b/b3/The_Lion_King.jpg/revision/latest/scale-to-width-down/1200?cb=20210531223233&path-prefix=es" };
            var marleyYYo = new Pelicula() { Id = 2, Calificacion = 2, FechaCreacion = new DateTime(2008, 01, 01), Titulo = "Marley y yo", Imagen = "https://pics.filmaffinity.com/Marley_y_yo-606232966-large.jpg" };
            var avatar = new Pelicula() { Id = 3, Calificacion = 5, FechaCreacion = new DateTime(2009, 01, 01), Titulo = "Avatar", Imagen = "https://es.web.img3.acsta.net/medias/nmedia/18/92/13/82/20182449.jpg" };
            var hcm = new Pelicula() { Id = 4, Calificacion = 2, FechaCreacion = new DateTime(2006, 01, 01), Titulo = "High School Musical", Imagen = "https://static.wikia.nocookie.net/doblaje/images/1/12/High_school_musical_xlg.jpg/revision/latest/top-crop/width/360/height/450?cb=20200826175615&path-prefix=es" };
            var luca = new Pelicula() { Id = 5, Calificacion = 3, FechaCreacion = new DateTime(2021, 01, 01), Titulo = "Luca", Imagen = "https://pics.filmaffinity.com/Luca-907827591-large.jpg" };
            modelBuilder.Entity<Pelicula>()
               .HasData(new List<Pelicula>
               {
                    elReyLeon, marleyYYo, avatar, hcm, luca
               });

            modelBuilder.Entity<PeliculasGeneros>().HasData(
                new List<PeliculasGeneros>()
                {
                    new PeliculasGeneros(){PeliculaId = elReyLeon.Id, GeneroId = animacion.Id},
                    new PeliculasGeneros(){PeliculaId = marleyYYo.Id, GeneroId = drama.Id},
                    new PeliculasGeneros(){PeliculaId = avatar.Id, GeneroId = fantasia.Id},
                    new PeliculasGeneros(){PeliculaId = hcm.Id, GeneroId = musical.Id},
                    new PeliculasGeneros(){PeliculaId = luca.Id, GeneroId = comedia.Id},
                      });

            modelBuilder.Entity<PeliculasPersonajes>().HasData(
                new List<PeliculasPersonajes>()
                {
                    new PeliculasPersonajes(){PeliculaId = elReyLeon.Id, PersonajeId = simba.Id},
                    new PeliculasPersonajes(){PeliculaId = avatar.Id, PersonajeId = simba.Id},
                    new PeliculasPersonajes(){PeliculaId = elReyLeon.Id, PersonajeId = zazu.Id},
                    new PeliculasPersonajes(){PeliculaId = marleyYYo.Id, PersonajeId = owenWilson.Id},
                    new PeliculasPersonajes(){PeliculaId = marleyYYo.Id, PersonajeId = jenniferAniston.Id},
                    new PeliculasPersonajes(){PeliculaId = avatar.Id, PersonajeId = samWorthington.Id},
                    new PeliculasPersonajes(){PeliculaId = avatar.Id, PersonajeId = sigourneyWeaver.Id},
                    new PeliculasPersonajes(){PeliculaId = hcm.Id, PersonajeId = zacEfron.Id},
                    new PeliculasPersonajes(){PeliculaId = hcm.Id, PersonajeId = vanessaHudgens.Id},
                    new PeliculasPersonajes(){PeliculaId = luca.Id, PersonajeId = jacobTremblay.Id},
                    new PeliculasPersonajes(){PeliculaId = luca.Id, PersonajeId = emmaBerman.Id},
                    });
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Personaje> Personajes { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        //Asociativas
        public DbSet<PeliculasPersonajes> PeliculasPersonajes { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
