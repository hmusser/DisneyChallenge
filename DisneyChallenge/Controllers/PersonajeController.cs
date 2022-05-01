using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;
using DisneyChallenge.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DisneyChallenge.Controllers
{
    [ApiController]
    [Route("api/characters")]
    //(Punto 2)
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public class PersonajeController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";

        //Constructor
        public PersonajeController(ApplicationDbContext context,
                                    IMapper mapper,
                                IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        //(Punto 3)
        [HttpGet]
        public async Task<ActionResult<List<PersonajesListadoDTO>>> ListadoPersonajes()
        {
            var entidades = await context.Personajes.ToListAsync();
            var dtos = mapper.Map<List<PersonajesListadoDTO>>(entidades);
            return dtos;
        }

        //(Punto 4)
        //Create
        [HttpPost]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen del Personaje a través de form-data desde Postman.
        public async Task<ActionResult> Post([FromForm] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var entidad = mapper.Map<Personaje>(personajeCreacionDTO);

            if (personajeCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personajeCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(personajeCreacionDTO.Imagen.FileName);
                    entidad.Imagen = await almacenadorArchivos.GuardarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              personajeCreacionDTO.Imagen.ContentType);
                }

            }
            context.Add(entidad);
            await context.SaveChangesAsync();

            var personajeDTO = mapper.Map<PersonajeDTO>(entidad);
            return new CreatedAtRouteResult("obtenerPersonaje", new { id = entidad.Id }, personajeDTO);

        }

        //Update
        [HttpPut("{id:int}")]
        //Utilizamos [FromForm] para poder recibir el archivo de imagen del Personaje a través de form-data desde Postman.
        public async Task<ActionResult> Put(int id, [FromForm] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var personajeDB = await context.Personajes
                .Include(x => x.PeliculasPersonajes) //Incluyo las películas donde aparece el personaje.
                .FirstOrDefaultAsync(x => x.Id == id);

            if (personajeDB == null)
            {
                return NotFound();//404
            }

            //Lo siguiente lo que hará es mapear lo que traigo de personajeCreacionDTO en personajeDB.
            //es decir, los campos que son distintos van a ser actualizados.
            personajeDB = mapper.Map(personajeCreacionDTO, personajeDB);

            if (personajeCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personajeCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();//arreglo de bytes.
                    var extension = Path.GetExtension(personajeCreacionDTO.Imagen.FileName);
                    personajeDB.Imagen = await almacenadorArchivos.EditarArchivo(contenido,
                                                                              extension,
                                                                              contenedor,
                                                                              personajeDB.Imagen,
                                                                              personajeCreacionDTO.Imagen.ContentType);
                }

            }
            /*SaveChangesAsync() por la forma en que funciona EF solo guardará en la BD aquellos campos
            que sean diferentes entre personajeCreacionDTO y personajeDB.*/
            await context.SaveChangesAsync();
            return NoContent();
        }

        //Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Personajes.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Personaje() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        
        //(Punto 5)
        [HttpGet("detalle")] //api/characters/detalle?id=...
        public async Task<ActionResult<PersonajeDTO>> GetCharacterDetail(int id)
        {
            var personaje = await context.Personajes
                .Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (personaje == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PersonajeDTO>(personaje);
            return dto;
        }

        //Read con filtro (Punto 6)
        [HttpGet("filtro")] //api/characters/filtro?...    
        public async Task<ActionResult<List<PersonajeDTO>>> Filtrar([FromQuery] FiltroPersonajesDTO filtroPersonajesDTO)
        {
            //Aplicamos el filtro paso por paso
            var personajesQueryable = context.Personajes.AsQueryable().
                Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula);
            //Nombre del personaje
            if (!string.IsNullOrEmpty(filtroPersonajesDTO.Nombre))
            {
                personajesQueryable = personajesQueryable.Where(x => x.Nombre.Contains(filtroPersonajesDTO.Nombre)).
                Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula);
            }

            //Edad del personaje
            if (filtroPersonajesDTO.Edad != 0)
            {
                personajesQueryable = personajesQueryable.Where(x => x.Edad == filtroPersonajesDTO.Edad).
                Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula);
            }

            //Peso del personaje
            if (filtroPersonajesDTO.Peso != 0)
            {
                personajesQueryable = personajesQueryable.Where(x => x.Peso == filtroPersonajesDTO.Peso).
                Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula);
            }

            //Id de pelicula
            if (filtroPersonajesDTO.IdPelicula != 0)
            {
                personajesQueryable = personajesQueryable.Where(x => x.PeliculasPersonajes
                .Select(y => y.PeliculaId)
                .Contains(filtroPersonajesDTO.IdPelicula)).
                Include(personajeBD => personajeBD.PeliculasPersonajes)
                .ThenInclude(peliculasPersonajeDB => peliculasPersonajeDB.Pelicula);
            }
            var personajesDB = await personajesQueryable.ToListAsync();
            var personajes = mapper.Map<List<PersonajeDTO>>(personajesDB);
            return personajes;
        }


        //Read
        [HttpGet("{id:int}", Name = "obtenerPersonaje")]
        public async Task<ActionResult<PersonajeDTO>> GetById(int id)
        {
            var entidad = await context.Personajes.FirstOrDefaultAsync(x => x.Id == id);
         
            if(entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PersonajeDTO>(entidad);
            return dto;
        }    

}
}
