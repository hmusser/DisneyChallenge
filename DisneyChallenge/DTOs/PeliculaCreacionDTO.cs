using DisneyChallenge.Helpers;
using DisneyChallenge.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.DTOs
{
    public class PeliculaCreacionDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMegabytes: 5)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Imagen { get; set; } //En este campo se recibe el archivo de la imagen.

        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1,5)]
        [ModelBinder(BinderType = typeof(TypeBinder<int>))]
        public int Calificacion { get; set; }

        /*Como en el Post de una Pelicula estamos usando FromForm para recibir los parámetros
        el ModelBinder no está tomando correctamente como enteros que el cliente nos envía,
        por lo cual debemos implementar un ModelBinder propio para solucionar esto que se llamará
        TypeBinder (está en Helpers)*
        */
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> PersonajesIds { get; set; }

    }
}
