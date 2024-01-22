namespace conoceles_api.Entities
{
    public class Candidato
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Sobrenombre { get; set; }
        public string NombreSuplente { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string DireccionCasaCampania { get; set; }
        public string TelefonoPublico { get; set; }
        public string Email { get; set; }
        public string PaginaWeb { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Tiktok { get; set; }
        public string Foto { get; set; }
        public bool Estatus { get; set; }
        public bool EstadoOperativo { get; set; }
        public AgrupacionPolitica AgrupacionPolitica { get; set; }
        public Cargo Cargo { get; set; }
        public Estado Estado { get; set; }
        public Genero Genero { get; set; }
        public List<AsignacionFormulario> AsignacionesFormulario { get; set; }
        public Usuario? Usuario { get; set; }

    }
}
