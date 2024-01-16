namespace conoceles_api.DTOs
{
    public class PreguntaFormularioDTO
    {
        public int Id { get; set; }
        public string Pregunta { get; set; }
        public FormularioDTO Formulario { get; set; }
    }
}
