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
    }
}
