using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace APIformbuilder.Models
{
    public class ConfigForm
    {
        public int IdConfigForm { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime Fecha_Creacion { get; set; }

        [Display(Name = "Fecha de Modificación")]
        public DateTime Fecha_Modificacion { get; set; }

        [Display(Name = "Fecha de Eliminación")]
        public DateTime? Fecha_Eliminacion { get; set; }
    }
    //Clase ConfigFormListaMenu proyeccion
    public class ConfigFormMenu
    {
        public int IdConfigForm { get; set; }
        public string Titulo { get; set; }
    }

    //Clase ConfigFormListaCRUD proyeccion
    public class ConfigFormCRUD
    {
        public int IdConfigForm { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }

    //Clase MostrarFormularios proyeccion
    // public class MostrarFormularios 
    //{
    //  public int IdConfigForm { get; set; }
    //  public string Titulo { get; set; }
    //  public string Descripcion { get; set; }
    //  }
}
