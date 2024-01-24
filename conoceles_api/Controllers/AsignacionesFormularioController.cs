using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace conoceles_api.Controllers
{
    [Route("api/asignaciones-formulario")]
    [ApiController]
    public class AsignacionesFormularioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AsignacionesFormularioController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-por-candidato/{candidatoId:int}")]
        public async Task<ActionResult<List<AsignacionFormularioDTO>>> GetByCandidatoId(int candidatoId)
        {
            try
            {
                var asignacionesFormulario = await context.AsignacionesFormulario
                    .Include(g => g.Formulario)
                    .Include(c => c.Candidato)
                    .Where(a => a.Candidato.Id == candidatoId)
                    .OrderBy(u => u.Id)
                    .ToListAsync();

                if (!asignacionesFormulario.Any())
                {
                    return NotFound("No se encontraron asignaciones de formularios para el candidato con ID " + candidatoId);
                }

                return Ok(mapper.Map<List<AsignacionFormularioDTO>>(asignacionesFormulario));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<AsignacionFormularioDTO>> GetById(int id)
        {
            var asignacionFormulario = await context.AsignacionesFormulario
                .Include(g => g.Formulario)
                .Include(c => c.Candidato)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asignacionFormulario == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AsignacionFormularioDTO>(asignacionFormulario));
        }


        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<AsignacionFormularioDTO>>> GetAll()
        {
            var asignacionesFormularios = await context.AsignacionesFormulario
                .Include(g => g.Formulario)
                .Include(c => c.Candidato)
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!asignacionesFormularios.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<AsignacionFormularioDTO>>(asignacionesFormularios));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(AsignacionFormularioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formulario = await context.Formularios.SingleOrDefaultAsync(g => g.Id == dto.Formulario.Id);
            var fechaActual = DateTime.Now;

            foreach (var candidatoId in dto.CandidatosIds)
            {
                var existeAsignacion = await context.AsignacionesFormulario.AnyAsync(a => a.Candidato.Id == candidatoId 
                    && a.Formulario.Id == dto.Formulario.Id);

                if (!existeAsignacion) 
                {
                    var asignacion = mapper.Map<AsignacionFormulario>(dto);
                    asignacion.Formulario = formulario;
                    asignacion.Candidato = await context.Candidatos.SingleOrDefaultAsync(c => c.Id == candidatoId);
                    asignacion.FechaHoraAsignacion = fechaActual;
                    context.Add(asignacion);
                }
            }       

            try
            {
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("editar-asignacion")]
        public async Task<ActionResult> EditarAsignacion([FromBody] AsignacionFormularioDTO dto)
        {
            if (dto == null || dto.Formulario == null)
            {
                return BadRequest("Los parámetros no son válidos");
            }

            var formulario = await context.Formularios.SingleOrDefaultAsync(f => f.Id == dto.Formulario.Id);

            if (formulario == null)
            {
                return NotFound("Formulario no encontrado");
            }

            var asignacion = await context.AsignacionesFormulario
                .Include(a => a.Formulario)
                .FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (asignacion == null)
            {
                return NotFound("Asignación no encontrada");
            }

            // Actualiza las propiedades del formulario en la asignación con los valores del DTO
            asignacion.Formulario = formulario;

            try
            {
                await context.SaveChangesAsync();
                return Ok("Asignación editada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var asignacion = await context.AsignacionesFormulario.FindAsync(id);

            if (asignacion == null)
            {
                return NotFound();
            }

            context.AsignacionesFormulario.Remove(asignacion);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("get-edit-links-and-emails-by-form-id")]
        public async Task<ActionResult> GetEditLinksAndEmailsByFormId(int formId)
        {           
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var existsForm = await context.Formularios
                    .Include(c => c.ConfigGoogleForm)
                    .SingleOrDefaultAsync(f => f.Id == formId);

                    if (existsForm != null)
                    {
                        HttpResponseMessage response = await client.GetAsync(existsForm.EndPointEditLinks);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            ApiResponseListEditLinksDTO apiResponse = JsonConvert.DeserializeObject<ApiResponseListEditLinksDTO>(jsonResponse);

                            foreach (var item in apiResponse.EditLinksAndEmails)
                            {
                                var existsAsignacion = await context.AsignacionesFormulario
                                    .Include(c => c.Candidato)                             
                                    .SingleOrDefaultAsync(a => a.Candidato.Email == item.UserEmail 
                                        && a.Formulario.Id == existsForm.Id
                                        && string.IsNullOrEmpty(a.EditLink));

                                if (existsAsignacion != null)
                                {
                                    existsAsignacion.EditLink = item.EditLink; 
                                }
                            }
                            await context.SaveChangesAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
            

            
        }

    }
}
