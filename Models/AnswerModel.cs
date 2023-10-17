namespace APIformbuilder.Models
{
    public class AnswerModel
    {
        public int? Id_Answer { get; set; }
        public int? Id_ConfigForm { get; set; }
        public int? Id_Field { get; set; }
        public string? valor { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public DateTime? fecha_eliminacion { get; set; }

    }

    public class AnswerEdit
    {
        public int? Id_Answer { get; set; }
        public int? Id_ConfigForm { get; set; }
        public int? Id_Field { get; set; }
        public string? valor { get; set; }
        public DateTime? fecha_modificacion { get; set; }
        public DateTime? fecha_eliminacion { get; set; }

    }
    public class AnswerErase
    {
        public int? Id_Answer { get; set; }
        public DateTime? fecha_eliminacion { get; set; }

    }
}
