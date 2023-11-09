namespace APIformbuilder.Models
{
    public class PermisosModel
    {
        public int Id { get; set; }
        public int usuarioId { get; set; }
        public int permisoId { get; set; }

    }

    public class EliminarPermisosRequest
    {
        public int UsuarioId { get; set; }
        public int PermisoId { get; set; }
    }


    public class PermisosListado
    {
        public int permisosID { get; set; }
        public int usuarioId { get; set; }
        public int funcionId { get; set; }
        public string descripcion { get; set; }
        public string Username { get; set;}
        public string roleId { get; set; }
        public DateTime? usuarioEliminado { get;set;}
    }

    public class PermisosListadoModal
    {
        public int permisosID { get; set; }
        public int usuarioId { get; set; }
        public int funcionId { get; set; }
        public string descripcion { get; set; }
    }
    public class ResultadoPermisos
    {
        public int PermisosID { get; set; }
        public int UsuarioId { get; set; }
        public int FuncionId { get; set; }
        public string Descripcion { get; set; }
        public string Username { get; set; }
        public string RoleID { get; set; }
        public DateTime? UsuarioEliminado { get; set; }
    }
    public class AsignacionPermiso
    {
        public int UsuarioId { get; set; }
        public int PermisoId { get; set; }
    }

    public class AsignacionPermisosRequest
    {
        public int UsuarioId { get; set; }
        public List<int> PermisoIds { get; set; }
    }
}
    