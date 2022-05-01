namespace DisneyChallenge.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; } //En este campo se guarda la url de la imagen.

        //Propiedad de navegación.
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
