using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using ShoppingCenter.WebServices.Base.Utilidades;
using ShoppingCenter.WebServices.Base.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ShoppingCenter.WebServices.Base.Controllers
{
    /// <summary>
    /// Controlador que sirve para recibir y enviar datos desde la consola Base
    /// </summary>
    [Route("IGT/[controller]/[action]")]
    [ApiController]
    public class ConsolaController : Controller
    {

        private readonly ILogger<ConsolaController> _logger;

        public ConsolaController(ILogger<ConsolaController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ActionName("EditarIntermedia")]
        public ActionResult EditarIntermedia(ConfiguracionDBModel Datos)
        {
            string Resultado = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método EditarIntermedia ***************");
                Resultado = UConsola.Instance.setConfiguracionIntermedia(Datos,"Editar");
                Log.logger.Info("*************** Finalizando Método EditarIntermedia ***************");

                if (Resultado.ToUpper().Contains("EXITO"))
                {
                    return Ok(Resultado);
                }
                else
                {
                    return BadRequest(Resultado);
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en EditarIntermedia. Error: " + ex.Message);
                return BadRequest(ex.Message);
                throw;
            }


        }
        /// <summary>
        /// Obtiene los datos de conexion a la Base de datos intermedia
        /// y retorna el valor en formato json
        /// para posteriormente ser leidos en la consola
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ConexionIntermedia")]       
        public ActionResult<string> Get() 
        {
            string Json = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método ConexionIntermedia ***************");

                Json = UConsola.Instance.getConfiguracionIntermedia();

                Log.logger.Info("*************** Finalizando Método ConexionIntermedia ***************");

                return Json;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en ConexionIntermedia. Error: " + ex.Message);
                return ex.Message;
            }
        }
        /// <summary>
        /// Metodo que se encarga de crear la base de datos intermedia y de almacenar los datos de conexion
        /// </summary>
        [HttpPost]
        [ActionName("CrearIntermedia")]
        public ActionResult<string> Post(ConfiguracionDBModel Datos)
        {
            try
            {
                Log.logger.Info("*************** Iniciando Método CrearIntermedia ***************");

                string Respuesta = UConsola.Instance.CrearIntermedia(Datos);

                Log.logger.Info("*************** Finalizando Método CrearIntermedia ***************");

                if (Respuesta.Contains("Exito"))
                {
                    return Ok(Respuesta);
                }
                else
                {
                    return BadRequest(Respuesta);
                }
            }             
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en CrearIntermedia. Error: " + ex.Message);
                return ex.Message;
            }


        }
        /// <summary>
        /// Obtiene los datos de conexion a SAP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ConexionSAP")]
        public ActionResult<string> GetSap()
        {
            string Json = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método ConexionSAP ***************");
                Json = UConsola.Instance.getConfiguracionSAP();

                Log.logger.Info("*************** Finalizando Método ConexionSAP ***************");

                return Json;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en ConexionSAP. Error: " + ex.Message);
                return ex.Message;
            }
        }
        /// <summary>
        /// Alamcena los datos de conexion hacia SAP
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        
        [HttpPost]
        [ActionName("GuardarSap")]
        public ActionResult<string> PostSap(ConfiguracionDBModel item)
        {
            try
            {
                Log.logger.Info("*************** Iniciando Método GuardarSap ***************");

                string Respuesta = UConsola.Instance.setConfiguracionSAP(item);

                Log.logger.Info("*************** Finalizando Método GuardarSap ***************");

                return Respuesta;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GuardarSap. Error: " + ex.Message);
                return "Error General, "+ex.Message;
            }
        }
        

        /// <summary>
        /// Metodo que obtiene los registros de la bitacora
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetBitacora")]
        public ActionResult<string> GetBitacora()
        {
            string Resultado = string.Empty;
            List<BitacoraModel> bitacoras = new List<BitacoraModel>();
            try
            {
                Log.logger.Info("*************** Iniciando Método GetBitacora ***************");

                var headers = Request.Headers;
                bitacoras = UConsola.Instance.getBitacoras(Convert.ToInt32(headers["TipoObjeto"]), headers["TipoOperacion"], headers["tipoTransaccion"], headers["resultadoTransaccion"], headers["FechaDesde"], headers["FechaHasta"]);

                Resultado = JsonConvert.SerializeObject(bitacoras, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });
                Log.logger.Info("*************** Finalizando Método GetBitacora ***************");

                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GetBitacora. Error: " + ex.Message);
                return ex.Message;
                throw;
            }

        }

        [HttpPost]
        [ActionName("AddConfiguracionParametroGeneral")]
        public ActionResult<string> setParametroGeneral(object Datos)
        {
            try
            {
                string Data = JsonConvert.SerializeObject(Datos);
                JObject jObject = JObject.Parse(Data);

                string respuesta = UConsola.Instance.setParametroGeneral(jObject["Nombre"].ToString(), jObject["Descripcion"].ToString(), jObject["Valor"].ToString());

                return respuesta;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        [ActionName("GetConfiguracionParametroGeneral")]
        public ActionResult<string> getParametrosGenerales()
        {
            string Resultado = string.Empty;
            List<ParametrosGeneralesModel> parametros = new List<ParametrosGeneralesModel>();
            try
            {

                Log.logger.Info("*************** Iniciando Método GetConfiguracionParametroGeneral ***************");
                parametros = UConsola.Instance.getParametrosGenerales();
                Resultado = JsonConvert.SerializeObject(parametros, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });

                Log.logger.Info("*************** Finalizando Método GetConfiguracionParametroGeneral ***************");

                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GetConfiguracionParametroGeneral. Error: " + ex.Message);
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpGet]
        [ActionName("GetConfiguracionParametroGeneralID")]
        public ActionResult<string> getParametroGeneral()
        {
            string Resultado = string.Empty;
            ParametrosGeneralesModel parametro = new ParametrosGeneralesModel();
            try
            {
                Log.logger.Info("*************** Iniciando Método GetConfiguracionParametroGeneralID ***************");

                var headers = Request.Headers;
                parametro =UConsola.Instance.getParametroGeneral(headers["Id"].ToString());
                Resultado = JsonConvert.SerializeObject(parametro, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });

                Log.logger.Info("*************** Finalizando Método GetConfiguracionParametroGeneralID ***************");

                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GetConfiguracionParametroGeneralID. Error: " + ex.Message);
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpPut]
        [ActionName("EditConfiguracionParametroGeneral")]
        public ActionResult<string> editarParametroGeneral(object Datos)
        {
            string Respuesta = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método EditConfiguracionParametroGeneral ***************");

                string Data = JsonConvert.SerializeObject(Datos);
                JObject jObject = JObject.Parse(Data);

                string respuesta = UConsola.Instance.updateParametroGeneral(Convert.ToInt32(jObject["Codigo"]),jObject["Nombre"].ToString(), jObject["Descripcion"].ToString(), jObject["Valor"].ToString());


                Log.logger.Info("*************** Finalizando Método EditConfiguracionParametroGeneral ***************");

                return Respuesta;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en EditConfiguracionParametroGeneral. Error: " + ex.Message);
                return ex.Message;
            }
        }

        [HttpDelete]
        [ActionName("DeleteConfiguracionParametroGeneral")]
        public ActionResult<string> eliminarParametroGenereal()
        {
            string Respuesta = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método DeleteConfiguracionParametroGeneral ***************");

                var headers = Request.Headers;
                Respuesta = UConsola.Instance.deleteParametroGeneral(Convert.ToInt32(headers["Id"]));

                Log.logger.Info("*************** Finalizando Método DeleteConfiguracionParametroGeneral ***************");

                return Respuesta;

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en DeleteConfiguracionParametroGeneral. Error: " + ex.Message);
                return ex.Message;
            }
        }

        [HttpGet]
        [ActionName("GetTipoIbjetos")]
        public ActionResult<string> getTipoObjetos()
        {
            string Resultado = string.Empty;
            List<SelectListItem> objetos = new List<SelectListItem>();
            try
            {
                objetos = UConsola.Instance.getTiposObjetos();
                Resultado = JsonConvert.SerializeObject(objetos, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });
                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en getTipoObjetos. Error: " + ex.Message);
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpGet]
        [ActionName("GetDocPendientes")]
        public ActionResult<string> GetDocPendientes()
        {
            string Resultado = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                Log.logger.Info("*************** Iniciando Método GetDocPendientes ***************");

                var headers = Request.Headers;
                dt = UConsola.Instance.getDocPendientes(headers["TipoObjeto"], headers["FechaDesde"], headers["FechaHasta"]);

                Resultado = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });
                Log.logger.Info("*************** Finalizando Método GetDocPendientes ***************");

                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GetDocPendientes. Error: " + ex.Message);
                return ex.Message;
                throw;
            }

        }



        [HttpPost]
        [ActionName("SetParametrosEnvio")]
        public ActionResult<string> setParametrosEnvio(object Datos)
        {
            try
            {
                string Data = JsonConvert.SerializeObject(Datos);
                JObject jObject = JObject.Parse(Data);

                string respuesta = UConsola.Instance.setParametrosEnvio(Convert.ToInt32(jObject["IdServicio"]), jObject["Ejecucion"].ToString(), jObject["DiasEjecucion"].ToString(), jObject["TipoEnvio"].ToString(), jObject["Horario"].ToString(), Convert.ToInt32(jObject["TiempoEspera"]));

                return respuesta;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        [ActionName("GetParametrosEnvio")]
        public ActionResult<string> getParametrosEnvio()
        {
            string Resultado = string.Empty;
            ParametrosEnvioModel parametro = new ParametrosEnvioModel();
            try
            {
                Log.logger.Info("*************** Iniciando Método GetParametrosEnvio ***************");

                var headers = Request.Headers;
                parametro = UConsola.Instance.getParametrosEnvio();
                Resultado = JsonConvert.SerializeObject(parametro, Newtonsoft.Json.Formatting.Indented,
                             new JsonSerializerSettings
                             {
                                 Formatting = Newtonsoft.Json.Formatting.Indented,
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                 NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                 DefaultValueHandling = DefaultValueHandling.Ignore
                             });

                Log.logger.Info("*************** Finalizando Método GetParametrosEnvio ***************");

                return Ok(Resultado);

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en GetParametrosEnvio. Error: " + ex.Message);
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpPut]
        [ActionName("UpdateParametrosEnvio")]
        public ActionResult<string> updateParametroEnvio(object Datos)
        {
            string Respuesta = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método UpdateParametrosEnvio ***************");

                string Data = JsonConvert.SerializeObject(Datos);
                JObject jObject = JObject.Parse(Data);

                string respuesta = UConsola.Instance.updateParametrosEnvio(Convert.ToInt32(jObject["IdServicio"]), jObject["Ejecucion"].ToString(), jObject["DiasEjecucion"].ToString(), jObject["TipoEnvio"].ToString(), jObject["Horario"].ToString(), Convert.ToInt32(jObject["TiempoEspera"]));


                Log.logger.Info("*************** Finalizando Método UpdateParametrosEnvio ***************");

                return Respuesta;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en UpdateParametrosEnvio. Error: " + ex.Message);
                return ex.Message;
            }
        }

        [HttpDelete]
        [ActionName("DeleteParametrosEnvio")]
        public ActionResult<string> deleteParametrosEnvio()
        {
            string Respuesta = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método DeleteParametrosEnvio ***************");

                var headers = Request.Headers;
                Respuesta = UConsola.Instance.deleteParametrosEnvio(Convert.ToInt32(headers["IdServicio"]));

                Log.logger.Info("*************** Finalizando Método DeleteParametrosEnvio ***************");

                return Respuesta;

            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en DeleteParametrosEnvio. Error: " + ex.Message);
                return ex.Message;
            }
        }



        [HttpPut]
        [ActionName("EnvioManual")]
        public ActionResult<string> envioManual()
        {
            string Respuesta = string.Empty;
            try
            {
                Log.logger.Info("*************** Iniciando Método EnvioManual ***************");

                var headers = Request.Headers;

                string respuesta = UConsola.Instance.envioManual(Convert.ToInt32(headers["IdObjeto"]));


                Log.logger.Info("*************** Finalizando Método EnvioManual ***************");

                return Respuesta;
            }
            catch (Exception ex)
            {
                Log.logger.Error("Ocurrió un error inesperado en EnvioManual. Error: " + ex.Message);
                return ex.Message;
            }
        }

    }
}