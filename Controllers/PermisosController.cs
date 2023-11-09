using APIformbuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace APIformbuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly string cadenaSQL;
        public PermisosController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        //***************LISTAR USURIOS CON PERMISOS**************
        [HttpGet]
        [Route("ListaPermisosUsuarios")]
        public IActionResult ListaPermisosUsers()
        {
            List<PermisosListado> lista = new List<PermisosListado>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListadoPermisoCompleto", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new PermisosListado() 
                            {
                                permisosID = Convert.ToInt32(rd["permisosID"]),
                                usuarioId = Convert.ToInt32(rd["usuarioId"]),
                                funcionId = Convert.ToInt32(rd["funcionId"]),
                                descripcion = rd["descripcion"].ToString(),
                                Username = rd["Username"].ToString(),
                                usuarioEliminado = rd["usuario_eliminado"] as DateTime ?,
                                roleId = rd["roleId"].ToString()

                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        /*Eliminar permisos*/
        [HttpDelete]
        [Route("EliminarPermisos")]
        public IActionResult EliminarPermisos([FromBody] List<EliminarPermisosRequest> permisosAEliminar)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    connection.Open();

                    foreach (var permiso in permisosAEliminar)
                    {
                        using (SqlCommand command = new SqlCommand("EliminarPermisoDeUsuario", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UsuarioId", permiso.UsuarioId);
                            command.Parameters.AddWithValue("@PermisoId", permiso.PermisoId);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return Ok("Permisos eliminados exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar los permisos: {ex.Message}");
            }
        }
        //***************LISTAR USURIOS CON PERMISOS**************
        [HttpGet]
        [Route("ObtenerPermisosUsuario/{usuarioId}")]
        public IActionResult ObtenerPermisosUsuario(int usuarioId)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListadoPermisoCompleto", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    Dictionary<string, List<PermisosListadoModal>> resultado = new Dictionary<string, List<PermisosListadoModal>>();

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            if (Convert.ToInt32(rd["usuarioId"]) == usuarioId)
                            {
                                string usuarioKey = "usuario"; // Cambiado a un valor estático

                                if (!resultado.ContainsKey(usuarioKey))
                                {
                                    resultado[usuarioKey] = new List<PermisosListadoModal>();
                                }

                                resultado[usuarioKey].Add(new PermisosListadoModal()
                                {
                                    permisosID = Convert.ToInt32(rd["permisosID"]),
                                    usuarioId = Convert.ToInt32(rd["usuarioId"]),
                                    funcionId = Convert.ToInt32(rd["funcionId"]),
                                    descripcion = rd["descripcion"].ToString(),
                                });
                            }
                        }
                    }

                    return StatusCode(StatusCodes.Status200OK, resultado);
                }
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        /***********/
        [HttpPost]
        [Route("AsignarPermisos")]
        public IActionResult AsignarPermisos([FromBody] List<AsignacionPermisosRequest> asignaciones)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    connection.Open();

                    foreach (var asignacion in asignaciones)
                    {
                        foreach (var permisoId in asignacion.PermisoIds)
                        {
                            using (SqlCommand command = new SqlCommand("AsignarPermisoAUsuario", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@UsuarioId", asignacion.UsuarioId);
                                command.Parameters.AddWithValue("@PermisoId", permisoId);

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return Ok("Permisos asignados exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar los permisos: {ex.Message}");
            }
        }



    }
}
