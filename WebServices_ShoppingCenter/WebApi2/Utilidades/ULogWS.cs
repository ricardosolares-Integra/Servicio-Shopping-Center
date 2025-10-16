using ShoppingCenter.WebServices.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class ULogWS
    {
        private readonly static ULogWS _instance = new ULogWS();

        public static ULogWS instance => _instance;

        private Conexion utilConexion;

        private ULogWS()
        {
            utilConexion = new Conexion();
        }
        public string recordLog(LogWSModel log)
        {
            string query = string.Format("INSERT INTO {0}.T_LOG_CONSUMO_SL (\"IdTipoObjetoSap\", \"Fecha\", \"BdSap\", \"MetodoHtml\", \"Url\", \"Request\", \"Response\" ) VALUES (?,CURRENT_TIMESTAMP,?,?,?,?,?)",
              utilConexion.NombreDB("INTERMEDIA"));

            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@IdTipoObjetoSap", SqlDbType.Int, log.TipoObjeto));
            parametros.Add(new ParametroDBModel("@BdSap", SqlDbType.VarChar, log.Bd));
            parametros.Add(new ParametroDBModel("@MetodoHtml", SqlDbType.VarChar, log.MetodoHttp));
            parametros.Add(new ParametroDBModel("@Url", SqlDbType.VarChar, log.Url));
            parametros.Add(new ParametroDBModel("@Request", SqlDbType.VarChar, log.Request));
            parametros.Add(new ParametroDBModel("@Response", SqlDbType.VarChar, log.Response));

            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }
    }
}
