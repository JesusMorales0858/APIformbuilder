using APIformbuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using BCrypt.Net;
namespace APIformbuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroController : ControllerBase
    {
        private readonly string cadenaSQL;
		public RegistroController(IConfiguration config)
		{
			cadenaSQL = config.GetConnectionString("CadenaSQL");
		}


		[HttpPost]
        [Route("nuevoUsuario")]
        public IActionResult Registro([FromBody] registroUsuario nuevoUsuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Establece el rol como "Administrador"
                    nuevoUsuario.RoleID = (Roles) 1;

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
                        string insertQuery = "INSERT INTO Users (Username, Password, RoleID) " +
                                "VALUES (@Username, @Password, @RoleID)";
                        SqlCommand insertCommand = new SqlCommand(insertQuery, conexion);
                        insertCommand.Parameters.AddWithValue("@Username", nuevoUsuario.Username);
                        insertCommand.Parameters.AddWithValue("@Password", nuevoUsuario.Password);
                        insertCommand.Parameters.AddWithValue("@RoleID", nuevoUsuario.RoleID);

                        insertCommand.ExecuteNonQuery();

                        return Ok(new { mensaje = "Registro exitoso" });
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

		// Función para hashear la contraseña de manera segura
		private string HashPassword(string password)
		{
			// Genera un nuevo hash con una sal aleatoria
			string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
			return hash;
		}   

		
	}
}
