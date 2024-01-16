using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<GeneroDTO>> GetById(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(g => g.Id == id);

            if (genero == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<GeneroDTO>(genero));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<GeneroDTO>>> GetAll()
        {
            var generos = await context.Generos.ToListAsync();

            if (!generos.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<GeneroDTO>>(generos));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(GeneroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeGenero = await context.Generos.AnyAsync(g => g.NombreGenero == dto.NombreGenero);

            if (existeGenero)
            {
                return Conflict();
            }

            var genero = mapper.Map<Genero>(dto);
            context.Add(genero);

            try
            {
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", details = ex.Message });
            }
        }


        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var genero = await context.Generos.FindAsync(id);

            if (genero == null)
            {
                return NotFound();
            }

            context.Generos.Remove(genero);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, GeneroDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var genero = await context.Generos.FindAsync(id);

            if (genero == null)
            {
                return NotFound();
            }

            mapper.Map(dto, genero);
            context.Update(genero);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeneroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool GeneroExists(int id)
        {
            return context.Generos.Any(g => g.Id == id);
        }
    }
}
