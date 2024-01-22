using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/comunidades")]
    [ApiController]
    public class ComunidadesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComunidadesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<ComunidadDTO>> GetById(int id)
        {
            var comunidad = await context.Comunidades
                .Include(e => e.Municipio)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (comunidad == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<ComunidadDTO>(comunidad));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<ComunidadDTO>>> GetAll()
        {
            var comunidad = await context.Comunidades
                .Include(e => e.Municipio)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!comunidad.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<ComunidadDTO>>(comunidad));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(ComunidadDTO dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeo del DTO a la entidad
            var comunidad = mapper.Map<Comunidad>(dto);
            comunidad.Municipio = await context.Municipios.SingleOrDefaultAsync(r => r.Id == dto.Municipio.Id);

            // Incluir la entidad en el contexto
            context.Add(comunidad);

            try
            {
                // Guardar cambios en la base de datos dentro de una transacción
                await context.SaveChangesAsync();
                return Ok();
                // return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Manejar errores de base de datos
                return StatusCode(500);
            }
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var comunidad = await context.Comunidades.FindAsync(id);

            if (comunidad == null)
            {
                return NotFound();
            }

            context.Comunidades.Remove(comunidad);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ComunidadDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var comunidad = await context.Comunidades.FindAsync(id);

            if (comunidad == null)
            {
                return NotFound();
            }

            // Mapea los datos del DTO al usuario existente
            mapper.Map(dto, comunidad);
            comunidad.Municipio = await context.Municipios.SingleOrDefaultAsync(r => r.Id == dto.Municipio.Id);

            context.Update(comunidad);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

    }
}

