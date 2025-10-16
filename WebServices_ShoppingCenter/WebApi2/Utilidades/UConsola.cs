using Newtonsoft.Json;
using System;
using ShoppingCenter.WebServices.Base.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Data.Odbc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class UConsola
    {
        private readonly static UConsola _instance = new UConsola();
        private Conexion utilConexion;

        private UConsola()
        {
            utilConexion = new Conexion();
        }

        public static UConsola Instance
        {
            get
            {
                return _instance;
            }
        }
        public string getConfiguracionSAP()
        {
            string Respuesta = string.Empty;
            ConfiguracionDBModel item = new ConfiguracionDBModel();
            try
            {
                Seguridad seguridad = new Seguridad();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(@"Connections.xml");
                XmlElement root = (XmlElement)xDoc.GetElementsByTagName("INFO")[0];

                //DATOS SAP
                XmlElement db = (XmlElement)root.GetElementsByTagName("SAP")[0];
                item.Server = db.GetElementsByTagName("SERVER")[0].InnerText;
                item.Port = db.GetElementsByTagName("PORT")[0].InnerText;
                item.UserHana = seguridad.Desencripta(db.GetElementsByTagName("USERHANA")[0].InnerText);
                item.PasswordHana = seguridad.Desencripta(db.GetElementsByTagName("PASSWORDHANA")[0].InnerText);
                item.DNS = db.GetElementsByTagName("DNS")[0].InnerText;
                item.DBHana = db.GetElementsByTagName("DBHANA")[0].InnerText;

                Respuesta = JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented,
                              new JsonSerializerSettings
                              {
                                  Formatting = Newtonsoft.Json.Formatting.Indented,
                                  ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                  NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                  DefaultValueHandling = DefaultValueHandling.Ignore
                              });
                return Respuesta;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public string setConfiguracionSAP(ConfiguracionDBModel item)
        {
            string respuesta = string.Empty;
            try
            {
                Conexion con = new Conexion();
                // BD SAP
                string result = con.PruebaLogin(item);
                if (result.Equals("Login"))
                {

                    Seguridad seguridad = new Seguridad();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(@"Connections.xml");
                    XmlElement root = (XmlElement)xDoc.GetElementsByTagName("INFO")[0];
                    XmlElement db = (XmlElement)root.GetElementsByTagName("SAP")[0];

                    db.GetElementsByTagName("SERVER")[0].InnerText = item.Server;
                    db.GetElementsByTagName("PORT")[0].InnerText = item.Port;
                    db.GetElementsByTagName("USERHANA")[0].InnerText = seguridad.Encripta(item.UserHana);
                    db.GetElementsByTagName("PASSWORDHANA")[0].InnerText = seguridad.Encripta(item.PasswordHana);
                    db.GetElementsByTagName("DNS")[0].InnerText = item.DNS;
                    db.GetElementsByTagName("DBHANA")[0].InnerText = item.DBHana;

                    xDoc.Save(@"Connections.xml");
                    respuesta = " Datos SAP Guardados Con Exito";
                }
                else
                {
                    respuesta = " Error Conexion Base de Datos SAP: " + result;
                }
                
                return respuesta;

            }
            catch (Exception e)
            {
                return " Error General " + e.Message;
            }
        }

        public string getConfiguracionIntermedia()
        {
            string Respuesta = string.Empty;
            ConfiguracionDBModel item = new ConfiguracionDBModel();
            try
            {
                Seguridad seguridad = new Seguridad();
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(@"Connections.xml");
                XmlElement root = (XmlElement)xDoc.GetElementsByTagName("INFO")[0];

                //DATOS INTERMEDIA
                XmlElement dbInter = (XmlElement)root.GetElementsByTagName("INTERMEDIA")[0];
                item.Server = dbInter.GetElementsByTagName("SERVER")[0].InnerText;
                item.Port= dbInter.GetElementsByTagName("PORT")[0].InnerText;
                item.UserHana = seguridad.Desencripta(dbInter.GetElementsByTagName("USERHANA")[0].InnerText);
                item.PasswordHana = seguridad.Desencripta(dbInter.GetElementsByTagName("PASSWORDHANA")[0].InnerText);
                item.DNS= dbInter.GetElementsByTagName("DNS")[0].InnerText;
                item.DBHana = dbInter.GetElementsByTagName("DBHANA")[0].InnerText;

                Respuesta = JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented,
                              new JsonSerializerSettings
                              {
                                  Formatting = Newtonsoft.Json.Formatting.Indented,
                                  ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                  NullValueHandling = NullValueHandling.Ignore, //used to remove empty or null properties
                                  DefaultValueHandling = DefaultValueHandling.Ignore
                              });
                return Respuesta;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public string setConfiguracionIntermedia(ConfiguracionDBModel item,string Accion)
        {
            string respuesta = string.Empty;
            try
            {
                Conexion con = new Conexion();
                // BD INTERMEDIA
                string result = con.PruebaConexionIntermedia(item,Accion);
                if (result.ToUpper().Contains("EXITO"))
                {
                    Seguridad seguridad = new Seguridad();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(@"Connections.xml");
                    XmlElement root = (XmlElement)xDoc.GetElementsByTagName("INFO")[0];
                    XmlElement db = (XmlElement)root.GetElementsByTagName("INTERMEDIA")[0];

                    db.GetElementsByTagName("SERVER")[0].InnerText = item.Server;
                    db.GetElementsByTagName("PORT")[0].InnerText = item.Port;
                    db.GetElementsByTagName("USERHANA")[0].InnerText = seguridad.Encripta(item.UserHana);
                    db.GetElementsByTagName("PASSWORDHANA")[0].InnerText = seguridad.Encripta(item.PasswordHana);
                    db.GetElementsByTagName("DNS")[0].InnerText = item.DNS;
                    db.GetElementsByTagName("DBHANA")[0].InnerText = item.DBHana;

                    xDoc.Save(@"Connections.xml");
                    respuesta =" Datos Intermedia Guardados Con Exito";
                }
                else
                {
                    respuesta = " Error Conexion Base de Datos Intermedia: " + result;
                }

                return respuesta;

            }
            catch (Exception e)
            {
                return " Error General " + e.Message;
            }
        }

        public string CrearIntermedia(ConfiguracionDBModel ModeloIntermedia)
        {
            string Respuesta = string.Empty;
            OdbcConnection myConnection = new OdbcConnection();
            try
            {
                string dummy = setConfiguracionIntermedia(ModeloIntermedia,"Crear");
                if (dummy.ToUpper().Contains("EXITO"))
                {

                    StreamReader sr = new StreamReader(@"BaseIntermedia.txt");
                    string[] QueryIntermedia = sr.ReadToEnd().Split(';');
                    myConnection.ConnectionString = utilConexion.ConnectionString("INTERMEDIA");

                    myConnection.Open();
                    OdbcCommand commandIntermedia = myConnection.CreateCommand();
                    try
                        {
                            commandIntermedia.CommandText = "DROP SCHEMA " + ModeloIntermedia.DBHana + " CASCADE";
                            commandIntermedia.ExecuteNonQuery();
                            commandIntermedia.CommandText = "CREATE SCHEMA " + ModeloIntermedia.DBHana;
                            commandIntermedia.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            commandIntermedia.CommandText = "CREATE SCHEMA " + ModeloIntermedia.DBHana;
                            commandIntermedia.ExecuteNonQuery();
                        }

                    string Query = string.Empty;

                    foreach (string item in QueryIntermedia)
                    {
                        if (!string.IsNullOrWhiteSpace(item) || !string.IsNullOrEmpty(item))
                        {
                            Query = item;
                            if (Query.Contains("NOMBREDB."))
                            {
                                Query = Query.Replace("NOMBREDB.", ModeloIntermedia.DBHana + ".");
                            }
                            commandIntermedia.CommandText = Query;
                            commandIntermedia.ExecuteNonQuery();
                        }
                    }
                    commandIntermedia.Dispose();

                    return "Base de datos creada con Exito";
                }
                else
                {
                    return Respuesta;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                myConnection.Dispose();
                myConnection.Close();
            }
        }

        public List<BitacoraModel> getBitacoras(int tipoObjeto, string tipoOperacion, string tipoTransaccion, string resultadoTransaccion, string fechaDesde, string fechaHasta)
        {
            List<BitacoraModel> lista = new List<BitacoraModel>();
            string query = getQueryBitacora(tipoObjeto, tipoOperacion, tipoTransaccion, resultadoTransaccion, fechaDesde, fechaHasta);
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_BITACORA", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                BitacoraModel bit = new BitacoraModel();
                bit.Correlativo = item["Correlativo"].ToString();
                bit.TipoObjeto = (string)item["Nombre"];
                bit.Fecha = Convert.ToDateTime(item["Fecha"]);
                bit.Resultado = item["ResultadoTransaccion"].ToString();
                bit.Mensaje = string.IsNullOrEmpty(item["Mensaje"].ToString()) ? "" : item["Mensaje"].ToString();
                bit.CodigoError = string.IsNullOrEmpty(item["IdError"].ToString()) ? "" : item["IdError"].ToString();
                bit.IDObjetoSap = string.IsNullOrEmpty(item["IdObjetoSap"].ToString()) ? "" : item["IdObjetoSap"].ToString();
                bit.IDObjetoIntermedia = string.IsNullOrEmpty(item["IdObjetoIntermedia"].ToString()) ? "" : item["IdObjetoIntermedia"].ToString();
                bit.TipoTransaccion = item["TipoTransaccion"].ToString();
                bit.TipoOperacion = item["TipoOperacion"].ToString();
                lista.Add(bit);
            }
            return lista;
        }

        private string getQueryBitacora(int tipoObjeto, string tipoOperacion, string tipoTransaccion, string resultadoTransaccion, string fechaDesde, string fechaHasta)
        {
            string BaseDatos = utilConexion.NombreDB("INTERMEDIA");
            string queryString = "" +
                "SELECT T0.\"Correlativo\", T1.\"Nombre\", T0.\"Fecha\", T0.\"ResultadoTransaccion\",T0.\"Mensaje\",T0.\"IdError\",T0.\"IdObjetoSap\",T0.\"IdObjetoIntermedia\",T0.\"TipoTransaccion\",T0.\"TipoOperacion\" " +
                "FROM " + BaseDatos + ".\"T_BITACORA\" AS T0 " +
                "INNER JOIN " + BaseDatos + ".\"T_TIPO_OBJETO\" T1 ON T1.\"IdTipoObjetoSap\" = T0.\"IdTipoObjetoSap\" " +
                "WHERE to_date(T0.\"Fecha\") BETWEEN to_date('" + fechaDesde + "') AND to_date('" + fechaHasta + "') " +
                "AND T1.\"Estado\" = 'A' ";

            if (tipoObjeto != 0)
            {
                queryString += " AND T0.\"IdTipoObjetoSap\" = '" + tipoObjeto + "'";
            }

            if (tipoOperacion != "Todos")
            {
                queryString += " AND T0.\"TipoOperacion\" = '" + tipoOperacion + "'";
            }

            if (tipoTransaccion != "Todos")
            {
                queryString += " AND T0.\"TipoTransaccion\" = '" + tipoTransaccion + "'";
            }

            if (resultadoTransaccion != "Todos")
            {
                queryString += " AND T0.\"ResultadoTransaccion\" = '" + resultadoTransaccion + "'";
            }
            return queryString;
        }

        public List<ParametrosGeneralesModel> getParametrosGenerales()
        {
            List<ParametrosGeneralesModel> lista = new List<ParametrosGeneralesModel>();
            string query =  "SELECT * FROM "+ utilConexion.NombreDB("INTERMEDIA")+ ".\"T_PARAMETROS\"";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_PARAMETROS", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                ParametrosGeneralesModel parametro = new ParametrosGeneralesModel();
                parametro.Codigo = Convert.ToInt32(item["Codigo"]);
                parametro.Nombre = item["Nombre"].ToString();
                parametro.Descripcion = item["Descripcion"].ToString();
                parametro.Valor = item["Valor"].ToString();
                lista.Add(parametro);
            }
            return lista;
        }

        public ParametrosGeneralesModel getParametroGeneral(string codigo)
        {
            ParametrosGeneralesModel parametro = new ParametrosGeneralesModel();
            string query = "SELECT * FROM " + utilConexion.NombreDB("INTERMEDIA") + ".\"T_PARAMETROS\" WHERE \"Codigo\" = '" + codigo+"'";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_PARAMETROS", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                parametro.Codigo = Convert.ToInt32(item["Codigo"]);
                parametro.Nombre = item["Nombre"].ToString();
                parametro.Descripcion = item["Descripcion"].ToString();
                parametro.Valor = item["Valor"].ToString();
            }
            return parametro;
        }

        public string setParametroGeneral(string nombre,string descripcion,string valor)
        {
            string query = string.Format("INSERT INTO {0}.\"T_PARAMETROS\"(\"Nombre\",\"Descripcion\",\"Valor\") VALUES (?,?,?)", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@Nombre", SqlDbType.VarChar, nombre));
            parametros.Add(new ParametroDBModel("@Descripcion", SqlDbType.VarChar, descripcion));
            parametros.Add(new ParametroDBModel("@Valor", SqlDbType.VarChar, valor));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public string updateParametroGeneral(int Codigo,string nombre, string descripcion, string valor)
        {
            string query = string.Format("UPDATE {0}.\"T_PARAMETROS\" SET \"Nombre\"=?,\"Descripcion\"=?,\"Valor\"=? WHERE \"Codigo\" = ?", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@Nombre", SqlDbType.VarChar, nombre));
            parametros.Add(new ParametroDBModel("@Descripcion", SqlDbType.VarChar, descripcion));
            parametros.Add(new ParametroDBModel("@Valor", SqlDbType.VarChar, valor));
            parametros.Add(new ParametroDBModel("@Codigo", SqlDbType.Int, Codigo));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public string deleteParametroGeneral(int Codigo)
        {
            string query = string.Format("DELETE FROM {0}.\"T_PARAMETROS\" WHERE \"Codigo\" = ?", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@Codigo", SqlDbType.Int, Codigo));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public List<SelectListItem> getTiposObjetos()
        {
            List<SelectListItem> listado = new List<SelectListItem>();
            string query = "SELECT \"IdTipoObjetoSap\",\"Nombre\" FROM " + utilConexion.NombreDB("INTERMEDIA") + ".\"T_TIPO_OBJETO\" WHERE \"Estado\" = 'A' ORDER BY \"Orden\" ";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_OBJETO", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                listado.Add(new SelectListItem(item["Nombre"].ToString(), item["IdTipoObjetoSap"].ToString()));
            }
            return listado;
        }

        public DataTable getDocPendientes(string idObjeto, string fechaDesde, string fechaHasta)
        {
            string query = QueryDocPendientes(idObjeto,fechaDesde,fechaHasta);
            DataSet ds = utilConexion.GetQueryGenerico(query, "T_PARAMETROS", "INTERMEDIA");
            DataTable dt = ds.Tables["T_PARAMETROS"];
            return dt;
        }

        public string QueryDocPendientes(string idObjeto, string fechaDesde, string fechaHasta)
        {
            string result = "SELECT ";
            string tabla = string.Empty, campos = string.Empty;
            
            string query = "SELECT \"NombreTabla\",\"NombreCampo\" FROM " + utilConexion.NombreDB("INTERMEDIA") + ".\"T_TIPO_OBJETO\" WHERE \"Estado\" = 'A' AND \"IdTipoObjetoSap\" = '"+idObjeto+"'";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_OBJETO", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                tabla = item["NombreTabla"].ToString();
                campos = item["NombreCampo"].ToString();
            }
            string[] split = campos.Split(',');

            foreach(string cam in split)
            {
                result += " \"" +cam+"\",";
            }

            result = result.TrimEnd(',');

            result += " FROM " + utilConexion.NombreDB("INTERMEDIA") + ".\""+tabla+ "\" WHERE \"Enviar\" = True AND to_date(\"FechaCreacion\") BETWEEN to_date('" + fechaDesde + "') AND to_date('" + fechaHasta + "') ";

            return result;
        }

        public ParametrosEnvioModel getParametrosEnvio()
        {
            ParametrosEnvioModel parametro = new ParametrosEnvioModel();
            string query = "SELECT * FROM " + utilConexion.NombreDB("INTERMEDIA") + ".\"T_PARAMETROS_ENVIO\" ";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_PARAMETROS", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                parametro.IdServicio = Convert.ToInt32(item["IdServicio"]);
                parametro.Ejecucion = item["Ejecucion"].ToString();
                parametro.DiasEjecucion = item["DiasEjecucion"].ToString();
                parametro.TipoEnvio = item["TipoEnvio"].ToString();
                parametro.Horario = item["Horario"].ToString();
                parametro.TiempoEspera = Convert.ToInt32(item["TiempoEspera"]);
            }
            return parametro;
        }

        public string setParametrosEnvio(int idServicio, string ejecucion, string diasEjecucion,string tipoEnvio, string horario, int tiempoEspera)
        {
            string query = string.Format("INSERT INTO {0}.\"T_PARAMETROS_ENVIO\"(\"IdServicio\",\"Ejecucion\",\"DiasEjecucion\",\"TipoEnvio\",\"Horario\",\"TiempoEspera\") VALUES (?,?,?,?,?,?)", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@IdServicio", SqlDbType.Int, idServicio));
            parametros.Add(new ParametroDBModel("@Ejecucion", SqlDbType.VarChar, ejecucion));
            parametros.Add(new ParametroDBModel("@DiasEjecucion", SqlDbType.VarChar, diasEjecucion));
            parametros.Add(new ParametroDBModel("@TipoEnvio", SqlDbType.VarChar, tipoEnvio));
            parametros.Add(new ParametroDBModel("@Horario", SqlDbType.VarChar, horario));
            parametros.Add(new ParametroDBModel("@TiempoEspera", SqlDbType.VarChar, tiempoEspera));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public string updateParametrosEnvio(int idServicio, string ejecucion, string diasEjecucion, string tipoEnvio, string horario, int tiempoEspera)
        {
            string query = string.Format("UPDATE {0}.\"T_PARAMETROS_ENVIO\" SET \"Ejecucion\"=?,\"DiasEjecucion\"=?,\"TipoEnvio\"=?,\"Horario\"=?,\"TiempoEspera\"=? WHERE \"IdServicio\" = ?", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@Ejecucion", SqlDbType.VarChar, ejecucion));
            parametros.Add(new ParametroDBModel("@DiasEjecucion", SqlDbType.VarChar, diasEjecucion));
            parametros.Add(new ParametroDBModel("@TipoEnvio", SqlDbType.VarChar, tipoEnvio));
            parametros.Add(new ParametroDBModel("@Horario", SqlDbType.VarChar, horario));
            parametros.Add(new ParametroDBModel("@TiempoEspera", SqlDbType.VarChar, tiempoEspera));
            parametros.Add(new ParametroDBModel("@IdServicio", SqlDbType.Int, idServicio));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public string deleteParametrosEnvio(int idServicio)
        {
            string query = string.Format("DELETE FROM {0}.\"T_PARAMETROS_ENVIO\" WHERE \"IdServicio\" = ?", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@IdServicio", SqlDbType.Int, idServicio));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }


        public string envioManual(int IdObjeto)
        {
            Servicios.Servicio.Instance.Enviar(IdObjeto);
            return "OK";
            /*
            string query = string.Format("UPDATE {0}.\"T_TIPO_OBJETO\" SET \"EnvioManual\"=? WHERE \"IdTipoObjetoSap\" = ?", utilConexion.NombreDB("INTERMEDIA"));
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@EnvioManual", SqlDbType.VarChar, "1"));
            parametros.Add(new ParametroDBModel("@IdTipoObjetoSap", SqlDbType.VarChar, IdObjeto));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
            */
        }
    }
}
