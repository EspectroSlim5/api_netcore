namespace api_netcore.DTOs
{
    public class EmpresaExcel
    {
        public string? ruc { get; set; }
        public string? imagen { get; set; } // Ruta de la imagen en Excel
        public string? nombredueno { get; set; }
        public string? nombrempresa { get; set; }
        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public string? correo { get; set; }
        public int? estado { get; set; }
    }
}