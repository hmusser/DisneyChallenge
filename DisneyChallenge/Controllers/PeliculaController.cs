using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;
using DisneyChallenge.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq.Dynamic.Core;
namespace DisneyChallenge.Controllers
{
    [ApiController]
    [Route("api/movies")]
    //(Punto 2)
     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeliculaController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";
        public PeliculaController(ApplicationDbContext context,
                                    IMapper mapper,              
                                    IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        //(Punto 7)
        //Read
        [HttpGet]
        public async Task<ActionResult<List<PeliculaListadoDTO>>> ListadoPeliculas()
        {
            var entidades = await context.Peliculas.ToListAsync();
            var dtos = mapper.Map<List<PeliculaListadoDTO>>(entidades);
            return dtos;
        }

        //(Punto 8)
        [HttpGet("detalle")] //api/movies/detalle?id=...
        public async Task<ActionResult<PeliculaDTO>> GetMovieDetails(int id)
        {
            var pelicula = await context.Peliculas
                .Include(peliculaBD => peliculaBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Personaje)
                .Include(peliculaDB => peliculaDB.PeliculasGeneros)
                .ThenInclude(peliculasGenerosDB => peliculasGenerosDB.Genero)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PeliculaDTO>(pelicula);
            return dto;
        }

        //(Punto 9)
        //Create
        [HttpPost]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen de la Película a través de form-data desde Postman.
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var entidad = mapper.Map<Pelicula>(peliculaCreacionDTO);
            if (peliculaCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(peliculaCreacionDTO.Imagen.FileName);
                    entidad.Imagen = await almacenadorArchivos.GuardarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              peliculaCreacionDTO.Imagen.ContentType);
                }

            }
            context.Add(entidad);
            await context.SaveChangesAsync();

            var peliculaDTO = mapper.Map<PeliculaDTO>(entidad);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = entidad.Id }, peliculaDTO);
        }
        //Update
        [HttpPut("{id:int}")]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen de la Pelicula a través de form-data desde Postman.
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            //Busco la pelicula a actualizar.
            var peliculaDB = await context.Peliculas
                .Include(x => x.PeliculasPersonajes) //Incluyo los personajes de la pelicula.
                .Include(x => x.PeliculasGeneros) //Incluyo los géneros de las pelicula.
                .FirstOrDefaultAsync(x => x.Id == id);
            //Si no la encuentro
            if (peliculaDB == null)
            {
                return NotFound();//404
            }

            //Lo siguiente lo que hará es mapear lo que traigo de peliculaCreacionDTO en peliculaDB.
            //es decir, los campos que son distintos van a ser actualizados.
            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(peliculaCreacionDTO.Imagen.FileName);
                    peliculaDB.Imagen = await almacenadorArchivos.EditarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              peliculaDB.Imagen,
                                                                              peliculaCreacionDTO.Imagen.ContentType);
                }

            }
            /*SaveChangesAsync() por la forma en que funciona EF solo guardará en la BD aquellos campos
            que sean diferentes entre generoCreacionDTO y peliculaDB.*/
            await context.SaveChangesAsync();
            return NoContent();
        }
        //Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Peliculas.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        //(Punto 10)
        //Read con filtro (Punto 6)
        [HttpGet("filtro")] //api/movies/filtro?...    
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculaDTO filtroPeliculaDTO)
        {
            //Aplicamos el filtro paso por paso
            var peliculasQueryable = context.Peliculas.AsQueryable().
                Include(peliculaBD => peliculaBD.PeliculasGeneros)
                .ThenInclude(peliculasGenerosDB => peliculasGenerosDB.Genero)
                .Include(peliculaBD => peliculaBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajes => peliculasPersonajes.Personaje);

            //Nombre de la pelicula
            if (!string.IsNullOrEmpty(filtroPeliculaDTO.NombrePelicula))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculaDTO.NombrePelicula)).
                Include(peliculaBD => peliculaBD.PeliculasGeneros)
                .ThenInclude(peliculasGenerosDB => peliculasGenerosDB.Genero)
                .Include(peliculaBD => peliculaBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajes => peliculasPersonajes.Personaje);
            }

            //Id de género
            if (filtroPeliculaDTO.IdGenero != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                .Contains(filtroPeliculaDTO.IdGenero)).
                Include(peliculaBD => peliculaBD.PeliculasGeneros)
                .ThenInclude(peliculasGenerosDB => peliculasGenerosDB.Genero)
                .Include(peliculaBD => peliculaBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajes => peliculasPersonajes.Personaje);
            }

            //Ordenar?
            if(!string.IsNullOrEmpty(filtroPeliculaDTO.CampoAOrdenar))
            {
                var tipoOrden = (filtroPeliculaDTO.Order).ToUpper().Equals("ASC") ? "ascending" : "descending";
                peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculaDTO.CampoAOrdenar} {tipoOrden}").
                Include(peliculaBD => peliculaBD.PeliculasGeneros)
                .ThenInclude(peliculasGenerosDB => peliculasGenerosDB.Genero)
                .Include(peliculaBD => peliculaBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajes => peliculasPersonajes.Personaje);
            }

            var peliculasDB = await peliculasQueryable.ToListAsync();
            var peliculas = mapper.Map<List<PeliculaDTO>>(peliculasDB);
            return peliculas;
        }

        //Read
        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var entidad = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PeliculaDTO>(entidad);
            return dto;
        }
        
       
    }
}
