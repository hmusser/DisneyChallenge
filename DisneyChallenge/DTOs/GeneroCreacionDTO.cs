﻿using DisneyChallenge.Helpers;
using DisneyChallenge.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.DTOs
{
    public class GeneroCreacionDTO
    {
        public string Nombre { get; set; }
        [PesoArchivoValidacion(PesoMaximoEnMegabytes: 5)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Imagen { get; set; } //En este campo se recibe el archivo de la imagen.
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> PeliculasIds { get; set; }
    }
}
