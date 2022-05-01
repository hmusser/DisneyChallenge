using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.DTOs
{
    public class PeliculaBusquedaDTO
    {
        public string Imagen { get; set; } //En este campo se guarda la url de la imagen.

        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1, 5)]
        public int Calificacion { get; set; }

        //Propiedades de navegacion
        public List<GeneroListadoDTO> Generos { get; set; }
    }
}
