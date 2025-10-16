using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class BitacoraModel
    {
        public BitacoraModel( string tipoObjeto, string resultado, string mensaje, string codigoError, string iDObjetoSap, string iDObjetoIntermedia, string tipoTransaccion, string tipoOperacion)
        {
            TipoObjeto = tipoObjeto;
            Resultado = resultado;
            Mensaje = mensaje;
            CodigoError = codigoError;
            IDObjetoSap = iDObjetoSap;
            IDObjetoIntermedia = iDObjetoIntermedia;
            TipoTransaccion = tipoTransaccion;
            TipoOperacion = tipoOperacion;
        }

        public BitacoraModel() {
        }

        public string Correlativo { get; set; }
        public string TipoObjeto { get; set; }
        public DateTime Fecha { get; set; }
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
        public string CodigoError { get; set; }
        public string IDObjetoSap { get; set; }
        public string IDObjetoIntermedia{ get; set; }
        public string TipoTransaccion { get; set; }
        public string TipoOperacion { get; set; }
    }
}
