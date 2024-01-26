using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace conoceles_api.Controllers
{
    [Route("api/public-candidatos")]
    [ApiController]
    public class CandidatosPublicController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CandidatosPublicController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
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
                .Include(p => p.Comunidad)
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
    }
}
