namespace conoceles_api.Entities
{
    public class AgrupacionPolitica
    {
        public int Id { get; set; }
        public string NombreAgrupacion { get; set; }
        public string Acronimo { get; set; }
        public string Logo { get; set; }
        public bool Estatus { get; set; }
        public bool EstadoOperativo { get; set; }
        public TipoAgrupacionPolitica TipoAgrupacionPolitica { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
