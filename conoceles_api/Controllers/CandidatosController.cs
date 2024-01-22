using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace conoceles_api.Controllers
{
    [Route("api/candidatos")]
    [ApiController]
    public class CandidatosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CandidatosController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<CandidatoDTO>> GetById(int id)
        {
            var candidato = await context.Candidatos
                .Include(g => g.Genero)
                .Include(e => e.Estado)
                .Include(d => d.DistritoLocal)
                .Include(m => m.Municipio)
                .Include(c => c.Comunidad)
                .Include(g => g.Genero)

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
                var candidatos = await context.Candidatos
                .Include(g => g.Genero)
                .Include(e => e.Estado)
                .Include(d => d.DistritoLocal)
                .Include(m => m.Municipio)
                .Include(c => c.Comunidad)
                .Include(g => g.Genero)

                .Include(c => c.Cargo)
                .Include(o => o.AgrupacionPolitica)
                    .ThenInclude(t => t.TipoAgrupacionPolitica)
                .ToListAsync();

                if (!candidatos.Any())
                {
                    return NotFound();
                }

                var candidatosDTO = mapper.Map<List<CandidatoDTO>>(candidatos);

                foreach (var candidato in candidatosDTO)
                {
                    candidato.ImagenBase64 = GetBase64Image(candidato.Foto); 
                }

                return Ok(candidatosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(CandidatoDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                byte[] bytes = Convert.FromBase64String(dto.ImagenBase64);
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                dto.Foto = fileName;
            }

            var candidato = mapper.Map<Candidato>(dto);
            candidato.Genero = await context.Generos.SingleOrDefaultAsync(b => b.Id == dto.Genero.Id);
            candidato.AgrupacionPolitica = await context.AgrupacionesPoliticas.SingleOrDefaultAsync(o => o.Id == dto.AgrupacionPolitica.Id);
            candidato.Cargo = await context.Cargos.SingleOrDefaultAsync(b => b.Id == dto.Cargo.Id);
            candidato.Estado = await context.Estados.SingleOrDefaultAsync(o => o.Id == dto.Estado.Id);
            if (dto.DistritoLocal != null)
            {
                candidato.DistritoLocal = await context.DistritosLocales.SingleOrDefaultAsync(c => c.Id == dto.DistritoLocal.Id);
            }
            if (dto.Municipio != null)
            {
                candidato.Municipio = await context.Municipios.SingleOrDefaultAsync(c => c.Id == dto.Municipio.Id);
            }
            if (dto.Comunidad != null)
            {
                candidato.Comunidad = await context.Comunidades.SingleOrDefaultAsync(c => c.Id == dto.Comunidad.Id);
            }

            context.Candidatos.Add(candidato);
            await context.SaveChangesAsync();

            return Ok();
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
