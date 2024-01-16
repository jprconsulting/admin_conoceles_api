using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/cargos")]
    [ApiController]
    public class CargosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CargosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<CargoDTO>> GetById(int id)
        {
            var cargo = await context.Cargos.FirstOrDefaultAsync(c => c.Id == id);

            if (cargo == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CargoDTO>(cargo));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<CargoDTO>>> GetAll()
        {
            var cargos = await context.Cargos.ToListAsync();

            if (!cargos.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<CargoDTO>>(cargos));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(CargoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeCargo = await context.Cargos.AnyAsync(c => c.NombreCargo == dto.NombreCargo);

            if (existeCargo)
            {
                return Conflict();
            }

            var cargo = mapper.Map<Cargo>(dto);
            context.Add(cargo);

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
            var cargo = await context.Cargos.FindAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }

            context.Cargos.Remove(cargo);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, CargoDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var cargo = await context.Cargos.FindAsync(id);

            if (cargo == null)
            {
                return NotFound();
            }

            mapper.Map(dto, cargo);
            context.Update(cargo);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CargoExists(id))
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

        private bool CargoExists(int id)
        {
            return context.Cargos.Any(c => c.Id == id);
        }
    }
}
