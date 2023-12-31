﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.SqlClient;
using APIformbuilder.Models;
using Microsoft.AspNetCore.Cors;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Microsoft.AspNetCore.Authorization;

namespace APIformbuilder.Controllers
{
    [EnableCors("ReglasCorse")]
    [Route("api/[controller]")]
    [Authorize]
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
                            campo.opciones = reader.GetString(reader.GetOrdinal("OpcionesCampo"));
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
					cmd.Parameters.AddWithValue("@FormularioID", IdConfigForm);
					cmd.CommandType = CommandType.StoredProcedure;
					using (var rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							lista.Add(new RespuestasLista()
							{
                                Id_ConfigForm = Convert.ToInt32(rd["Id_ConfigForm"]),
                                Id_Field = Convert.ToInt32(rd["Id_Field"]),
                                Id_Answer = Convert.ToInt32(rd["Id_Answer"]),
                                nombre = rd["nombre"].ToString(),
								valor = rd["valor"].ToString(),
                                identificador_fila = Convert.ToInt32(rd["identificador_fila"])
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
		[HttpPost]
		[Route("Respuestas/Editar")]
		public IActionResult ActualizarDatos([FromBody] List<AnswerModel> actualizaciones)
		{
			try
			{
				// Obtén la cadena de conexión a la base de datos desde la configuración
				using (var conexion = new SqlConnection(cadenaSQL))
				{
					conexion.Open();

					// Crea una tabla de valores para los datos de actualización
					DataTable actualizacionesTable = new DataTable();
					actualizacionesTable.Columns.Add("Id_Answer", typeof(int));
					actualizacionesTable.Columns.Add("valor", typeof(string));

					foreach (var actualizacion in actualizaciones)
					{
						actualizacionesTable.Rows.Add(actualizacion.Id_Answer, actualizacion.valor);
					}

					using (SqlCommand cmd = new SqlCommand("EditarRegistrosAnswer", conexion))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						// Asigna el parámetro del procedimiento almacenado
						SqlParameter parameter = cmd.Parameters.AddWithValue("@Actualizaciones", actualizacionesTable);
						parameter.SqlDbType = SqlDbType.Structured;

						cmd.ExecuteNonQuery();
					}
				}

				return Ok("Registros actualizados exitosamente");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error: {ex.Message}");
			}
		}
		//*************************************************************
		//***************GUARDAR RESPUESTA***************************************
		[HttpPost]
		[Route("Respuestas/Guardar")]
		public ActionResult<int> GuardarRespuesta([FromBody] List<NuevaRespuestasGuardar> registros)
		{
			try
			{
				using (var conexion = new SqlConnection(cadenaSQL))
				{
					conexion.Open();

					// Obtener el próximo identificador_fila una vez
					int proximoIdentificador = ObtenerProximoIdentificador(conexion);

					foreach (var registro in registros)
					{
						// Construye la consulta SQL para insertar un registro en la tabla Answer
						string query = "INSERT INTO Answer (Id_ConfigForm, Id_Field, valor, fecha_creacion, identificador_fila) " +
									   "VALUES (@Id_ConfigForm, @Id_Field, @valor, GETDATE(), @IdentificadorFila);";

						using (SqlCommand cmd = new SqlCommand(query, conexion))
						{
							cmd.Parameters.AddWithValue("@Id_ConfigForm", registro.Id_ConfigForm);
							cmd.Parameters.AddWithValue("@Id_Field", registro.Id_Field);
							cmd.Parameters.AddWithValue("@valor", registro.valor);

							// Utiliza el mismo valor de identificador_fila para todos los registros
							cmd.Parameters.AddWithValue("@IdentificadorFila", proximoIdentificador);

							cmd.ExecuteNonQuery();
						}
					}

					return Ok(registros.Count);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		private int ObtenerProximoIdentificador(SqlConnection conexion)
		{
			// Obtener el próximo identificador_fila
			string query = "SELECT ISNULL(MAX(identificador_fila), 0) + 1 FROM Answer;";

			using (SqlCommand cmd = new SqlCommand(query, conexion))
			{
				object resultado = cmd.ExecuteScalar();
				return resultado is DBNull ? 1 : (int)resultado;
			}
		}

	
	//******************************************************************
	//****LISTAR RESPUESTAS X IDENTIFICADOR DE FILA
	[HttpGet]
		[Route("ListaRespuestasIdentificadorFila/{IdConfigForm:int}/{identificador_fila:int}")]

		public IActionResult ListaRespuestaIdentificadorFila(int IdConfigForm, int identificador_fila)
		{
			List<RespuestasLista> lista = new List<RespuestasLista>();
			try
			{
				using (var conexion = new SqlConnection(cadenaSQL))
				{
					conexion.Open();
					var cmd = new SqlCommand("ListarRespuestasPorIdentificadorFila", conexion);
					cmd.Parameters.AddWithValue("@FormularioID", IdConfigForm);
					cmd.Parameters.AddWithValue("@IdentificadorFila", identificador_fila);

					cmd.CommandType = CommandType.StoredProcedure;
					using (var rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							lista.Add(new RespuestasLista()
							{
								Id_ConfigForm = Convert.ToInt32(rd["Id_ConfigForm"]),
								Id_Field = Convert.ToInt32(rd["Id_Field"]),
								Id_Answer = Convert.ToInt32(rd["Id_Answer"]),
								nombre = rd["nombre"].ToString(),
								valor = rd["valor"].ToString(),
								identificador_fila = Convert.ToInt32(rd["identificador_fila"])
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

        [HttpPut]
        [Route("EditarFormulario")]
        public IActionResult EditarFormulario(int id, ConfigForm Field)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                conexion.Open();
                using (var transaction = conexion.BeginTransaction())
                {
                    try
                    {
                        // Verifica si el formulario con el ID proporcionado existe
                        var existingForm = conexion.QuerySingleOrDefault<ConfigForm>("SELECT * FROM ConfigForm WHERE Id_ConfigForm = @Id", new { Id = id }, transaction);

                        if (existingForm == null)
                        {
                            // El formulario no existe, devolver un error
                            return NotFound("Formulario no encontrado");
                        }

                        // Actualiza los datos del formulario
                        DateTime fechaModificacion = DateTime.Now;
                        var updateConfigFormSql = "UPDATE ConfigForm SET Titulo = @Titulo, Descripcion = @Descripcion, fecha_modificacion = @fecha_modificacion WHERE Id_ConfigForm = @Id";
                        conexion.Execute(updateConfigFormSql, new
                        {
                            Id = id,
                            Titulo = Field.Titulo,
                            Descripcion = Field.Descripcion,
                            fecha_modificacion = fechaModificacion
                        }, transaction);

                       

                        // Inserta los nuevos campos
                        foreach (var fieldInput in Field.Campos)
                        {
                            var insertFieldSql = "INSERT INTO Field (nombre, orden, etiqueta, tipo, requerido, marcador, opciones, visible, clase, estado, Id_ConfigForm, fecha_eliminacion) " +
                                "VALUES (@nombre, @orden, @etiqueta, @tipo, @requerido, @marcador, @opciones, @visible, @clase, @estado, @Id_ConfigForm, @fecha_eliminacion);";
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
                                Id_ConfigForm = id,
                                fecha_eliminacion = fieldInput.fecha_eliminacion
                            }, transaction);
                        }

                        transaction.Commit();

                        // Devuelve el ID del formulario actualizado como respuesta HTTP 200 (éxito)
                        return Ok(new { Id = id , Estado = "Cargado correctamente"});
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }



    }
}





