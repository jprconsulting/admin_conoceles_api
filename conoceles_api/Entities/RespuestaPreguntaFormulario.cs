namespace conoceles_api.Entities
{
    public class RespuestaPreguntaFormulario
    {
        public int Id { get; set; }
        public string Respuesta { get; set; }
        public PreguntaFormulario PreguntaFormulario { get; set; }
        public AsignacionFormulario AsignacionFormulario { get; set; }
    }
}
