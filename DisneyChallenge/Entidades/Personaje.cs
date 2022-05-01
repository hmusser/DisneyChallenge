namespace DisneyChallenge.Entidades
{
    public class Personaje
    {
        public int Id { get; set; }
        public string Imagen { get; set; } //En este campo se guarda la url de la imagen.
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; }
        //Propiedad de navegación.
        public List<PeliculasPersonajes> PeliculasPersonajes { get; set; }
    }
}
