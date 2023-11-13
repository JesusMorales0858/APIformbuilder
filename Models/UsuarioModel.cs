namespace APIformbuilder.Models
{
    public class UsuarioModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int RoleID { get; set; } 
        public DateTime? fecha_eliminacion { get; set; }
    }

    public class RolesModel
    {
        public int RolID { get; set; }
        public string RolName { get; set; }
    }

}
