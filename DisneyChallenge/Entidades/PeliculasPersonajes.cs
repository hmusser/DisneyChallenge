namespace DisneyChallenge.Entidades
{
    //Entidad de asociación entre Peliculas y Personajes.
    public class PeliculasPersonajes
    {
        public int PeliculaId { get; set; }
        public int PersonajeId { get; set; }

        //Propiedades de navegacion
        public Pelicula Pelicula { get; set; }
        public Personaje Personaje { get; set; }
        
    }
}
