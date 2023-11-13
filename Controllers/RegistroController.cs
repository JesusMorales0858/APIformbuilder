using APIformbuilder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
					nuevoUsuario.RoleID = (Roles)1;

					using (var conexion = new SqlConnection(cadenaSQL))
					{
						conexion.Open();
						// Verificar si hay un usuario con RoleID = 1
						string adminCheckQuery = "SELECT COUNT(*) FROM Users WHERE RoleID = 1";
						SqlCommand adminCheckCommand = new SqlCommand(adminCheckQuery, conexion);
						int adminCount = (int)adminCheckCommand.ExecuteScalar();
						if (adminCount > 0 && nuevoUsuario.RoleID == Roles.Administrador)
						{
							return BadRequest("Ya existe un usuario con RoleID = 1. No se permite el registro.");
						}

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
												"VALUES (@Username, @Password, @RoleID); SELECT SCOPE_IDENTITY();";
						SqlCommand insertCommand = new SqlCommand(insertQuery, conexion);
						insertCommand.Parameters.AddWithValue("@Username", nuevoUsuario.Username);
						insertCommand.Parameters.AddWithValue("@Password", nuevoUsuario.Password);
						insertCommand.Parameters.AddWithValue("@RoleID", nuevoUsuario.RoleID);

						int userId = Convert.ToInt32(insertCommand.ExecuteScalar());

						// Asignar permisos del 1 al 12 al nuevo usuario
						for (int i = 1; i <= 12; i++)
						{
							string assignPermissionQuery = "INSERT INTO permisos (usuarioId, permisoId) " +
															"VALUES (@UsuarioId, @PermisoId)";
							SqlCommand assignPermissionCommand = new SqlCommand(assignPermissionQuery, conexion);
							assignPermissionCommand.Parameters.AddWithValue("@UsuarioId", userId);
							assignPermissionCommand.Parameters.AddWithValue("@PermisoId", i);

							assignPermissionCommand.ExecuteNonQuery();
						}

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
