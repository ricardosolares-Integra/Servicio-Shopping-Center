using ShoppingCenter.WebServices.Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using ShoppingCenter.WebServices.Base.Controllers;
using Newtonsoft.Json.Linq;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class Conexion
    {
        CookieContainer SlCookiesSesion = null;
        WebHeaderCollection SlHeadersSesion = null;

        #region LecturaXML
        private XmlElement PropiedadesDB(string DB)
        {

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"Connections.xml");
            XmlElement root = (XmlElement)xDoc.GetElementsByTagName("INFO")[0];
            XmlElement db = (XmlElement)root.GetElementsByTagName(DB.ToUpper())[0];
            return db;
        }

        public string ConnectionString(string DB)
        {
            Seguridad seguridad = new Seguridad();
            Dictionary<string, string> DatosConexion = new Dictionary<string, string>();
            XmlElement nodo = PropiedadesDB(DB);

            DatosConexion.Add("SERVER", nodo.GetElementsByTagName("SERVER")[0].InnerText);
            DatosConexion.Add("PORT", nodo.GetElementsByTagName("PORT")[0].InnerText);
            DatosConexion.Add("USERHANA", seguridad.Desencripta(nodo.GetElementsByTagName("USERHANA")[0].InnerText));
            DatosConexion.Add("PASSWORDHANA", seguridad.Desencripta(nodo.GetElementsByTagName("PASSWORDHANA")[0].InnerText));
            DatosConexion.Add("DNS", nodo.GetElementsByTagName("DNS")[0].InnerText);

            return @"DSN=" + DatosConexion.GetValueOrDefault("DNS") + ";" +
                    "SERVERNODE=" + DatosConexion.GetValueOrDefault("SERVER") + ":" + DatosConexion.GetValueOrDefault("PORT") + ";" +
                    "UID=" + DatosConexion.GetValueOrDefault("USERHANA") + ";" +
                    "PWD=" + DatosConexion.GetValueOrDefault("PASSWORDHANA") + ";";
        }

        public string NombreDB(string DB)
        {
            XmlElement nodo = PropiedadesDB(DB);
            return nodo.GetElementsByTagName("DBHANA")[0].InnerText;
        }

        public bool EsUsrDB(string DB, string usr, string pass)
        {
            XmlElement nodo = PropiedadesDB(DB);
            Seguridad seguridad = new Seguridad();
            return seguridad.Desencripta(nodo.GetElementsByTagName("USERHANA")[0].InnerText).ToUpper().Equals(usr.ToUpper()) &&
                seguridad.Desencripta(nodo.GetElementsByTagName("PASSWORDHANA")[0].InnerText).Equals(pass);
        }

        private string UrlDB(string DB)
        {
            XmlElement nodo = PropiedadesDB(DB);
            string Server = nodo.GetElementsByTagName("SERVER")[0].InnerText;
            string puerto = nodo.GetElementsByTagName("PORT")[0].InnerText;

            if (!Server.EndsWith(":"))
            {
                Server = Server + ":" + puerto + "/b1s/v1/";
            }
            if (!Server.Contains("https://") || !Server.Contains("http://"))
            {
                Server = "https://" + Server;
            }
            return Server;
        }
        #endregion

        #region Intermedia

        public DataRowCollection GetDataRowCollection(string Query, string Tabla, string DB) {
            DataSet ds = GetQueryGenerico(Query, Tabla, DB);
            return ds.Tables[Tabla].Rows;
        }

        public DataSet GetQueryGenerico(string Query, string Tabla, string DB)
        {
            DataSet ds = new DataSet();
                OdbcConnection myConnection = new OdbcConnection();
                myConnection.ConnectionString = ConnectionString(DB);
                myConnection.Open();

                OdbcDataAdapter adapter = new OdbcDataAdapter(Query, myConnection);
                adapter.Fill(ds, Tabla);
                myConnection.Close();
                myConnection.Dispose();
            return ds;
        }

        public string GetQueryGenerico(string Query, string DB, List<ParametroDBModel> parametros)
        {
            string respuesta = "";
            using (OdbcConnection conexion = new OdbcConnection(ConnectionString(DB)))
            {
                OdbcCommand comando = conexion.CreateCommand();
                OdbcTransaction transaccion;
                comando.Connection = conexion;
                conexion.Open();
                transaccion = conexion.BeginTransaction();
                try
                {
                    comando.Connection = conexion;
                    comando.Transaction = transaccion;
                    comando.CommandText = Query;

                    foreach (ParametroDBModel parametro in parametros)
                    {
                        if (parametro.Valor == null)
                        {
                            comando.Parameters.AddWithValue(parametro.Nombre, parametro.Tipo).Value = DBNull.Value;
                        }
                        else
                        {
                            comando.Parameters.AddWithValue(parametro.Nombre, parametro.Tipo).Value = parametro.Valor;
                        }

                    }
                    
                    OdbcDataReader reader =  comando.ExecuteReader();
                    while (reader.Read())
                    {
                        respuesta = reader[0].ToString();
                    }
                    reader.Close();
                    transaccion.Commit();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    respuesta = "Error: " + ex.Message;
                }
                finally
                {
                    comando.Dispose();
                    transaccion.Dispose();
                    conexion.Close();
                    conexion.Dispose();
                }
            }
            return respuesta;
        }

        public string EjecutarComandoGenerico(string Query, string DB, List<ParametroDBModel> parametros)
        {
            string respuesta = "";
            using (OdbcConnection conexion = new OdbcConnection(ConnectionString(DB)))
            {
                OdbcCommand comando = conexion.CreateCommand();
                OdbcTransaction transaccion;
                comando.Connection = conexion;
                conexion.Open();
                transaccion = conexion.BeginTransaction();
                try
                {
                    comando.Connection = conexion;
                    comando.Transaction = transaccion;
                    comando.CommandText = Query;

                    foreach (ParametroDBModel parametro in parametros)
                    {
                        if (parametro.Valor == null)
                        {
                            comando.Parameters.AddWithValue(parametro.Nombre, parametro.Tipo).Value = DBNull.Value;
                        }
                        else {
                            comando.Parameters.AddWithValue(parametro.Nombre, parametro.Tipo).Value = parametro.Valor;
                        }
                        
                    }
                    respuesta = comando.ExecuteNonQuery() + " fila(s) afectadas";
                    transaccion.Commit();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    respuesta = "Error: " + ex.Message;
                }
                finally
                {
                    comando.Dispose();
                    transaccion.Dispose();
                    conexion.Close();
                    conexion.Dispose();
                }
            }
            return respuesta;
        }

        public string PruebaConexionIntermedia(ConfiguracionDBModel item,string Accion)
        {
            string respuesta = "";
            string stringConexion = @"DSN="+item.DNS+";" + "SERVERNODE=" + item.Server + ":" + item.Port + ";" + "UID=" + item.UserHana + ";" +"PWD=" + item.PasswordHana + ";";
            string Query = "SET SCHEMA \""+item.DBHana+"\";";
            OdbcConnection myConnection = new OdbcConnection();
            myConnection.ConnectionString = stringConexion;
            OdbcCommand comando = myConnection.CreateCommand();
            try
            {
                myConnection.Open();
                if (Accion.Equals("Crear"))
                {
                    if (myConnection.State.Equals(ConnectionState.Open))
                    {
                        respuesta = "Conectado con Exito";
                    }
                    else
                    {
                        respuesta = "Error al Conectar";
                    }
                }
                else
                {
                    comando.Connection = myConnection;
                    comando.CommandText = Query;
                    OdbcDataAdapter adapter = new OdbcDataAdapter(Query, myConnection);
                    respuesta = "Exito: " + comando.ExecuteNonQuery() + " fila(s) afectadas";
                }
            }
            catch (Exception ex)
            {
                respuesta = "Error: " + ex.Message;
            }
            finally
            {
                comando.Dispose();
                myConnection.Close();
                myConnection.Dispose();
            }

            return respuesta;
        }

        #endregion

        #region SAP
        public string PruebaLogin(ConfiguracionDBModel item)
        {
            HttpWebResponse LoginResponse = null;
            try
            {
                string Server = item.Server;
                if (!Server.EndsWith(":"))
                {
                    Server = Server + ":" + item.Port + "/b1s/v1/";
                }
                if (!Server.Contains("https://") || !Server.Contains("http://"))
                {
                    Server = "https://" + Server;
                }

                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Server + "Login");
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.CookieContainer = new CookieContainer();

                LoginModel Login = new LoginModel();
                Login.CompanyDB = item.DBHana;
                Login.Password = item.PasswordHana;
                Login.UserName = item.UserHana;

                string Parametros = JsonConvert.SerializeObject(Login);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(Parametros);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                LoginResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (LoginResponse.StatusCode == HttpStatusCode.OK)
                {
                    SesionLogout();
                    return "Login";
                }
                else
                {
                    string actual = string.Empty;
                    using (var streamReader = new StreamReader(LoginResponse.GetResponseStream()))
                    {
                        actual = streamReader.ReadToEnd();
                    }
                    return actual;
                }
            }
            catch (WebException e)
            {
                string text;
                string ManejoDeErrores = string.Empty;
                WebResponse response;
                using (response = e.Response)
                {
                    if (!(response == null))
                    {
                        using (Stream data = response.GetResponseStream())
                        {
                            text = new StreamReader(data).ReadToEnd();
                        }
                        ManejoDeErrores = text;
                    }
                    else
                    {
                        ManejoDeErrores = "No se logro tener conexion con el servidor.";
                    }

                }

                return ManejoDeErrores;
            }
        }

        public HttpWebResponse SesionLogout()
        {
            HttpWebResponse LoginResponse = null;
            string Server = UrlDB("SAP");
            Uri UrlConecction = new Uri(Server);
            ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Server + "Logout");
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.CookieContainer = this.SlCookiesSesion;

            LoginResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            this.SlCookiesSesion = null;
            this.SlHeadersSesion = null;

            return LoginResponse;
        }

       

        public HttpWebResponse SesionLogin()
        {
            HttpWebResponse LoginResponse = null;
            Seguridad seguridad = new Seguridad();
            XmlElement nodo = PropiedadesDB("SAP");
            string Server = UrlDB("SAP");
            Uri URLSap = new Uri(Server + "Login");

            ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URLSap);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.CookieContainer = new CookieContainer();
            LoginModel Login = new LoginModel();
            Login.CompanyDB = nodo.GetElementsByTagName("DBHANA")[0].InnerText;
            Login.Password = seguridad.Desencripta(nodo.GetElementsByTagName("PASSWORDHANA")[0].InnerText);
            Login.UserName = seguridad.Desencripta(nodo.GetElementsByTagName("USERHANA")[0].InnerText);

            string Parametros = JsonConvert.SerializeObject(Login);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(Parametros);
                streamWriter.Flush();
                streamWriter.Close();
            }

            LoginResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            
            this.SlHeadersSesion = LoginResponse.Headers;
            this.SlCookiesSesion = new CookieContainer();    
            foreach (Cookie cookieValue in LoginResponse.Cookies)
            {
                Cookie cookie = new Cookie();
                cookie.Path = cookieValue.Name.Equals("B1SESSION") ? "/b1s/v1" : "/b1s";
                cookie.Name = cookieValue.Name;
                cookie.Value = cookieValue.Value;
                cookie.Domain = URLSap.Host;
                this.SlCookiesSesion.Add(cookie);
            }

            return LoginResponse;
        }

        
        private bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //throw new NotImplementedException();
            //accept
            return true;
        }

        /// <summary>
        /// Metodo que ejecuta una instruccion REST a service Layer
        /// </summary>
        /// <param name="Metodo">GET | POST | PATCH | DELETE</param>
        /// <param name="Data"></param>
        /// <param name="URL"></param>
        /// 2. Firmar
        /// 3. Mandar a hacienda
        ///    1. Login
        /// <returns></returns>
        public string RandomRest(string Metodo, string Data, string URL, int idTipoObjetoSap , int idObjetoSap, string  idObjetoIntermedia = "-1", bool UltimoIntento = false)
        {
            string respuesta = "";
            string Server = UrlDB("SAP");
            HttpWebResponse CreateResponse = null;
            bool error = false ;
            try
            {
                if (SlCookiesSesion == null || SlHeadersSesion == null) {
                    SesionLogin();
                }

                ServicePointManager.ServerCertificateValidationCallback += RemoteSSLTLSCertificateValidate;
                Uri URLSap = new Uri(Server + URL);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URLSap);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = Metodo;
                httpWebRequest.CookieContainer = this.SlCookiesSesion; // new CookieContainer();
                httpWebRequest.Headers = this.SlHeadersSesion; // session.Headers;
                switch (Metodo)
                {
                    case "GET":
                    case "DELETE":
                        break;
                    default:
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(Data);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        break;
                }

                CreateResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(CreateResponse.GetResponseStream()))
                {
                    respuesta = streamReader.ReadToEnd();
                }

            }
            catch (WebException e)
            {
                error = true;
                HttpWebResponse httpResponse;
                WebResponse response;
                bool LoginFallido = false;
                using (response = e.Response)
                {
                    httpResponse = (HttpWebResponse)response;
                    LoginFallido = httpResponse.StatusCode == HttpStatusCode.Unauthorized;
                    using (Stream data = response.GetResponseStream())
                    {
                        respuesta = new StreamReader(data).ReadToEnd();
                    }
                }

                if (!UltimoIntento && LoginFallido)
                {
                    this.SlCookiesSesion = null;
                    this.SlHeadersSesion = null;
                    return RandomRest(Metodo, Data, URL, idTipoObjetoSap, idObjetoSap, idObjetoIntermedia, true);
                }
                throw new FieldAccessException(message: respuesta.ToString());
            }
            catch (Exception e) {
                respuesta += " " + e.Message;
            }
            finally
            {
                ULogger.instance.log(
                    idTipoObjetoSap,
                    NombreDB("SAP"),
                    Metodo,
                    URL,
                    Data,
                    respuesta,
                    error,
                    Constante.TipoTransaccion.SAP,
                    idObjetoSap.ToString(),
                    idObjetoIntermedia
                    );
            }

            return respuesta;
        }

        #endregion
    }
}
