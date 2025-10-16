using ShoppingCenter.WebServices.Base.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class UBitacora
    {
        private readonly static UBitacora _instance = new UBitacora();

        public static UBitacora instance => _instance;
        private Conexion utilConexion;

        private UBitacora()
        {
            utilConexion = new Conexion();
        }

        public string recordBitacora(BitacoraModel bitacora, bool flag ) {

            if (bitacora.TipoOperacion.ToLower().Equals(Constante.tipoOperacion.Listar.ToString().ToLower()) && !flag) //si es get y false no se registra
                return "IS GET";

            string query = string.Format("INSERT INTO {0}.T_BITACORA (\"IdTipoObjetoSap\", \"Fecha\", \"ResultadoTransaccion\", \"IdObjetoSap\", \"IdObjetoIntermedia\", \"IdError\", \"Mensaje\", \"TipoTransaccion\", \"TipoOperacion\" ) VALUES (?,CURRENT_TIMESTAMP,?,?,?,?,?,?,?)",
              utilConexion.NombreDB("INTERMEDIA")); ;

            String mensaje = getMensaje(Int32.Parse(bitacora.CodigoError), bitacora);

            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@IdTipoObjetoSap", SqlDbType.Int, Int32.Parse( bitacora.TipoObjeto)));
            parametros.Add(new ParametroDBModel("@ResultadoTransaccion", SqlDbType.VarChar, bitacora.Resultado));
            parametros.Add(new ParametroDBModel("@IdObjetoSap", SqlDbType.Int, Int32.Parse(bitacora.IDObjetoSap)));
            parametros.Add(new ParametroDBModel("@IdObjetoIntermedia", SqlDbType.VarChar, bitacora.IDObjetoIntermedia));
            parametros.Add(new ParametroDBModel("@IdError", SqlDbType.Int, Int32.Parse(bitacora.CodigoError)));
            parametros.Add(new ParametroDBModel("@Mensaje", SqlDbType.VarChar, mensaje));
            parametros.Add(new ParametroDBModel("@TipoTransaccion", SqlDbType.VarChar, bitacora.TipoTransaccion));
            parametros.Add(new ParametroDBModel("@TipoOperacion", SqlDbType.VarChar, bitacora.TipoOperacion));

            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros); ;

        }

        private string getMensaje(int idError, BitacoraModel bitacora) {

            if (idError == 1) {

                return getMensajeError(bitacora);
            }

            return getMensajeExito(bitacora);
          
        }

        private string getMensajeError(BitacoraModel bitacora) {

            string ms = string.Empty;

            ms = $"Error al {bitacora.TipoOperacion.ToLower()} un documento tipo {Constante.getStrIdTipoObjetoSap(int.Parse(bitacora.TipoObjeto))} ";

            if (bitacora.Mensaje.Equals("")) return ms;

            if (!bitacora.Mensaje.Contains("}") && !bitacora.Mensaje.Contains("{")) return ms;

            JObject res = JObject.Parse(bitacora.Mensaje);

            if (!res.ContainsKey("error")) return ms;

            foreach (JProperty element in res["error"])
            {

                if (!element.Name.ToLower().Equals("message")) continue;

                foreach (JProperty item in element.Value)
                {
                    if (!item.Name.ToLower().Equals("value")) continue;
                    ms += $"- descripcion: {item.Value.ToString()}";
                    break;
                }
                break;
            }


            return ms;
        }


        private string getMensajeExito(BitacoraModel bitacora) {

            string ms = string.Empty;

            ms = $"Se realizo la accion {bitacora.TipoOperacion.ToLower()} un documento tipo {Constante.getStrIdTipoObjetoSap(int.Parse(bitacora.TipoObjeto))} de manera exitosa \n";

            if (bitacora.Mensaje.Equals("")) return ms;

            if (!bitacora.Mensaje.Contains("}") && !bitacora.Mensaje.Contains("{")) return ms;

            if (bitacora.TipoOperacion.ToLower().Equals(Constante.tipoOperacion.Actualizar.ToString().ToLower())) return ms ;

            JObject res = JObject.Parse(bitacora.Mensaje);

            if (!res.ContainsKey("DocEntry")) return ms;

            ms += $"(DocEntry: {res["DocEntry"].ToString()}) ";

            if (!res.ContainsKey("DocNum")) return ms;

            ms += $"(DocNum: {res["DocNum"].ToString()}) ";

            return ms;
        }


     

    }
}
