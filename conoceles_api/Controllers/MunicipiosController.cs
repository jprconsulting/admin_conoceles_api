using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace conoceles_api.Controllers
{
    [Route("api/municipios")]
    [ApiController]
    public class MunicipiosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public MunicipiosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }        

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<MunicipioDTO>> GetById(int id)
        {
            var municipio = await context.Municipios
                .Include(e => e.DistritoLocal)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (municipio == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<MunicipioDTO>(municipio));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<MunicipioDTO>>> GetAll()
        {
            var municipios = await context.Municipios
                .Include(e => e.DistritoLocal)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!municipios.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<MunicipioDTO>>(municipios));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(MunicipioDTO dto)
        {
            // Validación del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeo del DTO a la entidad
            var municipio = mapper.Map<Municipio>(dto);
            municipio.DistritoLocal = await context.DistritosLocales.SingleOrDefaultAsync(r => r.Id == dto.DistritoLocal.Id);

            // Incluir la entidad en el contexto
            context.Add(municipio);

            try
            {
                // Guardar cambios en la base de datos dentro de una transacción
                await context.SaveChangesAsync();
                return Ok();
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
            var municipio = await context.Municipios.FindAsync(id);

            if (municipio == null)
            {
                return NotFound();
            }

            context.Municipios.Remove(municipio);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] MunicipioDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var municipio = await context.Municipios.FindAsync(id);

            if (municipio == null)
            {
                return NotFound();
            }

            // Mapea los datos del DTO al usuario existente
            mapper.Map(dto, municipio);
            municipio.DistritoLocal = await context.DistritosLocales.SingleOrDefaultAsync(r => r.Id == dto.DistritoLocal.Id);

            context.Update(municipio);

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
