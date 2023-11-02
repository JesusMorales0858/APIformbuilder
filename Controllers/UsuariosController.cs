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
        [Route("ListaUsuarios")]
        public IActionResult ListaFormCRUD()
        {
            List<UsuarioListaModel> lista = new List<UsuarioListaModel>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListaUsers", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new UsuarioListaModel()
                            {
                                UserID = Convert.ToInt32(rd["UserID"]),
                                Username = rd["Username"].ToString(),
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

        //***************Editar RESPUESTA***************************************
        [HttpPut]
        [Route("Editar/{UserID}")]
        public async Task<IActionResult> EditarUsuario(int UserID, [FromBody] UsuarioModel UsuarioModel)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    await connection.OpenAsync();

                    // Verificar si el usuario existe antes de la actualización
                    string ListaUsuarioQuery = "SELECT COUNT(*) FROM User WHERE UserID = @UserID";
                    using (SqlCommand verificaUsuarioCmd = new SqlCommand(ListaUsuarioQuery, connection))
                    {
                        verificaUsuarioCmd.Parameters.AddWithValue("@UserID", UserID);
                        int userCount = (int)await verificaUsuarioCmd.ExecuteScalarAsync();
                        if (userCount == 0)
                        {
                            return NotFound("Usuario no encontrado.");
                        }
                    }

                    // Realizar la actualización
                    string query = "UPDATE User SET Username = @Username, Password = @Password, RoleID = @RoleID WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@Username", UsuarioModel.Username);
                        command.Parameters.AddWithValue("@Password", UsuarioModel.Password);
                        command.Parameters.AddWithValue("@RoleID", UsuarioModel.RoleID);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al editar el usuario: {ex.Message}");
            }
        }
    }


}
