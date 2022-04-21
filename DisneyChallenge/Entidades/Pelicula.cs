using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Imagen { get; set; }

        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1, 5)]
        public int Calificacion { get; set; }
        
    }
}
