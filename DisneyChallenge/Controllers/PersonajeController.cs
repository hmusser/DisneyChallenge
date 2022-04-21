using AutoMapper;
using DisneyChallenge.DTOs;
using DisneyChallenge.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisneyChallenge.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class PersonajeController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PersonajeController(ApplicationDbContext context,
                                    IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonajeDTO>>> Get()
        {
            var entidades = await context.Personajes.ToListAsync();
            var dtos = mapper.Map<List<PersonajeDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerPersonaje")]
        public async Task<ActionResult<PersonajeDTO>> Get(int id)
        {
            var entidad = await context.Personajes.FirstOrDefaultAsync(x => x.Id == id);
         
            if(entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<PersonajeDTO>(entidad);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var entidad = mapper.Map<Personaje>(personajeCreacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var personajeDTO = mapper.Map<PersonajeDTO>(entidad);
            return new CreatedAtRouteResult("obtenerPersonaje", new { id = entidad.Id }, personajeDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var entidad = mapper.Map<Personaje>(personajeCreacionDTO);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;
            context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult>Delete(int id)
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




    }
}
