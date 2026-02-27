using System.ComponentModel.DataAnnotations.Schema;

namespace api_netcore.Models
{
    // En tu Models/Empresa.cs
    public class Empresa
    {
        public int Id { get; set; }
        public string? RutaImagen { get; set; }

        [Column(TypeName = "nvarchar(20)")] // FORZAR como string en BD
        public string? Ruc { get; set; }     // ← Asegurar que es string, no int

        public string? NombreDueno { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? Estado { get; set; } = 1;
        public virtual ICollection<Empleado>? Empleados { get; set; }
    }
}
