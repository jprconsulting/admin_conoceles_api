using AutoMapper;
using conoceles_api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/tipos-organizaciones-politicas")]
    [ApiController]
    public class TiposAgrupacionPoliticasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public TiposAgrupacionPoliticasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<TipoAgrupacionPoliticaDTO>>> GetAll()
        {
            var tiposOrganizacionesPoliticas = await context.TiposAgrupacionesPoliticas.ToListAsync();

            if (!tiposOrganizacionesPoliticas.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<TipoAgrupacionPoliticaDTO>>(tiposOrganizacionesPoliticas));
        }
    }
}
