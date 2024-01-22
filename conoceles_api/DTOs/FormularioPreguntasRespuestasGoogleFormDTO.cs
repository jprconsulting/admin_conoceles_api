namespace conoceles_api.DTOs
{
    public class FormularioPreguntasRespuestasGoogleFormDTO
    {
        public int FormularioId { get; set; }
        public string NombreFormulario { get; set; }
        public string GoogleFormId { get; set; }
        public List<PreguntaRespuestaGoogleFormDTO> PreguntasRespuestas { get; set; }
    }
}
