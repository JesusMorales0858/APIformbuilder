using APIformbuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using APIformbuilder.Controllers;


namespace APIformbuilder.Controllers
{

    [Route("api/[controller]/")]
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
        public IActionResult ListaUsuarios()
        {
            List<UsuarioModel> lista = new List<UsuarioModel>();
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
                            lista.Add(new UsuarioModel()
                            {
                                UserID = Convert.ToInt32(rd["UserID"]),
                                Username = rd["Username"].ToString(),
                                RoleID = rd["RoleID"].ToString(),
                                RolName = rd["RolName"].ToString(),
                                fecha_eliminacion = rd["fecha_eliminacion"] as DateTime?
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
        /**************EDITAR USUARIOS*********/
        
        [HttpPut]
        [Route("EditarContraseña/{UserID:int}")]
        public IActionResult Editar( int UserID, [FromBody] string Password)//Actualizar el campo fecha_eliminacion
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    Password = HashPassword(Password);
                    conexion.Open();
                    var cmd = new SqlCommand("ActualizarPasswordUsuario", conexion);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();


                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Nueva Contraseña Ingresada" });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }



        }
        private string HashPassword(string password)
        {
            // Genera un nuevo hash con una sal aleatoria
            string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
            return hash;
        }

        /********EDITAR USERNAME USUARIOS***********/

        [HttpPut]
        [Route("EditarNombre/{UserID:int}")]
        public IActionResult EditarNomUsuario(int UserID, [FromBody] string username)//Actualizar el campo fecha_eliminacion
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {   
                    
                    conexion.Open();
                    var cmd = new SqlCommand("ActualizarUsernameUsuario", conexion);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();


                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Nuevo Nombre de Usuario Ingresado" });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }
        }



        /***********ELIMINAR USUARIO************/
        [HttpPut]
        [Route("Eliminar/{UserID:int}")]
        public IActionResult EliminarUsuarios(int UserID)//Actualizar el campo fecha_eliminacion
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("EliminarUsuarios", conexion);
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();


                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Eliminado " });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }
        }


    }


}
