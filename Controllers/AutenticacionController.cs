using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIformbuilder.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace APIformbuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
		private readonly string cadenaSQL;
		private readonly string secretKey;
        public AutenticacionController(IConfiguration config)
        {
             secretKey = config.GetSection("settings").GetSection("secretkey").ToString();
			cadenaSQL = config.GetConnectionString("CadenaSQL");
		}

        [HttpPost]
        [Route("validar")]
        public ActionResult validar ([FromBody] LogUsuario request) {

			bool usuarioValido = ValidarUsuario(request.Username, request.Password);

			if (usuarioValido)
			{
				var keyBytes = Encoding.ASCII.GetBytes(secretKey);
				var claims = new ClaimsIdentity();
				claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Username));
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = claims,
					Expires = DateTime.UtcNow.AddMinutes(30),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
				};
				var tokenHandler = new JwtSecurityTokenHandler();
				var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
				string tokenCreado = tokenHandler.WriteToken(tokenConfig);
				var NombreUsuario = request.Username;
				return StatusCode(StatusCodes.Status200OK, new { 
					token = tokenCreado,
				    usuario = NombreUsuario});
			}
			else
			{
				return StatusCode(StatusCodes.Status401Unauthorized, new { mensaje = "El usuario es incorrecto o no existe" });
			}
		}
		private bool ValidarUsuario(string username, string password)
		{
			using (var conexion = new SqlConnection(cadenaSQL))
			{
				conexion.Open();

				string query = "SELECT Username, Password FROM Users WHERE Username = @Username";
				using (var command = new SqlCommand(query, conexion))
				{
					command.Parameters.AddWithValue("@Username", username);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							string storedPasswordHash = reader["Password"].ToString(); // Suponemos que la contraseña se almacena como hash

							// Aquí debes verificar si la contraseña proporcionada coincide con el hash almacenado
							// Utiliza la función VerifyPassword que mencioné anteriormente para comparar la contraseña proporcionada con el hash almacenado
							if (VerifyPassword(password, storedPasswordHash))
							{
								return true; // La autenticación fue exitosa
							}
						}
					}
				}
			}

			return false; // El usuario no existe o la contraseña no coincide
		}
		private bool VerifyPassword(string password, string hash)
		{
			return BCrypt.Net.BCrypt.Verify(password, hash);
		}

	}
}
