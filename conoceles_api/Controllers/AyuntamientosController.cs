using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AyuntamientosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AyuntamientosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<AyuntamientoDTO>> GetById(int id)
        {
            var ayuntamiento = await context.Ayuntamientos
                .Include(e => e.DistritoLocal)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (ayuntamiento == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AyuntamientoDTO>(ayuntamiento));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<AyuntamientoDTO>>> GetAll()
        {
            var ayuntamiento = await context.Ayuntamientos
                .Include(e => e.DistritoLocal)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!ayuntamiento.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<AyuntamientoDTO>>(ayuntamiento));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(AyuntamientoDTO dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeo del DTO a la entidad
            var ayuntamiento = mapper.Map<Ayuntamiento>(dto);
            ayuntamiento.DistritoLocal = await context.DistritosLocales.SingleOrDefaultAsync(r => r.Id == dto.DistritoLocal.Id);

            // Incluir la entidad en el contexto
            context.Add(ayuntamiento);

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
                // return StatusCode(500, new { error = "Error interno del servidor.", details = ex.Message });
                return StatusCode(500);
            }
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ayuntamiento = await context.Ayuntamientos.FindAsync(id);

            if (ayuntamiento == null)
            {
                return NotFound();
            }

            context.Ayuntamientos.Remove(ayuntamiento);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] AyuntamientoDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var ayuntamiento = await context.Ayuntamientos.FindAsync(id);

            if (ayuntamiento == null)
            {
                return NotFound();
            }

            // Mapea los datos del DTO al usuario existente
            mapper.Map(dto, ayuntamiento);
            ayuntamiento.DistritoLocal = await context.DistritosLocales.SingleOrDefaultAsync(r => r.Id == dto.DistritoLocal.Id);

            context.Update(ayuntamiento);

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
