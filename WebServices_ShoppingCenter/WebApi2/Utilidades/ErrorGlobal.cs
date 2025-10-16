using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{

    
    public class ErrorGlobal: ArgumentException
    {
        private readonly Conexion _utilConexion = new Conexion();
        public ErrorGlobal(string message, string metodo, string data, string URL, int idTipoObjetoSap, int idObjetoSap, string idObjetoIntermedia) : base(message)
        {
            ULogger.instance.log(
                   idTipoObjetoSap,
                   _utilConexion.NombreDB("INTERMEDIA"),
                   metodo,
                   URL,
                   data,
                   message,
                   true,
                   Constante.TipoTransaccion.SAP,
                   idObjetoSap.ToString(),
                   idObjetoIntermedia, true
                   );
        }


        public ErrorGlobal(string message, string metodo, string data, string URL, int idTipoObjetoSap, int idObjetoSap, string idObjetoIntermedia, bool flag) : base(message)
        {
            ULogger.instance.log(
                   idTipoObjetoSap,
                   _utilConexion.NombreDB("INTERMEDIA"),
                   metodo,
                   URL,
                   data,
                   createJSONError(message),
                   true,
                   Constante.TipoTransaccion.SAP,
                   idObjetoSap.ToString(),
                   idObjetoIntermedia,
                   true
                   );
        }


        //private string createJSONError(string msj) {

        //    string str = string.Empty;
        //    try
        //    {
        //        if (msj.StartsWith("Excepcion '{")) {
        //            msj = msj.Substring(11, msj.Length - 12);
        //        }
        //        JObject res = JObject.Parse(msj);
        //        str = msj;
        //    }
        //    catch (Exception ex) { 
        //        str = "{ \"error\" : { \n" +
        //                                " \"message\" : { \n" + $" \"value\" : \"{msj}\"" + "} \n" +
        //                            "} \n" +
        //               "}";
        //    }
        //    return str;

        //}

        private string createJSONError(string msj)
        {
            try
            {
                if (msj.StartsWith("Excepcion '{"))
                {
                    msj = msj.Substring(11, msj.Length - 12);
                }

                // Intentar parsearlo directamente
                JObject.Parse(msj); // si no lanza, ya es JSON válido
                return msj;
            }
            catch
            {
                // Si no es JSON, escapar comillas y crear uno bien formado
                string sanitized = msj.Replace("\"", "\\\"").Replace("\n", " ").Replace("\r", " ");
                return $"{{ \"error\": {{ \"message\": {{ \"value\": \"{sanitized}\" }} }} }}";
            }
        }


    }
}
