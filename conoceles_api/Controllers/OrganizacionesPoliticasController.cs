using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/organizaciones-politicas")]
    [ApiController]
    public class OrganizacionesPoliticasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public OrganizacionesPoliticasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<OrganizacionPoliticaDTO>> GetById(int id)
        {
            var organizacionPolitica = await context.OrganizacionesPoliticas
                .Include(t => t.TipoOrganizacionPolitica)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organizacionPolitica == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<OrganizacionPoliticaDTO>(organizacionPolitica));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<OrganizacionPoliticaDTO>>> GetAll()
        {
            var organizacionesPoliticas = await context.OrganizacionesPoliticas
                .Include(t => t.TipoOrganizacionPolitica)
                .ToListAsync();

            if (!organizacionesPoliticas.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<OrganizacionPoliticaDTO>>(organizacionesPoliticas));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(OrganizacionPoliticaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeOrganizacionPolitica = await context.OrganizacionesPoliticas
                .AnyAsync(o => o.NombreOrganizacion == dto.NombreOrganizacion);

            if (existeOrganizacionPolitica)
            {
                return Conflict();
            }

            var organizacionPolitica = mapper.Map<OrganizacionPolitica>(dto);
            organizacionPolitica.TipoOrganizacionPolitica = await context.TiposOrganizacionesPoliticas
                .SingleOrDefaultAsync(t => t.Id == dto.TipoOrganizacionPolitica.Id);

            context.Add(organizacionPolitica);

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
            var organizacionPolitica = await context.OrganizacionesPoliticas.FindAsync(id);

            if (organizacionPolitica == null)
            {
                return NotFound();
            }

            context.OrganizacionesPoliticas.Remove(organizacionPolitica);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, OrganizacionPoliticaDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var organizacionPolitica = await context.OrganizacionesPoliticas.FindAsync(id);

            if (organizacionPolitica == null)
            {
                return NotFound();
            }

            mapper.Map(dto, organizacionPolitica);
            organizacionPolitica.TipoOrganizacionPolitica = await context.TiposOrganizacionesPoliticas
                .SingleOrDefaultAsync(t => t.Id == dto.TipoOrganizacionPolitica.Id);
            context.Update(organizacionPolitica);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizacionPoliticaExists(id))
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

        private bool OrganizacionPoliticaExists(int id)
        {
            return context.OrganizacionesPoliticas.Any(o => o.Id == id);
        }
    }
}
