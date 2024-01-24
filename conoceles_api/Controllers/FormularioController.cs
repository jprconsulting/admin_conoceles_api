using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace conoceles_api.Controllers
{
    [Route("api/formulario")]
    [ApiController]
    public class FormularioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public FormularioController(ApplicationDbContext context, IMapper mapper)
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

        [HttpGet("obtener-formularios-sin-configuracion")]
        public async Task<ActionResult<List<FormularioDTO>>> GetFormulariosSinConfiguracion()
        {
            var formularios = await context.Formularios.ToListAsync();

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
        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, FormularioDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var formulario = await context.Formularios
                .Include(f => f.ConfigGoogleForm)  // Incluye la carga del ConfigGoogleForm
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formulario == null)
            {
                return NotFound();
            }

            // Actualiza propiedades del formulario desde el DTO
            mapper.Map(dto, formulario);

            // Actualiza propiedades del ConfigGoogleForm solo si se proporciona en el DTO
            if (dto.ConfigGoogleForm != null)
            {
                // Asegúrate de que ConfigGoogleForm tenga un Id para que EF lo actualice correctamente
                dto.ConfigGoogleForm.Id = formulario.ConfigGoogleForm?.Id ?? 0;

                // Actualiza propiedades del ConfigGoogleForm
                mapper.Map(dto.ConfigGoogleForm, formulario.ConfigGoogleForm);
            }

            context.Update(formulario);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormularioExists(id))
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


        private bool FormularioExists(int id)
        {
            return context.Formularios.Any(c => c.Id == id);
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
