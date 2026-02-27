using System.ComponentModel.DataAnnotations;

namespace api_netcore.DTOs
{
    public class RegisterEmpleado
    {
        [Required(ErrorMessage = "El nombre es requerido"), MaxLength(50, ErrorMessage = "El nombre debe tener máximo 50 caracteres")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido"), MaxLength(50, ErrorMessage = "El apellido debe tener.maxcdn 50 caracteres")]
        public string? apellido { get; set; }

        [Required(ErrorMessage = "La direccion es requerida"), MaxLength(100, ErrorMessage = "La direccion debe tener.maxcdn 100 caracteres")]
        public string? direccion { get; set; }

        [Required(ErrorMessage = "El telefono es requerido"), MaxLength(10, ErrorMessage = "El telefono debe tener 10 caracteres"), MinLength(10, ErrorMessage = "El telefono debe tener 10 caracteres")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "El correo es requerido"), MaxLength(50, ErrorMessage = "El correo debe tener.maxcdn 50 caracteres"), EmailAddress(ErrorMessage = "El formato del correo es incorrecto")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida"), DataType(DataType.Date, ErrorMessage = "El formato de la fecha debe ser yyyy-MM-dd")]
        public DateTime? fechanacimiento { get; set; }

        [Required(ErrorMessage = "La empresa es requerido"), Range(1, int.MaxValue, ErrorMessage = "La empresa es requerida")]
        public int? empresa_id { get; set; }
    }

    public class UpdateEmpleado
    {
        [Required(ErrorMessage = "El id es requerido")]
        public int? id { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre debe tener máximo 50 caracteres")]
        public string? nombre { get; set; }

        [MaxLength(50, ErrorMessage = "El apellido debe tener.maxcdn 50 caracteres")]
        public string? apellido { get; set; }

        [MaxLength(100, ErrorMessage = "La direccion debe tener.maxcdn 100 caracteres")]
        public string? direccion { get; set; }

        [MaxLength(10, ErrorMessage = "El telefono debe tener 10 caracteres"), MinLength(10, ErrorMessage = "El telefono debe tener 10 caracteres")]
        public string? telefono { get; set; }

        [MaxLength(50, ErrorMessage = "El correo debe tener.maxcdn 50 caracteres"), EmailAddress(ErrorMessage = "El formato del correo es incorrecto")]
        public string? correo { get; set; }

        [DataType(DataType.Date, ErrorMessage = "El formato de la fecha debe ser yyyy-MM-dd"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? fechanacimiento { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La empresa es requerida")]
        public int? empresa_id { get; set; }
    }

    public class ShowEmpleado
    {
        public int? id { get; set; }
        public string? nombrecompleto { get; set; }
        public string? correo { get; set; }
        public DateTime? fechanacimiento { get; set; }
        public string? empresa { get; set; }
        public int? estado { get; set; }
        public virtual ShowEmpresa? Empresa { get; set; }
    }

    public class ShowEmpleadoDetalle
    {
        public int? id { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public string? correo { get; set; }
        public DateTime? fechanacimiento { get; set; }
        public DateTime? fechaingreso { get; set; }
        public string? empresa { get; set; }
        public int? estado { get; set; }
    }
}
