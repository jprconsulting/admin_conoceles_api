namespace conoceles_api.Entities
{
    public class PreguntaFormulario
    {
        public int Id { get; set; }
        public string Pregunta { get; set; }
        public Formulario Formulario { get; set; }
        public List<RespuestaPreguntaFormulario> RespuestasPreguntasFormulario { get; set; }
    }
}
