namespace APIformbuilder.Models
{
    public class PermisosModel
    {
        public int Id { get; set; }
        public int usuarioId { get; set; }
        public int permisoId { get; set; }

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
}
