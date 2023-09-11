using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.SqlClient;
using APIformbuilder.Models;
using Microsoft.AspNetCore.Cors;

namespace APIformbuilder.Controllers
{
    [EnableCors("ReglasCorse")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigFormController : ControllerBase
    {
        private readonly string cadenaSQL;
        public ConfigFormController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }


        //***************LISTAR FORMULARIOS MENU**************
        [HttpGet]
        [Route("ListaFormulariosMenu")]
        public IActionResult ListaFormMenu()
        {
            List<ConfigFormMenu> lista = new List<ConfigFormMenu>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerFormulariosMenu", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new ConfigFormMenu()
                            {
                                IdConfigForm = Convert.ToInt32(rd["ID"]),
                                Titulo = rd["Titulo_Formulario"].ToString(),
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        //*****************************************************************************************************

        //***************LISTAR FORMULARIOS CRUD**************
        [HttpGet]
        [Route("ListaFormulariosCRUD")]
        public IActionResult ListaFormCRUD()
        {
            List<ConfigFormCRUD> lista = new List<ConfigFormCRUD>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerFormulariosCRUD", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new ConfigFormCRUD()
                            {
                                IdConfigForm = Convert.ToInt32(rd["Id_ConfigForm"]),
                                Titulo = rd["titulo"].ToString(),
                                Descripcion = rd["descripcion"].ToString()
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        //*****************************************************************************************************

        //***************MARCAR FORMULARIO COMO ELIMINADO**************
        [HttpPut]
        [Route("EliminarModulo/{IdConfigForm:int}")]
        public IActionResult EliminarModulo(int IdConfigForm)//Actualizar el campo fecha_eliminacion
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("MarcarFormularioComoEliminado", conexion);
                    cmd.Parameters.AddWithValue("@Id_Formulario", IdConfigForm);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();


                }
                return StatusCode(StatusCodes.Status200OK, new { message ="eliminado" });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }
        }
        //*****************************************************************************************************

        //***************MOSTRAR FORMULARIO COMPLT**************
        [HttpGet]
        [Route("MostrarFormularioCompleto/{IdConfigForm:int}")]
        public IActionResult MostrarFormularios(int IdConfigForm)//Mostrar formulario completo segun su id
        {
            ConfigFormCRUD formulariodata = new ConfigFormCRUD();
            List<Field> campodata = new List<Field>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerFormularioYCampos", conexion);
                    cmd.Parameters.AddWithValue("@Id_Formulario", IdConfigForm);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // cmd.ExecuteNonQuery();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            formulariodata.IdConfigForm = reader.GetInt32(reader.GetOrdinal("Id_ConfigForm"));
                            formulariodata.Titulo = reader.GetString(reader.GetOrdinal("TituloFormulario"));
                            formulariodata.Descripcion = reader.GetString(reader.GetOrdinal("DescripcionFormulario"));
                            //formulariodata.FechaCreacionFormulario = reader.GetDateTime(reader.GetOrdinal("FechaCreacionFormulario"));
                            //formulariodata.FechaModificacionFormulario = reader.GetDateTime(reader.GetOrdinal("FechaModificacionFormulario"));

                        }
                        reader.NextResult();//ir al siguiente resultado (field)
                        while (reader.Read())
                        {
                            Field campo = new Field();
                            campo.Id_Field = reader.GetInt32(reader.GetOrdinal("Id_Field"));
                            campo.nombre = reader.GetString(reader.GetOrdinal("NombreCampo"));
                            campo.orden = reader.GetInt32(reader.GetOrdinal("OrdenCampo"));
                            campo.etiqueta = reader.GetString(reader.GetOrdinal("EtiquetaCampo"));
                            campo.tipo = reader.GetString(reader.GetOrdinal("TipoCampo"));
                            campo.requerido = reader.GetInt32(reader.GetOrdinal("RequeridoCampo"));
                            campo.marcador = reader.GetString(reader.GetOrdinal("MarcadorCampo"));
                           // campo.opciones = reader.GetString(reader.GetOrdinal("OpcionesCampo"));
                            campo.visible = reader.GetInt32(reader.GetOrdinal("VisibleCampo"));
                            campo.clase = reader.GetString(reader.GetOrdinal("ClaseCampo"));
                            campo.estado = reader.GetInt32(reader.GetOrdinal("EstadoCampo"));

                            campodata.Add(campo);
                        }

                    }


                }
                //formulariodata.Campos = campodata;
                //return formulariodata;
                return StatusCode(StatusCodes.Status200OK, new { datosForm=formulariodata, datosField=campodata });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }
        }

    }
}
