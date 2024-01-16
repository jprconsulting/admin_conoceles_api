namespace conoceles_api.DTOs
{
    public class FormularioDTO
    {
        public int? Id { get; set; }
        public string NombreFormulario { get; set; }
        public ConfigGoogleFormDTO ConfigGoogleForm { get; set; }
    }
}
