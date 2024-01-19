using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/distritos")]
    [ApiController]
    public class DistritosLocalesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public DistritosLocalesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<DistritoLocalDTO>> GetById(int id)
        {
            var usuario = await context.DistritosLocales
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<DistritoLocalDTO>(usuario));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<DistritoLocalDTO>>> GetAll()
        {
            var distritos = await context.DistritosLocales
                .Include(e => e.Estado)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!distritos.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<DistritoLocalDTO>>(distritos));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(DistritoLocalDTO dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeo del DTO a la entidad
            var distrito = mapper.Map<DistritoLocal>(dto);
            distrito.Estado = await context.Estados.SingleOrDefaultAsync(r => r.Id == dto.Estado.Id);

            // Incluir la entidad en el contexto
            context.Add(distrito);

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
            var distrito = await context.DistritosLocales.FindAsync(id);

            if (distrito == null)
            {
                return NotFound();
            }

            context.DistritosLocales.Remove(distrito);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] DistritoLocalDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var distrito = await context.DistritosLocales.FindAsync(id);

            if (distrito == null)
            {
                return NotFound();
            }

            // Mapea los datos del DTO al usuario existente
            mapper.Map(dto, distrito);
            distrito.Estado = await context.Estados.SingleOrDefaultAsync(r => r.Id == dto.Estado.Id);

            context.Update(distrito);

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
