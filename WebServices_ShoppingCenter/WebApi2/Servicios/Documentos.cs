using ShoppingCenter.WebServices.Base.Models;
using ShoppingCenter.WebServices.Base.Utilidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Servicios
{
    public abstract class Documentos
    {
        protected bool EnEjecucion { get; set; }

        private readonly Conexion _utilConexion = new Conexion();

        public Conexion utilConexion { get => _utilConexion; }

        public abstract void procesarFila(DataRow row, string bdIntermedia);
        
        public abstract void enviarDocumentosNoEnviadosSAP();

        public abstract string updateIntermedia(string idTabla, string idSAP, string nameBD);

        public abstract void addCamposUsuario(object document, DataRow row, bool enviado);

        public void enviarDocumentosNoEnviadosSAPSync()
        {
            if (EnEjecucion) return;
            EnEjecucion = true;
            try
            {
                utilConexion.SesionLogin();
                enviarDocumentosNoEnviadosSAP();
                utilConexion.SesionLogout();
            }
            catch(Exception e) {
                string mensajeError = ObtenerMensajeError(e);

                // Puedes registrar el error o simplemente dejarlo en el throw
                throw new Exception(mensajeError);
            }
            finally
            {
                EnEjecucion = false;
            }
        }

        public static string ObtenerMensajeError(Exception e)
        {
            // Recorre las inner exceptions si existen
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e.Message;
        }


        public bool esEnviado(string valorIdSap, object enviado) {
            return !string.IsNullOrEmpty(valorIdSap);
                //enviado.Equals(DBNull.Value) ? false : bool.Parse(enviado.ToString());
        }

        public string incrementarIntento(string tabla, string txtIdTabla, string valorIdTabla, string bdIntermedia)
        {
            string query = string.Format("UPDATE {0}.\"{1}\" SET \"Intento\" = (\"Intento\" + 1 ) WHERE \"{2}\" = ?", bdIntermedia, tabla, txtIdTabla);

            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel(("@"+txtIdTabla), SqlDbType.VarChar, valorIdTabla));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public void finalizarIntento(bool enviado, string bdIntermedia, string Metodo, string Data, string URL, int IdTipoObjetoSap, int idObjetoSap, string IdObjetoIntermedia, string TxtIdSap) {
            string respuesta = utilConexion.RandomRest(Metodo, Data, URL, IdTipoObjetoSap, idObjetoSap, IdObjetoIntermedia);            
            if (containsError(respuesta))
            {
                throw new ArgumentException(respuesta);
            }
            else
            {
                if (!enviado)
                {
                    JObject res = JObject.Parse(respuesta);
                    string SapId = res[TxtIdSap].ToString();
                    updateIntermedia(IdObjetoIntermedia, SapId, bdIntermedia);
                }
            }
        }

        public string getDocumentoExistente(string txtIdExterno, string valorIdExterno, string txtIdSap, string metodoSAP, int constanteTipoObjetoSap, int constanteObjetoSapIndexes) {
            string valorIdSap = null;
            string filtroEstadoDocumento = ""; 
            switch (constanteTipoObjetoSap) {
                case (int)Constante.idTipoObjetoSap.FACTURACOMPRAS:
                case (int)Constante.idTipoObjetoSap.FACTURADEUDORES:
                case (int)Constante.idTipoObjetoSap.NOTACREDITOCLIENTE:
                case (int)Constante.idTipoObjetoSap.PAGORECIBIDOS:
                case (int)Constante.idTipoObjetoSap.SALIDAMERCANCIA:
                    filtroEstadoDocumento = " and Cancelled eq 'tNO'";
                    break;
            }

            string urlConsumo = $"/{metodoSAP}?$select={txtIdSap}&$filter=U_{txtIdExterno} eq '{valorIdExterno}'{filtroEstadoDocumento}&$orderby={txtIdSap} desc"; ;
                try
                {
                    string respuesta = utilConexion.RandomRest(
                        "GET",
                        "",
                        urlConsumo,
                        constanteTipoObjetoSap,
                        constanteObjetoSapIndexes,
                        valorIdExterno
                    );

                    JObject res = JObject.Parse(respuesta);
                    JToken value = res["value"];
                    if (res.ContainsKey("value") && value.Count<object>() > 0)
                    {
                        foreach (JProperty element in res["value"][0])
                        {
                            if (!element.Name.ToLower().Equals(txtIdSap.ToLower())) continue;
                                valorIdSap = element.Value.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    new ErrorGlobal(
                        $"No se encontro {metodoSAP}.{txtIdSap} para U_{txtIdExterno} '{valorIdExterno}'",
                        "GET",
                        "",
                        urlConsumo,
                        constanteTipoObjetoSap,
                        constanteObjetoSapIndexes,
                        valorIdExterno,
                        true);

                    throw ex;
                }
            return valorIdSap;
        }

        public string serializarObjeto(object objeto)
        {
            string dummy = string.Empty;
            string jsonData = JsonConvert.SerializeObject(objeto, Newtonsoft.Json.Formatting.Indented,
                              new JsonSerializerSettings
                              {
                                  Formatting = Newtonsoft.Json.Formatting.Indented,
                                  ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                  NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                  DefaultValueHandling = DefaultValueHandling.Ignore
                              });
            JObject Sapobject = JObject.Parse(jsonData);
            List<string> Nombre = new List<string>();
            foreach (var item in Sapobject)
            {
                if (item.Value.Type == JTokenType.Array || item.Value.Type == JTokenType.Object)
                {
                    if (!item.Value.HasValues)
                    {
                        Nombre.Add(item.Key);
                    }
                }
            }
            for (int i = 0; i < Nombre.Count; i++)
            {
                Sapobject.Remove(Nombre[i]);
            }
            string jsonData2 = JsonConvert.SerializeObject(Sapobject, Newtonsoft.Json.Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                Formatting = Newtonsoft.Json.Formatting.Indented,
                                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                DefaultValueHandling = DefaultValueHandling.Ignore
                            });
            return jsonData2;
        }

        public string getParametroGenral(Constante.ParametrosGenerales param) {

            var parametros = UConsola.Instance.getParametrosGenerales();

            switch (param) {
                case Constante.ParametrosGenerales.SNCliente:
                case Constante.ParametrosGenerales.SNVendedor:
                case Constante.ParametrosGenerales.CPagoProv:
                case Constante.ParametrosGenerales.WHCentral:
                case Constante.ParametrosGenerales.MaxIntentos:
                case Constante.ParametrosGenerales.CtaSalidaMerc:
                    return parametros.Find(e => e.Codigo == (int)param).Valor.ToString(); 
                default: 
                    //nunca deberia de llegar aca
                    return "";
            }

          
        }

        public bool containsError(string respuesta) {
            if (respuesta.ToUpper().Contains("-2028,")) return false;
            try
            {
                JObject res = JObject.Parse(respuesta);
                return (res.ContainsKey("error") || res.ContainsKey("Error") || res.ContainsKey("ERROR"));
            }
            catch {
                return respuesta.ToUpper().Contains("ERROR");
            }
        }



    }
}
