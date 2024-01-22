namespace conoceles_api.DTOs
{
    public class PreguntaRespuestaGoogleFormDTO
    {
        public int PreguntaFormularioId { get; set; }
        public string Pregunta { get; set; }
        public int RespuestaPreguntaFormularioId { get; set; }
        public string Respuesta { get; set; }
    }
}
