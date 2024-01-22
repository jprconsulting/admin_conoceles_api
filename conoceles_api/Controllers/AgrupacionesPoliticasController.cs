using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using conoceles_api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace beneficiarios_dif_api.Controllers
{
    [Route("api/agrupaciones-politicas")]
    [ApiController]
    public class AgrupacionesPoliticasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AgrupacionesPoliticasController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<AgrupacionPoliticaDTO>> GetById(int id)
        {
            var visita = await context.AgrupacionesPoliticas
                .Include(b => b.TipoAgrupacionPolitica)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visita == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AgrupacionPoliticaDTO>(visita));
        }

        private string GetBase64Image(string fileName)
        {
            string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);

            if (System.IO.File.Exists(filePath))
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }

            return null;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var agrupaciones = await context.AgrupacionesPoliticas
                .Include(b => b.TipoAgrupacionPolitica)
                .ToListAsync();

                if (!agrupaciones.Any())
                {
                    return NotFound();
                }

                var agrupacionDTO = mapper.Map<List<AgrupacionPoliticaDTO>>(agrupaciones);

                foreach (var agrupacion in agrupacionDTO)
                {
                    agrupacion.ImagenBase64 = GetBase64Image(agrupacion.Logo); // Asigna el base64 de la imagen
                }

                return Ok(agrupacionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }



        [HttpPost("crear")]
        public async Task<ActionResult> Post(AgrupacionPoliticaDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                byte[] bytes = Convert.FromBase64String(dto.ImagenBase64);
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                dto.Logo = fileName;
            }

            var agrupacion = mapper.Map<AgrupacionPolitica>(dto);
            agrupacion.TipoAgrupacionPolitica = await context.TiposAgrupacionesPoliticas.SingleOrDefaultAsync(b => b.Id == dto.TipoAgrupacionPolitica.Id);



            context.AgrupacionesPoliticas.Add(agrupacion);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, AgrupacionPoliticaDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var agrupacion = await context.AgrupacionesPoliticas.FindAsync(id);

            if (agrupacion == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                byte[] bytes = Convert.FromBase64String(dto.ImagenBase64);
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                dto.Logo = fileName;
            }

            mapper.Map(dto, agrupacion);
            agrupacion.TipoAgrupacionPolitica = await context.TiposAgrupacionesPoliticas.SingleOrDefaultAsync(b => b.Id == dto.TipoAgrupacionPolitica.Id);

            context.Update(agrupacion);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.AgrupacionesPoliticas.Any(e => e.Id == id))
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

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var organizacion = await context.AgrupacionesPoliticas.FindAsync(id);

            if (organizacion == null)
            {
                return NotFound();
            }

            context.AgrupacionesPoliticas.Remove(organizacion);
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}