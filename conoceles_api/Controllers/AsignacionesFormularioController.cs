using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/asignaciones-formulario")]
    [ApiController]
    public class AsignacionesFormularioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AsignacionesFormularioController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<AsignacionFormularioDTO>> GetById(int id)
        {
            var asignacionFormulario = await context.AsignacionesFormulario
                .Include(g => g.Formulario)
                .Include(c => c.Candidato)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asignacionFormulario == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AsignacionFormularioDTO>(asignacionFormulario));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<AsignacionFormularioDTO>>> GetAll()
        {
            var asignacionesFormularios = await context.AsignacionesFormulario
                .Include(g => g.Formulario)
                .Include(c => c.Candidato)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!asignacionesFormularios.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<AsignacionFormularioDTO>>(asignacionesFormularios));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(AsignacionFormularioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var candidatoId in dto.CandidatosIds)
            {
                var existeAsignacion = await context.AsignacionesFormulario.AnyAsync(a => a.Candidato.Id == candidatoId 
                    && a.Formulario.Id == dto.Formulario.Id);

                if (!existeAsignacion) 
                {
                    var asignacion = mapper.Map<AsignacionFormulario>(dto);
                    asignacion.Formulario = await context.Formularios.SingleOrDefaultAsync(g => g.Id == dto.Formulario.Id);
                    asignacion.Candidato = await context.Candidatos.SingleOrDefaultAsync(c => c.Id == candidatoId);
                    context.Add(asignacion);
                }
            }       

            try
            {
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var asignacion = await context.AsignacionesFormulario.FindAsync(id);

            if (asignacion == null)
            {
                return NotFound();
            }

            context.AsignacionesFormulario.Remove(asignacion);
            await context.SaveChangesAsync();

            return NoContent();
        }
       
    }
}
