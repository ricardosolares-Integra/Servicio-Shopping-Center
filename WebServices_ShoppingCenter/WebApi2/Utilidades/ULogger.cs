using ShoppingCenter.WebServices.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class ULogger
    {
        private readonly static ULogger _instance = new ULogger();

        public static ULogger instance => _instance;


        public void log(
          int idTipoObjetoSap,
          string BdSap,
          string metodoHttp,
          string url,
          string request,
          string response,
          bool error,
          Constante.TipoTransaccion tipoTransaccion,
          string idObjetoSap,
          string idObjetoIntermedia = "-1",
          bool flag=false //esta variable indica si se ignora el get o no
            )
        {

            Log.logger.Info("*************** Log ***************");

            var parametros = UConsola.Instance.getParametrosGenerales();

             var valor = Int32.Parse(parametros.Find(e => e.Nombre == Constante.ParametrosGenerales.NivelLog.ToString()).Valor.ToString()); // Revisar si existe tabla con nivel de log
          
            string resultadoTransaccion = error ? Constante.ResultadoTransaccion.Fallido.ToString() : Constante.ResultadoTransaccion.Exitoso.ToString();
            string codeError = error ? "1" : "-1"; // 1 es error -1 exitoso
            string tipoOperacion = Constante.getTipoOperacion(metodoHttp);

            ULogWS.instance.recordLog(new LogWSModel(idTipoObjetoSap, BdSap, metodoHttp, url, request, response));

            UBitacora.instance.recordBitacora(new BitacoraModel(
                idTipoObjetoSap.ToString(),
                resultadoTransaccion,
                response,
                codeError,
                idObjetoSap,
                idObjetoIntermedia,
                tipoTransaccion.ToString(),
                (
                    tipoOperacion.ToLower().Equals(Constante.tipoOperacion.Crear.ToString().ToLower()) 
                    && (
                        url.Contains("SeriesService_GetDocumentSeries") 
                        || url.Contains("QueryService_PostQuery")
                        )
                ) 
                ? Constante.tipoOperacion.Listar.ToString() 
                : tipoOperacion
                ), flag);


            String str = $"URLSL: ({metodoHttp}) {url} \n" +
                $"Resultado Transaccion: {resultadoTransaccion}  \n";


            if ((error && valor == 0) || (error && valor == 2))
            {
                str += $"Tipo operacion: {tipoOperacion} \n" +
                    $"Tipo objeto: { idTipoObjetoSap.ToString()} \n" +
                    $"Request: {request.ToString()} \n" +
                    $"Response: { response.ToString()}\n";

            }
            else if (!error && valor == 2)
            {
                str += $"Request: {request.ToString()}";

            }
            else
            {

                str += $"Request: {request.ToString()} \n" +
                    $"Tipo objeto: {idTipoObjetoSap.ToString()}\n" +
                    $"Request: {request.ToString()} \n";
            }

            logger(valor, str);

            Log.logger.Info("*************** Finalizando Log ***************");


        }



        private int getValorLog()
        {

            var parametros = UConsola.Instance.getParametrosGenerales();


            return Int32.Parse(parametros.Find(e => e.Nombre == Constante.ParametrosGenerales.NivelLog.ToString()).Valor.ToString());

        }

        public void simpleLog(string text)
        {

            logger(getValorLog(), text);

        }

        private void logger(int valor, string text)
        {

            switch (valor)
            {

                case 1: //info
                    Log.logger.Info(text);
                    return;

                case 0: //error
                    Log.logger.Error(text);
                    return;

                case 2: //debug
                    Log.logger.Debug(text);
                    return;
            }

        }

    }
}
