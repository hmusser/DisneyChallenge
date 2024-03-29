﻿using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Imagen { get; set; } //En este campo se guarda la url de la imagen.

        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1, 5)]
        public int Calificacion { get; set; }

        //Propiedades de navegación.
        public List<PeliculasPersonajes> PeliculasPersonajes { get; set; }
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }

    }
}
