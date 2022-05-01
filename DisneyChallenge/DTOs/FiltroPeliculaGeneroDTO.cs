namespace DisneyChallenge.DTOs
{
    public class FiltroPeliculaDTO
    {
        public string NombrePelicula { get; set; }
        public int IdGenero { get; set; }

        //Para manejar el order by
        public string CampoAOrdenar { get; set; }
        public string Order { get; set; } = "ASC";
    }
}
