﻿namespace DisneyChallenge.DTOs
{
    public class GeneroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; } //En este campo se le envia al cliente la url de la imagen.
    }
}
