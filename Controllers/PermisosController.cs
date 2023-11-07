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
        /*Buscar permisos por usuario*/
        [HttpGet]
        public ActionResult<IEnumerable<ResultadoPermisos>> ObtenerPermisosPorUsuario(int userID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ListadoPermisosXUser", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userID);

                        var resultados = new List<ResultadoPermisos>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultados.Add(new ResultadoPermisos
                                {
                                    PermisosID = (int)reader["permisosID"],
                                    UsuarioId = (int)reader["usuarioId"],
                                    FuncionId = (int)reader["permisoId"],
                                    Descripcion = reader["descripcion"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    RoleID = reader["RoleID"].ToString(),
                                    UsuarioEliminado = reader["usuario_eliminado"] as DateTime?
                                });
                            }
                        }

                        return Ok(resultados);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener permisos por usuario: {ex.Message}");
            }
        }
        /*Asignar permiso*/
        [HttpPost]
        public IActionResult AsignarPermiso([FromBody] AsignacionPermiso asignacion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("AsignarPermisoAUsuario", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UsuarioId", asignacion.UsuarioId);
                        command.Parameters.AddWithValue("@PermisoId", asignacion.PermisoId);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Permiso asignado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al asignar el permiso: {ex.Message}");
            }
        }
        /*Eliminar permisos*/
        [HttpDelete]
        public IActionResult EliminarPermiso(int usuarioId, int permisoId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("EliminarPermisoDeUsuario", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        command.Parameters.AddWithValue("@PermisoId", permisoId);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Permiso eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el permiso: {ex.Message}");
            }
        }

    }
}
