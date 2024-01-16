using AutoMapper;
using conoceles_api.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/estados")]
    [ApiController]
    public class EstadosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public EstadosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<EstadoDTO>>> GetAll()
        {
            var estados = await context.Estados.ToListAsync();

            if (!estados.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<EstadoDTO>>(estados));
        }

    }
}
