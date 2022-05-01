namespace DisneyChallenge.DTOs
{
    public class PersonajeDTO
    {
        public string Imagen { get; set; } //En este campo se le envia al cliente la url de la imagen.
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public double Peso { get; set; }
        public string Historia { get; set; }
        public List<PeliculaDTO> Peliculas { get; set; }
    }
}
