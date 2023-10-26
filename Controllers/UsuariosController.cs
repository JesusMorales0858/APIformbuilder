using APIformbuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;


namespace APIformbuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly string cadenaSQL;
        public UsuariosController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }
        //***************LISTAR Usuarios**************
        [HttpGet]
        [Route("Usuarios/{UserId:int}")]
        public IActionResult ListaUsuarios(int UserId)
        {
            List<RespuestasLista> lista = new List<RespuestasLista>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarRespuestas", conexion);
                    //    cmd.Parameters.AddWithValue("@FormularioID", IdConfigForm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new RespuestasLista()
                            {
                                Id_ConfigForm = Convert.ToInt32(rd["Id_ConfigForm"]),
                                Id_Field = Convert.ToInt32(rd["Id_Field"]),
                                Id_Answer = Convert.ToInt32(rd["Id_Answer"]),
                                nombre = rd["nombre"].ToString(),
                                valor = rd["valor"].ToString(),
                                identificador_fila = Convert.ToInt32(rd["identificador_fila"])
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
        //*****************************************************************************************************

    }


}
