namespace api_netcore.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string? NombreEmpleado { get; set; }
        public string? ApellidoEmpleado { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public int? IdEmpresa { get; set; }
        public int Estado { get; set; } = 1;
        public virtual Empresa? Empresa { get; set; }
    }
}
