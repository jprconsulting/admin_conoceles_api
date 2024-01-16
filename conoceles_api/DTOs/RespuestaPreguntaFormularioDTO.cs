namespace conoceles_api.DTOs
{
    public class RespuestaPreguntaFormularioDTO
    {
        public int Id { get; set; }
        public string Respuesta { get; set; }
        public PreguntaFormularioDTO PreguntaFormulario { get; set; }
        public AsignacionFormularioDTO AsignacionFormulario { get; set; }
    }
}
