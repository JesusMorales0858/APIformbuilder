using System.ComponentModel.DataAnnotations;

namespace APIformbuilder.Models
{
	public class LogUsuario
	{
		[Required(ErrorMessage = "El campo 'Username' es obligatorio.")]
		public string Username { get; set; }

		[Required(ErrorMessage = "El campo 'Password' es obligatorio.")]
		public string Password { get; set; }

		public int UserID { get; set; }
		public int permisoId { get; set; }

		public int RoleID { get; set; }
	}

}

