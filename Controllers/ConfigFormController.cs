using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.SqlClient;
using APIformbuilder.Models;
using Microsoft.AspNetCore.Cors;
using Dapper;
using Microsoft.Extensions.Configuration;

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
                return StatusCode(StatusCodes.Status200OK, new { message = "eliminado" });
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
                return StatusCode(StatusCodes.Status200OK, new { datosForm = formulariodata, datosField = campodata });
            }
            catch (Exception erx)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = erx.Message });
            }
        }
        //*****************************************************************************************************

        //***************GUARDAR FORMULARIO COMPLT**************

        [HttpPost]
        [Route("GuardarFormularioCreado")]
        public IActionResult GuardarFormularioCampos(ConfigForm Field)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        var insertConfigFormSql = "INSERT INTO ConfigForm (Titulo, Descripcion, Fecha_Creacion) VALUES (@Titulo, @Descripcion, @Fecha_Creacion); SELECT SCOPE_IDENTITY();";
                        int configFormId = conexion.QuerySingle<int>(insertConfigFormSql, new
                        {
                            Titulo = Field.Titulo,
                            Descripcion = Field.Descripcion,
                            Fecha_Creacion = DateTime.Now
                        }, transaction);

                        foreach (var fieldInput in Field.Campos)
                        {
                            var insertFieldSql = "INSERT INTO Field (nombre, orden, etiqueta, tipo, requerido, marcador, opciones, visible, clase, estado, Id_ConfigForm, fecha_eliminacion) VALUES (@nombre, @orden, @etiqueta, @tipo, @requerido, @marcador, @opciones, @visible, @clase, @estado, @Id_ConfigForm, @fecha_eliminacion);";
                            conexion.Execute(insertFieldSql, new
                            {
                                nombre = fieldInput.nombre,
                                orden = fieldInput.orden,
                                etiqueta = fieldInput.etiqueta,
                                tipo = fieldInput.tipo,
                                requerido = fieldInput.requerido,
                                marcador = fieldInput.marcador,
                                opciones = fieldInput.opciones,
                                visible = fieldInput.visible,
                                clase = fieldInput.clase,
                                estado = fieldInput.estado,
                                Id_ConfigForm = configFormId,
                                fecha_eliminacion = fieldInput.fecha_eliminacion
                            }, transaction);
                        }

                        transaction.Commit();

                        // Devuelve el ID generado como respuesta HTTP 200 (éxito)
                        return Ok(new { Id = configFormId });
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
		//***************LISTAR RESPUESTAS**************
		[HttpGet]
		[Route("ListaRespuestas/{IdConfigForm:int}")]
		public IActionResult ListaRespuesta(int IdConfigForm)
		{
			List<RespuestasLista> lista = new List<RespuestasLista>();
			try
			{
				using (var conexion = new SqlConnection(cadenaSQL))
				{
					conexion.Open();
					var cmd = new SqlCommand("ListarRespuestas", conexion);
					cmd.Parameters.AddWithValue("@Id_Formulario", IdConfigForm);
					cmd.CommandType = CommandType.StoredProcedure;
					using (var rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							lista.Add(new RespuestasLista()
							{
								nombre = rd["nombre"].ToString(),
								valor = rd["valor"].ToString(),
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

        //***************GUARDAR RESPUESTA***************************************


        [HttpPost]
        [Route("Respuestas")]

        public async Task<IActionResult> GuardarRespuesta([FromBody] AnswerModel Answer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO Answer (Id_ConfigForm, Id_Field, valor, fecha_creacion, fecha_modificacion) VALUES (@Id_ConfigForm, @Id_Field, @valor, @fecha_creacion, @fecha_modificacion)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id_ConfigForm", Answer.Id_ConfigForm);
                        command.Parameters.AddWithValue("@Id_Field", Answer.Id_Field);
                        command.Parameters.AddWithValue("@valor", Answer.valor);
                        command.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                        command.Parameters.AddWithValue("@fecha_modificacion", DateTime.Now);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Respuesta del formulario guardada exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al guardar la respuesta: {ex.Message}");
            }
        }
        //***************EDITAR RESPUESTA***************************************
        [HttpPut]
        [Route("Respuestas/Editar/{id}")]
        public async Task<IActionResult> EditarRespuesta(int id, [FromBody] AnswerEdit Answer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE Answer SET Id_ConfigForm = @Id_ConfigForm, Id_Field = @Id_Field, valor = @valor, fecha_modificacion = @fecha_modificacion WHERE Id_Answer = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Id_ConfigForm", Answer.Id_ConfigForm);
                        command.Parameters.AddWithValue("@Id_Field", Answer.Id_Field);
                        command.Parameters.AddWithValue("@valor", Answer.valor);
                        command.Parameters.AddWithValue("@fecha_modificacion", DateTime.Now);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Respuesta del formulario editada exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al editar la respuesta: {ex.Message}");
            }
        }
        //***************ELIMINAR RESPUESTA***************************************
        [HttpPut]
        [Route("Respuestas/Eliminar/{id_fila}")]
        public async Task<IActionResult> EliminarRespuesta(int id_fila, AnswerErase Answer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cadenaSQL))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE Answer SET fecha_eliminacion = @fecha_eliminacion WHERE identificador_fila = @id_fila";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_fila", id_fila);
                        command.Parameters.AddWithValue("@fecha_eliminacion", DateTime.Now);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Respuesta del formulario eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al editar la respuesta: {ex.Message}");
            }
        }


    }
}





