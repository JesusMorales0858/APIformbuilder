using System.ComponentModel.DataAnnotations;


namespace APIformbuilder.Models
{
	public class registroUsuario
	{
		public int UserID { get; set; }

		[Required(ErrorMessage = "El campo Username es obligatorio.")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "El campo Username debe tener entre 2 y 50 caracteres.")]
		public string Username { get; set; }

		//[Required(ErrorMessage = "El campo Password es obligatorio.")]
		[MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
		public string Password { get; set; }

		//[Required(ErrorMessage = "El campo RoleID es obligatorio.")]
		public Roles RoleID { get; set; }

		public List<permisos> Permisos { get; set; }

	}
	public class permisos
	{
		public int id { get; set; }
		public int usuarioId { get; set; }
		public int permisoId { get; set; }
	}


}