namespace DisneyChallenge.Entidades
{
    //Entidad de asociación entre Peliculas y Géneros.
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }

        //Propiedades de navegación.
        public Pelicula Pelicula { get; set; }
        public Genero Genero { get; set; }

       
    }
}
