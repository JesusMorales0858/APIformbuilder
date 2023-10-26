using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace APIformbuilder.Models
{
    public class Field
    {
        public int Id_Field { get; set; }
        public string nombre { get; set; }
        public int? orden { get; set; }
        public string etiqueta { get; set; }
        public string tipo { get; set;}
        public int? requerido { get; set; }
        public string? marcador { get; set; }
        public string opciones { get; set; }
        public int? visible { get; set; }
        public string clase { get; set; }
        public int? estado { get; set; }
        public int Id_ConfigForm { get; set;}
        [Display(Name = "Fecha de Eliminación")]
        public DateTime? fecha_eliminacion { get; set; }

    }
}
