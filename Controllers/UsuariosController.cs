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
                                RoleID = Convert.ToInt32(rd["RoleID"]),
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

		//guardar nuevos usuarios
		[HttpPost]
		[Route("guardarUsuario")]
		public IActionResult agregarUsuario([FromBody] registroUsuario nuevoUsuario)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var conexion = new SqlConnection(cadenaSQL))
					{
						conexion.Open();

						// Verificar si el nombre de usuario ya existe en la base de datos
						string userCheckQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
						SqlCommand userCheckCommand = new SqlCommand(userCheckQuery, conexion);
						userCheckCommand.Parameters.AddWithValue("@Username", nuevoUsuario.Username);

						int existingUsers = (int)userCheckCommand.ExecuteScalar();
						if (existingUsers > 0)
						{
							return BadRequest("El nombre de usuario ya está en uso.");
						}

						// Hash y salting de la contraseña (debes implementarlo de manera segura)
						// Aquí asumimos que tienes una función de hash y salting llamada HashPassword
						nuevoUsuario.Password = HashPassword(nuevoUsuario.Password);

						// Insertar el nuevo usuario en la base de datos
						string insertUserQuery = "INSERT INTO Users (Username, Password, RoleID) VALUES (@Username, @Password, @Rol); SELECT SCOPE_IDENTITY()";
						SqlCommand insertUserCommand = new SqlCommand(insertUserQuery, conexion);
						insertUserCommand.Parameters.AddWithValue("@Username", nuevoUsuario.Username);
						insertUserCommand.Parameters.AddWithValue("@Password", nuevoUsuario.Password);
						insertUserCommand.Parameters.AddWithValue("@Rol", 2);
						int userId = Convert.ToInt32(insertUserCommand.ExecuteScalar());

						// Actualizar los permisos con el ID del usuario
						foreach (var permiso in nuevoUsuario.Permisos)
						{
							Console.WriteLine($"Usuario ID: {userId}, Permiso ID: {permiso.permisoId}");
							string insertPermisoQuery = "INSERT INTO permisos (usuarioId, permisoId) VALUES (@UsuarioId, @PermisoId)";
							SqlCommand insertPermisoCommand = new SqlCommand(insertPermisoQuery, conexion);
							insertPermisoCommand.Parameters.AddWithValue("@UsuarioId", userId);
							insertPermisoCommand.Parameters.AddWithValue("@PermisoId", permiso.permisoId);
							insertPermisoCommand.ExecuteNonQuery();
						}


						return Ok(new { userId });
					}
				}
				else
				{
					// Si la validación falla, se devuelven los errores de validación
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}



		//***********
		//******ObtenerUsuarioPorID
		[HttpGet]
		[Route("ObtenerUsuarioPorID/{UserID:int}")]
		public IActionResult ObtenerUsuario(int UserID)
		{
			try
			{
				using (var conexion = new SqlConnection(cadenaSQL))
				{
					conexion.Open();
					var cmd = new SqlCommand("ObtenerUsuarioPorID", conexion);
					cmd.Parameters.AddWithValue("@UserID", UserID);
					cmd.CommandType = CommandType.StoredProcedure;

					using (var rd = cmd.ExecuteReader())
					{
						if (rd.Read())
						{
							var usuario = new UsuarioModel()
							{
								UserID = Convert.ToInt32(rd["UserID"]),
								Username = rd["Username"].ToString(),
								//Password = rd["Password"].ToString(),
								//RoleID = rd["RoleID"].ToString()
							};
							return StatusCode(StatusCodes.Status200OK, new { usuario });
						}
						else
						{
							return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario no encontrado" });
						}
					}
				}
			}
			catch (Exception error)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
			}
		}
		//******
		//editarusuariosguardados
		//********Editar Usuario y permisos
		[HttpPut]
		[Route("editarUsuario/{userId}")]
		public IActionResult editarUsuario(int userId, [FromBody] registroUsuario usuarioActualizado)
		{
			try
			{
				if (ModelState.IsValid)
				{
					using (var conexion = new SqlConnection(cadenaSQL))
					{
						conexion.Open();

						// verificar si el usuario ingresado existe
						string userExistsQuery = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID";
						SqlCommand userExistsCommand = new SqlCommand(userExistsQuery, conexion);
						userExistsCommand.Parameters.AddWithValue("@UserID", userId);

						int userCount = (int)userExistsCommand.ExecuteScalar();
						if (userCount == 0)
						{
							return BadRequest("Usuario no encontrado.");
						}

						// Actualizar el nombre de usuario
						if (!string.IsNullOrEmpty(usuarioActualizado.Username))
						{
							string updateUsernameQuery = "UPDATE Users SET Username = @Username WHERE UserID = @UserID";
							SqlCommand updateUsernameCommand = new SqlCommand(updateUsernameQuery, conexion);
							updateUsernameCommand.Parameters.AddWithValue("@Username", usuarioActualizado.Username);
							updateUsernameCommand.Parameters.AddWithValue("@UserID", userId);
							updateUsernameCommand.ExecuteNonQuery();
						}

						// Actualizar contraseña si es que se da
						if (!string.IsNullOrEmpty(usuarioActualizado.Password))
						{
							// Hash y salting de la nueva contraseña
							usuarioActualizado.Password = HashPassword(usuarioActualizado.Password);

							string updatePasswordQuery = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
							SqlCommand updatePasswordCommand = new SqlCommand(updatePasswordQuery, conexion);
							updatePasswordCommand.Parameters.AddWithValue("@Password", usuarioActualizado.Password);
							updatePasswordCommand.Parameters.AddWithValue("@UserID", userId);
							updatePasswordCommand.ExecuteNonQuery();
						}



						// Actualizar permisos
						string deletePermisosQuery = "DELETE FROM permisos WHERE usuarioId = @UserID";
						SqlCommand deletePermisosCommand = new SqlCommand(deletePermisosQuery, conexion);
						deletePermisosCommand.Parameters.AddWithValue("@UserID", userId);
						deletePermisosCommand.ExecuteNonQuery();

						foreach (var permiso in usuarioActualizado.Permisos)
						{
							string insertPermisoQuery = "INSERT INTO permisos (usuarioId, permisoId) VALUES (@UsuarioId, @PermisoId)";
							SqlCommand insertPermisoCommand = new SqlCommand(insertPermisoQuery, conexion);
							insertPermisoCommand.Parameters.AddWithValue("@UsuarioId", userId);
							insertPermisoCommand.Parameters.AddWithValue("@PermisoId", permiso.permisoId);
							insertPermisoCommand.ExecuteNonQuery();
						}

						return Ok("Usuario actualizado exitosamente.");
					}
				}
				else
				{
					// Si la validación falla, se devuelven los errores de validación
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error interno del servidor: {ex.Message}");
			}
		}

		//************
		//******


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
