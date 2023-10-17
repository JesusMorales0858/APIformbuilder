using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace APIformbuilder.Models
{
	public class RespuestasLista

	{
		public int Id_ConfigForm { get; set; }

		public int Id_Field { get; set; }

		public int Id_Answer { get; set; } 

		public string nombre { get; set; }

		public string valor { get; set; }

		public int identificador_fila { get; set; }
	}
}
