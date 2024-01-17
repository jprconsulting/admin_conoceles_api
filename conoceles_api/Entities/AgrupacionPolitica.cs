namespace conoceles_api.Entities
{
    public class AgrupacionPolitica
    {
        public int Id { get; set; }
        public string NombreOrganizacion { get; set; }
        public string Acronimo { get; set; }
        public string Logo { get; set; }
        public bool Estatus { get; set; }
        public TipoAgrupacionPolitica TipoOrganizacionPolitica { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
