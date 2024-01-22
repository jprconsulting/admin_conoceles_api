using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/candidatos")]
    [ApiController]
    public class CandidatosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CandidatosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<CandidatoDTO>> GetById(int id)
        {
            var candidato = await context.Candidatos
                .Include(g => g.Genero)
                .Include(e => e.Estado)
                .Include(c => c.Cargo)
                .Include(o => o.AgrupacionPolitica)
                    .ThenInclude(t => t.TipoAgrupacionPolitica)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidato == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CandidatoDTO>(candidato));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<CandidatoDTO>>> GetAll()
        {
            var candidatos = await context.Candidatos
              .Include(g => g.Genero)
              .Include(e => e.Estado)
              .Include(c => c.Cargo)
              .Include(o => o.AgrupacionPolitica)
                  .ThenInclude(t => t.TipoAgrupacionPolitica)
              .ToListAsync();

            if (!candidatos.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<CandidatoDTO>>(candidatos));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(CandidatoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeCandidato = await context.Candidatos.AnyAsync(c => c.Nombres == dto.Nombres 
                && c.ApellidoPaterno == dto.ApellidoPaterno
                && c.ApellidoMaterno == dto.ApellidoMaterno);

            if (existeCandidato)
            {
                return Conflict();
            }

            var candidato = mapper.Map<Candidato>(dto);
            candidato.Genero = await context.Generos.SingleOrDefaultAsync(g => g.Id == dto.Genero.Id);
            candidato.Estado = await context.Estados.SingleOrDefaultAsync(e => e.Id == dto.Estado.Id);
            candidato.Cargo = await context.Cargos.SingleOrDefaultAsync(c => c.Id == dto.Cargo.Id);
            candidato.AgrupacionPolitica = await context.AgrupacionesPoliticas
                .SingleOrDefaultAsync(o => o.Id == dto.AgrupacionPolitica.Id);

            context.Add(candidato);

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
            var candidato = await context.Candidatos.FindAsync(id);

            if (candidato == null)
            {
                return NotFound();
            }

            context.Candidatos.Remove(candidato);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, CandidatoDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var candidato = await context.Candidatos.FindAsync(id);

            if (candidato == null)
            {
                return NotFound();
            }

            mapper.Map(dto, candidato);
            candidato.Genero = await context.Generos.SingleOrDefaultAsync(g => g.Id == dto.Genero.Id);
            candidato.Estado = await context.Estados.SingleOrDefaultAsync(e => e.Id == dto.Estado.Id);
            candidato.Cargo = await context.Cargos.SingleOrDefaultAsync(c => c.Id == dto.Cargo.Id);
            candidato.AgrupacionPolitica = await context.AgrupacionesPoliticas
                .SingleOrDefaultAsync(o => o.Id == dto.AgrupacionPolitica.Id);

            context.Update(candidato);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidatoExists(id))
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

        private bool CandidatoExists(int id)
        {
            return context.Candidatos.Any(c => c.Id == id);
        }
    }
}
