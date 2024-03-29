﻿using AutoMapper;
using conoceles_api.DTOs;
using conoceles_api.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace conoceles_api.Controllers
{
    [Route("api/respuestas-google-form")]
    [ApiController]
    public class RespuestasGoogleFormController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public RespuestasGoogleFormController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("importar-respuestas-google-form/{id:int}")]
        public async Task<IActionResult> ImportarRespuestasGoogleFor(int id)
        {
            var formulario = await context.Formularios
                .Include(c => c.ConfigGoogleForm)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (formulario != null)
            {
                var credentials = new CredentialsJSON
                {
                    type = formulario.ConfigGoogleForm.Type,
                    project_id = formulario.ConfigGoogleForm.ProjectId,
                    private_key_id = formulario.ConfigGoogleForm.PrivateKeyId,
                    private_key = formulario.ConfigGoogleForm.PrivateKey,
                    client_email = formulario.ConfigGoogleForm.ClientEmail,
                    client_id = formulario.ConfigGoogleForm.ClientId,
                    auth_uri = formulario.ConfigGoogleForm.AuthUri,
                    token_uri = formulario.ConfigGoogleForm.TokenUri,
                    auth_provider_x509_cert_url = formulario.ConfigGoogleForm.AuthProviderX509CertUrl,
                    client_x509_cert_url = formulario.ConfigGoogleForm.ClientX509CertUrl,
                    universe_domain = formulario.ConfigGoogleForm.UniverseDomain
                };

                string json = JsonConvert.SerializeObject(credentials, Formatting.Indented);

                // Configura las credenciales de autenticación (OAuth 2.0 o API key)
                GoogleCredential credential = GoogleCredential.FromJson(json);

                // Crea el servicio de Google Sheets
                SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                });

                // ID de la hoja de cálculo vinculada al formulario de Google
                string spreadsheetId = formulario.SpreadsheetId;

                // Rango de datos que deseas obtener (por ejemplo, "Sheet1!A:F" para todas las columnas de la hoja "Sheet1")
                string range = $"{formulario.SheetName}!A1:F";

                // Realiza la solicitud para obtener los datos
                SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);

                ValueRange response = request.Execute();

                IList<IList<object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    var preguntasIndex = new List<PreguntaIndexDTO>();

                    var nombresPreguntas = values[0];

                    for (int i = 0; i < nombresPreguntas.Count; i++)
                    {
                        string nombrePregunta = nombresPreguntas[i]?.ToString() ?? "";
                        var existePregunta = await context.PreguntasFormulario
                            .FirstOrDefaultAsync(p => p.Formulario.Id == formulario.Id && p.Pregunta == nombrePregunta);

                        // !string.IsNullOrWhiteSpace(nombrePregunta)
                        if (existePregunta == null)
                        {
                            var preguntaDTO = new PreguntaFormularioDTO() 
                            { 
                                Pregunta = nombrePregunta,
                            };

                            var preguntaFormulario = mapper.Map<PreguntaFormulario>(preguntaDTO);
                            preguntaFormulario.Formulario = formulario;
                            context.PreguntasFormulario.Add(preguntaFormulario);
                            await context.SaveChangesAsync();
                            preguntasIndex.Add(new PreguntaIndexDTO { PreguntaDBId = preguntaFormulario.Id, Index = i });
                        }
                        else
                        {
                            preguntasIndex.Add(new PreguntaIndexDTO { PreguntaDBId = existePregunta.Id, Index = i });
                        }

                    }

                    // Procesa las filas de respuestas (values[1] en adelante)
                    for (int rowIndex = 1; rowIndex < values.Count; rowIndex++)
                    {
                        var respuestas = values[rowIndex];

                        var email = respuestas[1];

                        var existeCandidato = await context.Candidatos.FirstOrDefaultAsync(c => string.Equals(c.Email, email));


                        if (existeCandidato != null)
                        {
                            var existeAsignacion = await context.AsignacionesFormulario
                                .FirstOrDefaultAsync(a => a.Candidato.Id == existeCandidato.Id && a.Formulario.Id == formulario.Id);

                            if (existeAsignacion != null)
                            {
                                for (int i = 0; i < respuestas.Count; i++)
                                {
                                    string respuestaCandidato = respuestas[i]?.ToString() ?? "";
                                    int preguntaCuestionarioIdDB = preguntasIndex.FirstOrDefault(p => p.Index == i)?.PreguntaDBId ?? 0;

                                    var preguntaCuestionario = await context.PreguntasFormulario
                                        .FirstOrDefaultAsync(p => p.Id == preguntaCuestionarioIdDB);

                                    var existeRespuesta = await context.RespuestasPreguntaFormulario
                                        .FirstOrDefaultAsync(r => r.PreguntaFormulario.Id == preguntaCuestionarioIdDB
                                            && r.AsignacionFormulario.Id == existeAsignacion.Id);


                                    if (existeRespuesta != null)
                                    {
                                        existeRespuesta.Respuesta = respuestaCandidato;
                                    }
                                    else
                                    {

                                        var respuestaPreguntaDTO = new RespuestaPreguntaFormularioDTO() 
                                        { 
                                            Respuesta = respuestaCandidato,
                                        };

                                        var respuestaPreguntaFormulario = mapper.Map<RespuestaPreguntaFormulario>(respuestaPreguntaDTO);
                                        respuestaPreguntaFormulario.AsignacionFormulario = existeAsignacion;
                                        respuestaPreguntaFormulario.PreguntaFormulario = preguntaCuestionario;
                                        context.RespuestasPreguntaFormulario.Add(respuestaPreguntaFormulario);
                                    }

                                    await context.SaveChangesAsync();
                                }
                            }                            
                        }
                    }
                }
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("respuestas-preguntas-google-form-por-candidato-id/{candidatoId}")]
        public async Task<CandidatoPreguntasRespuestasGoogleFormDTO> RespuestasPreguntasGoogleFormPorCandidatoId(int candidatoId)
        {
            CandidatoPreguntasRespuestasGoogleFormDTO formulariosCandidato = new CandidatoPreguntasRespuestasGoogleFormDTO();
            var infoCandidato = await context.Candidatos.FirstOrDefaultAsync(c => c.Id == candidatoId);

            if (infoCandidato != null)
            {
                formulariosCandidato.CandidatoId = candidatoId;
                formulariosCandidato.NombreCompleto = $"{infoCandidato.Nombres} {infoCandidato.ApellidoPaterno} {infoCandidato.ApellidoMaterno}";
                formulariosCandidato.Formularios = new List<FormularioPreguntasRespuestasGoogleFormDTO>();
                var formulariosIds = await context.Formularios.Include(f => f.ConfigGoogleForm).ToListAsync();

                foreach (var form in formulariosIds)
                {
                    var preguntasRespuestas = await (from p in context.PreguntasFormulario
                                                     join r in context.RespuestasPreguntaFormulario
                                                     on p.Id equals r.PreguntaFormulario.Id
                                                     join a in context.AsignacionesFormulario
                                                     on r.AsignacionFormulario.Id equals a.Id
                                                     where a.Formulario.Id == form.Id && a.Candidato.Id == candidatoId
                                                     select new PreguntaRespuestaGoogleFormDTO
                                                     {
                                                         PreguntaFormularioId = p.Id,
                                                         Pregunta = p.Pregunta,
                                                         RespuestaPreguntaFormularioId = r.Id,
                                                         Respuesta = r.Respuesta
                                                     }).ToListAsync();

                    var formularioPreguntasRespuestas = new FormularioPreguntasRespuestasGoogleFormDTO
                    {
                        FormularioId = form.Id,
                        NombreFormulario = form.NombreFormulario,
                        GoogleFormId = form.GoogleFormId,
                        PreguntasRespuestas = preguntasRespuestas
                    };

                    formulariosCandidato.Formularios.Add(formularioPreguntasRespuestas);


                }
            }

            return formulariosCandidato;
        }






    }
}
