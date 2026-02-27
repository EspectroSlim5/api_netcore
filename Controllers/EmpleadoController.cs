using api_netcore.Data;
using api_netcore.DTOs;
using api_netcore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace api_netcore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmpleadoController(AppDbContext db) : ControllerBase
    {
        [HttpGet("mostrar")]
        public async Task<ActionResult<List<object>>> GetAll()
        {
            try
            {
                var empleados = await db.Tbl_Empleado
                    .Include(e => e.Empresa)
                    .Select(e => new
                    {
                        id = e.Id,
                        nombre = e.NombreEmpleado,
                        apellido = e.ApellidoEmpleado,
                        
                        direccion = e.Direccion,
                        telefono = e.Telefono,
                        correo = e.Correo,
                        fechanacimiento = e.FechaNacimiento,
                        fechaingreso = e.FechaIngreso,
                        empresa = e.Empresa != null ? new
                        {
                            id = e.Empresa.Id,
                            nombreempresa = e.Empresa.NombreEmpresa,
                            ruc = e.Empresa.Ruc
                        } : null,
                        estado = e.Estado
                    })
                    .ToListAsync();

                if (empleados.Count == 0)
                    return NotFound(new { message = "No se encontraron empleados registrados" });

                return Ok(empleados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al obtener los empleados: " + ex.Message });
            }
        }



        [HttpGet("buscar/{id:int}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            try
            {
                var empleado = await db.Tbl_Empleado
                    .Include(e => e.Empresa)
                    .FirstOrDefaultAsync(em => em.Id == id);

                if (empleado == null)
                    return NotFound(new { message = "No se encontró el empleado" });

                var resultado = new
                {
                    id = empleado.Id,
                    nombreEmpleado = empleado.NombreEmpleado,
                    apellidoEmpleado = empleado.ApellidoEmpleado,
                    direccion = empleado.Direccion,
                    telefono = empleado.Telefono,
                    correo = empleado.Correo,
                    fechaNacimiento = empleado.FechaNacimiento,
                    fechaIngreso = empleado.FechaIngreso,
                    idEmpresa = empleado.IdEmpresa,
                    estado = empleado.Estado,
                    empresa = empleado.Empresa != null ? new
                    {
                        id = empleado.Empresa.Id,
                        nombreEmpresa = empleado.Empresa.NombreEmpresa
                    } : null
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al obtener el empleado" });
            }
        }


        [HttpPost("crear")]
        [Consumes("application/json")]
        public async Task<ActionResult<object>> Post([FromBody] RegisterEmpleado empleado)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var correoNormalizado = empleado.correo?.Trim().ToLower();

            // Validar que no exista un empleado con el mismo correo
            var empleadoExistente = await db.Tbl_Empleado
                .FirstOrDefaultAsync(em => em.Correo == correoNormalizado);

            if (empleadoExistente != null)
                return BadRequest(new { message = "Ya existe un empleado con el correo proporcionado" });

            // Validar que la empresa exista
            var empresa = await db.Tbl_Empresa.FirstOrDefaultAsync(em => em.Id == empleado.empresa_id);
            if (empresa == null)
                return NotFound(new { message = "No se encontró la empresa proporcionada" });

            try
            {
                var nuevoEmpleado = new Empleado
                {
                    NombreEmpleado = empleado.nombre,
                    ApellidoEmpleado = empleado.apellido,
                    Direccion = empleado.direccion,
                    Telefono = empleado.telefono,
                    Correo = correoNormalizado,
                    FechaNacimiento = empleado.fechanacimiento,
                    IdEmpresa = empresa.Id,
                    Estado = 1
                };

                db.Tbl_Empleado.Add(nuevoEmpleado);
                await db.SaveChangesAsync();

                // Devolver un objeto anónimo sin la relación circular
                var resultado = new
                {
                    id = nuevoEmpleado.Id,
                    nombreEmpleado = nuevoEmpleado.NombreEmpleado,
                    apellidoEmpleado = nuevoEmpleado.ApellidoEmpleado,
                    direccion = nuevoEmpleado.Direccion,
                    telefono = nuevoEmpleado.Telefono,
                    correo = nuevoEmpleado.Correo,
                    fechaNacimiento = nuevoEmpleado.FechaNacimiento,
                    fechaIngreso = nuevoEmpleado.FechaIngreso,
                    idEmpresa = nuevoEmpleado.IdEmpresa,
                    estado = nuevoEmpleado.Estado,
                    empresa = new
                    {
                        id = empresa.Id,
                        nombreEmpresa = empresa.NombreEmpresa
                    }
                };

                return CreatedAtAction(nameof(Get), new { id = nuevoEmpleado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al crear el empleado: " + ex.Message });
            }
        }


        [HttpPut("editar")]
        public async Task<ActionResult<Empleado>> Put(UpdateEmpleado empleado)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var empleadoDB = await db.Tbl_Empleado
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(em => em.Id == empleado.id);

            if (empleadoDB == null)
                return NotFound(new { message = "No se encontró el empleado" });

            // Validar correo único (si se proporcionó un nuevo correo)
            if (!string.IsNullOrEmpty(empleado.correo))
            {
                var correoNormalizado = empleado.correo.Trim().ToLower();
                var empleadoExistente = await db.Tbl_Empleado
                    .FirstOrDefaultAsync(em => em.Correo == correoNormalizado && em.Id != empleado.id);

                if (empleadoExistente != null)
                    return BadRequest(new { message = "Ya existe un empleado con el correo proporcionado" });

                empleadoDB.Correo = correoNormalizado;
            }

            // Validar empresa si se proporcionó
            if (empleado.empresa_id.HasValue)
            {
                var empresa = await db.Tbl_Empresa.FindAsync(empleado.empresa_id.Value);
                if (empresa == null)
                    return NotFound(new { message = "No se encontró la empresa proporcionada" });

                empleadoDB.IdEmpresa = empleado.empresa_id.Value;
            }

            try
            {
                // Actualizar SOLO los campos que NO son null
                if (!string.IsNullOrEmpty(empleado.nombre))
                    empleadoDB.NombreEmpleado = empleado.nombre;

                if (!string.IsNullOrEmpty(empleado.apellido))
                    empleadoDB.ApellidoEmpleado = empleado.apellido;

                if (!string.IsNullOrEmpty(empleado.direccion))
                    empleadoDB.Direccion = empleado.direccion;

                if (!string.IsNullOrEmpty(empleado.telefono))
                    empleadoDB.Telefono = empleado.telefono;

                if (empleado.fechanacimiento.HasValue)
                    empleadoDB.FechaNacimiento = empleado.fechanacimiento;

                db.Tbl_Empleado.Update(empleadoDB);
                await db.SaveChangesAsync();

                // Devolver el empleado actualizado sin ciclo
                var resultado = new
                {
                    id = empleadoDB.Id,
                    nombreEmpleado = empleadoDB.NombreEmpleado,
                    apellidoEmpleado = empleadoDB.ApellidoEmpleado,
                    direccion = empleadoDB.Direccion,
                    telefono = empleadoDB.Telefono,
                    correo = empleadoDB.Correo,
                    fechaNacimiento = empleadoDB.FechaNacimiento,
                    fechaIngreso = empleadoDB.FechaIngreso,
                    idEmpresa = empleadoDB.IdEmpresa,
                    estado = empleadoDB.Estado,
                    empresa = empleadoDB.Empresa != null ? new
                    {
                        id = empleadoDB.Empresa.Id,
                        nombreEmpresa = empleadoDB.Empresa.NombreEmpresa,
                        ruc = empleadoDB.Empresa.Ruc
                    } : null
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error al editar el empleado: " + ex.Message });
            }
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult<Empleado>> Delete(int id)
        {
            var empleado = await db.Tbl_Empleado
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(em => em.Id == id);

            if (empleado == null)
                return NotFound(new { message = "No se encontró el empleado" });

            // En lugar de eliminar, desactivamos
            empleado.Estado = 0;
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Empleado desactivado correctamente",
                id = empleado.Id,
                estado = empleado.Estado
            });
        }

        // activar o desactivar un empleado
        [HttpPut("activar/{id:int}")]
        public async Task<ActionResult<Empleado>> Activar(int id, int estado)
        {
            var empleado = await db.Tbl_Empleado
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(em => em.Id == id);

            if (empleado == null)
                return NotFound(new { message = "No se encontró el empleado" });

            // validar que el estado sea 0 o 1
            if (estado != 0 && estado != 1)
                return BadRequest(new { message = "El estado debe ser 0 o 1" });

            if (empleado.Estado == estado)
                return BadRequest(new { message = "El empleado ya se encuentra en ese estado" });

            empleado.Estado = estado;
            await db.SaveChangesAsync();

            // Devolver información completa sin ciclo
            var resultado = new
            {
                id = empleado.Id,
                nombreEmpleado = empleado.NombreEmpleado,
                apellidoEmpleado = empleado.ApellidoEmpleado,
                estado = empleado.Estado,
                empresa = empleado.Empresa != null ? new
                {
                    id = empleado.Empresa.Id,
                    nombreEmpresa = empleado.Empresa.NombreEmpresa
                } : null
            };

            return Ok(resultado);
        }

        [HttpPost("reactivar/{id:int}")]
        public async Task<ActionResult<Empleado>> Reactivar(int id)
        {
            var empleado = await db.Tbl_Empleado
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(em => em.Id == id);

            if (empleado == null)
                return NotFound(new { message = "No se encontró el empleado" });

            empleado.Estado = 1;
            await db.SaveChangesAsync();

            // La empresa sigue asociada (IdEmpresa no cambia)
            var resultado = new
            {
                id = empleado.Id,
                nombreEmpleado = empleado.NombreEmpleado,
                apellidoEmpleado = empleado.ApellidoEmpleado,
                estado = empleado.Estado,
                idEmpresa = empleado.IdEmpresa,
                empresa = empleado.Empresa != null ? new
                {
                    id = empleado.Empresa.Id,
                    nombreEmpresa = empleado.Empresa.NombreEmpresa
                } : null
            };

            return Ok(resultado);
        }

        
    }
}
