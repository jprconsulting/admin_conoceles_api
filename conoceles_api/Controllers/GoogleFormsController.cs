using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/google-form")]
    [ApiController]
    public class GoogleFormsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GoogleFormsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<FormularioDTO>> GetById(int id)
        {
            var formulario = await context.Formularios
                .Include(c => c.ConfigGoogleForm)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (formulario == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<FormularioDTO>(formulario));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<FormularioDTO>>> GetAll()
        {
            var formularios = await context.Formularios
                .Include(c => c.ConfigGoogleForm)
                .ToListAsync();

            if (!formularios.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<FormularioDTO>>(formularios));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(FormularioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existeFormulario = await context.Formularios.AnyAsync(g => g.NombreFormulario == dto.NombreFormulario);

            if (existeFormulario)
            {
                return Conflict();
            }

            using var transaction = context.Database.BeginTransaction();
            try
            {
                var configGoogleForm = mapper.Map<ConfigGoogleForm>(dto.ConfigGoogleForm);
                context.Add(configGoogleForm);

                await context.SaveChangesAsync();

                var formulario = mapper.Map<Formulario>(dto);
                formulario.ConfigGoogleForm = configGoogleForm;
                context.Add(formulario);

                await context.SaveChangesAsync();

                // Confirma la transacción si todo va bien
                transaction.Commit();

                return Ok();
            }
            catch (Exception ex)
            {
                // Revierte la transacción en caso de error
                transaction.Rollback();
                return StatusCode(500, new { error = "Error interno del servidor.", details = ex.Message });
            }
        }


        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var formulario = await context.Formularios.FindAsync(id);

            if (formulario == null)
            {
                return NotFound();
            }

            context.Formularios.Remove(formulario);
            await context.SaveChangesAsync();

            return NoContent();
        }
       
    }
}
