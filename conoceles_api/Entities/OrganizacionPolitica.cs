namespace conoceles_api.Entities
{
    public class OrganizacionPolitica
    {
        public int Id { get; set; }
        public string NombreOrganizacion { get; set; }
        public string Logo { get; set; }
        public bool Estatus { get; set; }
        public TipoOrganizacionPolitica TipoOrganizacionPolitica { get; set; }
        public List<Candidato> Candidatos { get; set; }
    }
}
