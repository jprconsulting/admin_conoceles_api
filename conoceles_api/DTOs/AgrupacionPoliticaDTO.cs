﻿namespace conoceles_api.DTOs
{
    public class AgrupacionPoliticaDTO
    {
        public int? Id { get; set; }
        public string NombreAgrupacion { get; set; }
        public string Acronimo { get; set; }
        public string Logo { get; set; }
        public string ImagenBase64 { get; set; }
        public bool Estatus { get; set; }
        public TipoAgrupacionPoliticaDTO TipoAgrupacionPolitica { get; set; }
    }
}
