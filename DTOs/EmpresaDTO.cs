using System.ComponentModel.DataAnnotations;

namespace api_netcore.DTOs
{
    public class RegisterEmpresa
    {
        [Required(ErrorMessage = "El RUC es requerido"), MaxLength(13, ErrorMessage = "El RUC debe tener 13 caracteres"), MinLength(13, ErrorMessage = "El RUC debe tener 13 caracteres")]
        public string? ruc { get; set; }

        [Required(ErrorMessage = "La imagen es requerida"), DataType(DataType.Upload, ErrorMessage = "El archivo debe ser de tipo imagen")]
        public IFormFile imagen { get; set; }

        [Required(ErrorMessage = "El nombre del dueño es requerido"), MaxLength(50, ErrorMessage = "El nombre del dueño debe tener máximo 50 caracteres")]
        public string? nomdueno { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es requerido"), MaxLength(50, ErrorMessage = "El nombre de la empresa debe tener.maxcdn 50 caracteres")]
        public string? nomempresa { get; set; }

        [Required(ErrorMessage = "La direccion es requerida"), MaxLength(100, ErrorMessage = "La direccion debe tener.maxcdn 100 caracteres")]
        public string? direccion { get; set; }

        [Required(ErrorMessage = "El telefono es requerido"), MaxLength(10, ErrorMessage = "El telefono debe tener máximo 10 caracteres"), MinLength(10, ErrorMessage = "El telefono debe tener mínimo 10 caracteres")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "El email es requerido"), MaxLength(50, ErrorMessage = "El email debe tener máximo 50 caracteres"), EmailAddress(ErrorMessage = "El formato del email es incorrecto")]
        public string? email { get; set; }
    }

    public class UpdateEmpresa
    {
        [Required(ErrorMessage = "El id es requerido")]
        public int id { get; set; }

        [MaxLength(13, ErrorMessage = "El RUC debe tener 13 caracteres"), MinLength(13, ErrorMessage = "El RUC debe tener 13 caracteres")]
        public string? ruc { get; set; }

        [Required(ErrorMessage = "La imagen es requerida"), DataType(DataType.Upload, ErrorMessage = "El archivo debe ser de tipo imagen")]
        public IFormFile imagen { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre del dueño debe tener máximo 50 caracteres")]
        public string? nomdueno { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre de la empresa debe tener.maxcdn 50 caracteres")]
        public string? nomempresa { get; set; }

        [MaxLength(100, ErrorMessage = "La direccion debe tener.maxcdn 100 caracteres")]
        public string? direccion { get; set; }
        [MaxLength(10, ErrorMessage = "El telefono debe tener máximo 10 caracteres"), MinLength(10, ErrorMessage = "El telefono debe tener mínimo 10 caracteres")]
        public string? telefono { get; set; }

        [MaxLength(50, ErrorMessage = "El email debe tener máximo 50 caracteres"), EmailAddress(ErrorMessage = "El formato del email es incorrecto")]
        public string? email { get; set; }
    }

    public class ShowEmpresa
    {
        public int id { get; set; }
        public string? ruc { get; set; }
        public string? imagen { get; set; }
        public string? nomdueno { get; set; }
        public string? nomempresa { get; set; }
        public string? email { get; set; }
        public int? estado { get; set; }
        public string? estado_text => estado == 1 ? "Activo" : "Inactivo";
    }

    public class ShowEmpresaDetalle
    {
        public int id { get; set; }
        public string? ruc { get; set; }
        public string? nomdueno { get; set; }
        public string? nomempresa { get; set; }
        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public string? email { get; set; }
        public DateTime? fechacreacion { get; set; }
        public int? estado { get; set; }
        public string? estado_text => estado == 1 ? "Activo" : "Inactivo";
    }

}
