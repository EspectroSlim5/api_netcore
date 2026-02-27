using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using api_netcore.Data;
using api_netcore.DTOs;
using api_netcore.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace api_netcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment _env;

        public EmpresaController(AppDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            _env = env;
        }

        // obtener todas las empresas
        [HttpGet("mostrar")]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetAll()
        {
            var empresas = await db.Tbl_Empresa.OrderByDescending(e => e.Id).ToListAsync();

            // Verificar si la lista de empresas está vacía
            if (empresas.Count == 0) return NotFound(new { message = "No se encontraron empresas registradas" });

            // retornar la lista de empresas con el dto
            List<ShowEmpresa> listaEmpresas = new List<ShowEmpresa>();
            foreach (var emp in empresas)
            {
                listaEmpresas.Add(new ShowEmpresa
                {
                    id = emp.Id,
                    ruc = emp.Ruc,
                    imagen = emp.RutaImagen,
                    nomdueno = emp.NombreDueno,
                    nomempresa = emp.NombreEmpresa,
                    email = emp.Email,
                    estado = emp.Estado

                });
            }
            return Ok(listaEmpresas);
        }

        // obtener una empresa por id
        [HttpGet("buscar/{id:int}")]
        public async Task<ActionResult<Empresa>> GetById(int id)
        {
            var empresa = await db.Tbl_Empresa.FindAsync(id);
            if (empresa == null) return NotFound(new { message = "No se encontraron empresas registradas" });
            List<ShowEmpresaDetalle> listaEmpresa = new List<ShowEmpresaDetalle>();
            listaEmpresa.Add(new ShowEmpresaDetalle
            {
                id = empresa.Id,
                ruc = empresa.Ruc,
                nomdueno = empresa.NombreDueno,
                nomempresa = empresa.NombreEmpresa,
                direccion = empresa.Direccion,
                telefono = empresa.Telefono,
                email = empresa.Email,
                fechacreacion = empresa.FechaCreacion,
                estado = empresa.Estado
            });
            return Ok(listaEmpresa);
        }

        // crear una empresa
        [HttpPost("crear")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Empresa>> Post([FromForm] RegisterEmpresa empresa)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validar si ya existe
                var empresaDB = await db.Tbl_Empresa.FirstOrDefaultAsync(em => em.Ruc == empresa.ruc);
                if (empresaDB != null)
                    return BadRequest(new { message = "Ya existe una empresa con el RUC proporcionado" });

                // Guardar imagen
                //string rutaImagen = await GuardarImagenAsync(empresa.imagen);

                var nuevaEmpresa = new Empresa
                {
                    Ruc = empresa.ruc,
                    //RutaImagen = rutaImagen,
                    NombreDueno = empresa.nomdueno,
                    NombreEmpresa = empresa.nomempresa,
                    Direccion = empresa.direccion,
                    Telefono = empresa.telefono,
                    Email = empresa.email == null ? "" : empresa.email.Trim().ToLower(),
                };

                db.Tbl_Empresa.Add(nuevaEmpresa);
                await db.SaveChangesAsync();

                return Ok(nuevaEmpresa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al guardar la empresa: " + ex.Message });
            }
        }


        // editar una empresa
        [HttpPut("editar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Empresa>> Put([FromForm] UpdateEmpresa empresa)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);


                //validar que la empresa exista
                var empresaDB = await db.Tbl_Empresa.FirstOrDefaultAsync(e => e.Id == empresa.id);
                if (empresaDB == null) return NotFound(new { message = "No se encontraro la empresa proporcionada" });

                //validar que la empresa no exista a excepcion de la que se quiere editar
                var empresaDB2 = await db.Tbl_Empresa.FirstOrDefaultAsync(em => em.Ruc == empresa.ruc && em.Id != empresa.id);
                if (empresaDB2 != null) return BadRequest(new { message = "Ya existe una empresa con el ruc proporcionado" });

                // Si se subió una nueva imagen → eliminar la anterior y guardar la nueva
                if (empresa.imagen != null)
                {
                    if (!string.IsNullOrEmpty(empresaDB.RutaImagen))
                    {
                        var rutaVieja = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", empresaDB.RutaImagen);
                        if (System.IO.File.Exists(rutaVieja))
                            System.IO.File.Delete(rutaVieja);
                    }

                    //empresaDB.RutaImagen = await GuardarImagenAsync(empresa.imagen);
                }

                if (!string.IsNullOrEmpty(empresa.ruc)) empresaDB.Ruc = empresa.ruc;
                if (!string.IsNullOrEmpty(empresa.nomdueno)) empresaDB.NombreDueno = empresa.nomdueno;
                if (!string.IsNullOrEmpty(empresa.nomempresa)) empresaDB.NombreEmpresa = empresa.nomempresa;
                if (!string.IsNullOrEmpty(empresa.direccion)) empresaDB.Direccion = empresa.direccion;
                if (!string.IsNullOrEmpty(empresa.telefono)) empresaDB.Telefono = empresa.telefono;
                if (!string.IsNullOrEmpty(empresa.email)) empresaDB.Email = empresa.email;

                db.Tbl_Empresa.Update(empresaDB);
                await db.SaveChangesAsync();
                return Ok(empresaDB);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrio un error al editar la empresa: " + ex.Message });
            }
        }


        // eliminar una empresa
        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult<Empresa>> Delete(int id)
        {
            try
            {
                //validar que la empresa exista
                var empresa = await db.Tbl_Empresa.FindAsync(id);
                if (empresa == null) return NotFound(new { message = "No se encontraro la empresa proporcionada" });
                db.Tbl_Empresa.Remove(empresa);

                await db.SaveChangesAsync();
                return Ok(empresa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrio un error al eliminar la empresa: " + ex.Message });
            }
        }


        // activar o desactivar una empresa
        [HttpPut("eliminar_logico/{id:int}")]
        public async Task<ActionResult<Empresa>> DeleteLogico(int id, int estado)
        {
            try
            {
                //validar que la empresa exista
                var empresa = await db.Tbl_Empresa.FindAsync(id);
                if (empresa == null) return NotFound(new { message = "No se encontraro la empresa proporcionada" });

                // validar que el estado sea 0 o 1
                if (estado != 0 && estado != 1) return BadRequest(new { message = "El estado debe ser 0 o 1" });

                if (empresa.Estado == estado) return BadRequest(new { message = "El empleado ya se encuentra en ese estado" });


                empresa.Estado = estado;
                await db.SaveChangesAsync();
                return Ok(empresa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrio un error al eliminar la empresa: " + ex.Message });
            }
        }

        [HttpPost("importar")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<object>> Importar([FromForm] IFormFile file)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Paul Torres");

            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Archivo inválido" });

                // Validar extensión
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                    return BadRequest(new { message = "El archivo debe ser Excel (.xlsx o .xls)" });

                // Leer el excel
                var empresas = await LeerExcel(file);

                if (empresas.Count == 0)
                    return BadRequest(new { message = "No se encontraron datos válidos en el Excel" });

                string carpeta = Path.Combine(_env.WebRootPath, "Files", "Images");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var empresasCreadas = new List<Empresa>();
                var empresasActualizadas = new List<Empresa>();
                var errores = new List<string>();

                foreach (var item in empresas)
                {
                    try
                    {
                        // Buscar si ya existe por RUC
                        var empresaExistente = await db.Tbl_Empresa
                            .FirstOrDefaultAsync(e => e.Ruc == item.ruc);

                        string rutaimagen = "Files/Images/default.png";
                        if (!string.IsNullOrEmpty(item.imagen) && System.IO.File.Exists(item.imagen))
                        {
                            rutaimagen = GuardarImagenXRuta(item.imagen, carpeta);
                        }

                        if (empresaExistente != null)
                        {
                            // ACTUALIZAR
                            empresaExistente.NombreDueno = item.nombredueno?.Trim() ?? empresaExistente.NombreDueno;
                            empresaExistente.NombreEmpresa = item.nombrempresa?.Trim() ?? empresaExistente.NombreEmpresa;
                            empresaExistente.Direccion = item.direccion?.Trim() ?? empresaExistente.Direccion;
                            empresaExistente.Telefono = item.telefono?.Trim() ?? empresaExistente.Telefono;
                            empresaExistente.Email = item.correo?.Trim().ToLower() ?? empresaExistente.Email;
                            empresaExistente.RutaImagen = rutaimagen;
                            empresaExistente.Estado = item.estado ?? empresaExistente.Estado;

                            db.Tbl_Empresa.Update(empresaExistente);
                            empresasActualizadas.Add(empresaExistente);
                        }
                        else
                        {
                            // CREAR
                            var nuevaEmpresa = new Empresa
                            {
                                Ruc = item.ruc,
                                NombreDueno = item.nombredueno?.Trim(),
                                NombreEmpresa = item.nombrempresa?.Trim(),
                                Direccion = item.direccion?.Trim() ?? "",
                                Telefono = item.telefono?.Trim() ?? "",
                                Email = item.correo?.Trim().ToLower() ?? "",
                                RutaImagen = rutaimagen,
                                Estado = item.estado ?? 1,
                            };

                            db.Tbl_Empresa.Add(nuevaEmpresa);
                            empresasCreadas.Add(nuevaEmpresa);
                        }
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Error con RUC {item.ruc}: {ex.Message}");
                    }
                }

                await db.SaveChangesAsync();

                var mensaje = $"{empresasCreadas.Count} empresas creadas, {empresasActualizadas.Count} empresas actualizadas";

                return Ok(new
                {
                    message = mensaje,
                    creadas = empresasCreadas.Count,
                    actualizadas = empresasActualizadas.Count,
                    errores = errores
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error al importar",
                    error = ex.Message
                });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GuardarImagenXRuta(string? rutaImagen, string carpetaDestino)
        {
            try
            {
                // Si no hay ruta o no existe, retornar default
                if (string.IsNullOrEmpty(rutaImagen) || !System.IO.File.Exists(rutaImagen))
                    return "Files/Images/default.jpg";

                // Validar que sea una imagen
                var extensiones = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(rutaImagen).ToLower();

                if (!extensiones.Contains(extension))
                    return "Files/Images/default.png";

                // Generar nombre único para evitar sobrescritura
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaDestino = Path.Combine(carpetaDestino, nombreArchivo);

                // Copiar archivo
                System.IO.File.Copy(rutaImagen, rutaDestino, overwrite: false);

                return $"Files/Images/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copiando imagen: {ex.Message}");
                return "Files/Images/default.png";
            }
        }

        [HttpGet("imagen/{nombre}")]
        [AllowAnonymous]
        public IActionResult GetImagen(string nombre)
        {
            var ruta = Path.Combine(_env.WebRootPath, "Files", "Images", nombre);

            if (!System.IO.File.Exists(ruta))
                ruta = Path.Combine(_env.WebRootPath, "Files", "Images", "default.png");

            var extension = Path.GetExtension(ruta).ToLower();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };

            var imagenBytes = System.IO.File.ReadAllBytes(ruta);
            return File(imagenBytes, contentType);
        }

        private async Task<List<EmpresaExcel>> LeerExcel(IFormFile file)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Paul Torres");

            var listaEmpresa = new List<EmpresaExcel>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var hoja = package.Workbook.Worksheets[0];
                    if (hoja.Dimension == null)
                        throw new Exception("El archivo Excel está vacío");

                    int filas = hoja.Dimension.Rows;

                    for (int i = 2; i <= filas; i++)
                    {
                        string ruc = "";
                        var cellValue = hoja.Cells[i, 1].Value;

                        if (cellValue != null)
                        {
                            if (cellValue is double doubleValue)
                            {
                                // Si es número, convertir a string sin decimales
                                ruc = doubleValue.ToString("F0").Replace(".", "").Replace(",", "");
                            }
                            else
                            {
                                ruc = cellValue.ToString().Trim();
                            }

                            // Eliminar cualquier carácter no numérico
                            ruc = new string(ruc.Where(c => char.IsDigit(c)).ToArray());

                            // Limitar longitud si es necesario
                            if (ruc.Length > 20)
                                ruc = ruc.Substring(0, 20);
                        }

                        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length < 5)
                            continue;

                        var dto = new EmpresaExcel
                        {
                            ruc = ruc,
                            nombredueno = hoja.Cells[i, 2].Text?.Trim(),
                            nombrempresa = hoja.Cells[i, 3].Text?.Trim(),
                            direccion = hoja.Cells[i, 4].Text?.Trim(),
                            telefono = hoja.Cells[i, 5].Text?.Trim(),
                            correo = hoja.Cells[i, 6].Text?.Trim(),
                            imagen = hoja.Cells[i, 7].Text?.Trim(),
                            estado = int.TryParse(hoja.Cells[i, 8].Text?.Trim(), out int estado) ? estado : 1
                        };

                        listaEmpresa.Add(dto);
                    }
                }
            }

            return listaEmpresa;
        }

        // GUARDAR IMAGEN DESDE IFormFile
        private async Task<string> GuardarImagenAsync(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return "Files/Images/default.png"; // Retorna default si no hay imagen

            // Validar extensiones permitidas
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension))
                throw new Exception("El archivo debe ser una imagen (jpg, jpeg, png, gif, bmp)");

            // Validar tamaño máximo (5MB)
            if (archivo.Length > 5 * 1024 * 1024)
                throw new Exception("La imagen no debe superar los 5MB");

            try
            {
                // Carpeta donde se guardarán las imágenes
                var carpetaBase = _env.WebRootPath;
                if (string.IsNullOrEmpty(carpetaBase))
                {
                    carpetaBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var carpetaImagenes = Path.Combine(carpetaBase, "Files", "Images");

                if (!Directory.Exists(carpetaImagenes))
                    Directory.CreateDirectory(carpetaImagenes);

                // Generar nombre único
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

                // Guardar archivo
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Retornar ruta relativa para BD (usando /)
                return $"Files/Images/{nombreArchivo}";
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error guardando imagen: {ex.Message}");
                return "Files/Images/default.png"; // Retorna default en caso de error
            }
        }
        [HttpPost("reactivar-empleados")]
        public async Task<ActionResult> ReactivarEmpleados()
        {
            try
            {
                var empleadosInactivos = await db.Tbl_Empleado
                    .Where(e => e.Estado == 0)
                    .Include(e => e.Empresa)
                    .ToListAsync();

                if (!empleadosInactivos.Any())
                    return Ok(new { message = "No hay empleados inactivos" });

                var empresasActivas = await db.Tbl_Empresa
                    .Where(e => e.Estado == 1)
                    .ToListAsync();

                if (!empresasActivas.Any())
                    return BadRequest(new { message = "No hay empresas activas para asignar" });

                int index = 0;
                foreach (var empleado in empleadosInactivos)
                {
                    empleado.IdEmpresa = empresasActivas[index % empresasActivas.Count].Id;
                    empleado.Estado = 1;
                    index++;
                }

                await db.SaveChangesAsync();

                return Ok(new
                {
                    message = $"{empleadosInactivos.Count} empleados reactivados",
                    empleadosReactivados = empleadosInactivos.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("inactivos")]
        public async Task<ActionResult> GetInactivos()
        {
            var empleados = await db.Tbl_Empleado
                .Where(e => e.Estado == 0)
                .Include(e => e.Empresa)
                .Select(e => new
                {
                    id = e.Id,
                    nombreCompleto = e.NombreEmpleado + " " + e.ApellidoEmpleado,
                    empresaAnterior = e.Empresa != null ? e.Empresa.NombreEmpresa : "Sin empresa",
                    estado = e.Estado
                })
                .ToListAsync();

            return Ok(empleados);
        }
    }
        
    }

