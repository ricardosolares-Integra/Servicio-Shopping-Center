using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Utilidades
{
    public class Constante
    {
        //Constante tipo operacion
    
        public enum tipoOperacion {
            Listar,
            Crear,
            Eliminar,
            Actualizar,
            Otro
        }

        //Constante tipo transaccion
        public enum TipoTransaccion
        {
            SAP,
            Intermedia
        }
    
        public enum ResultadoTransaccion
        {
            Exitoso,
            Fallido
        }

        //el numero es el codigo
        //representacion de codigo de la tabla T_PARAMETROS
        public enum ParametrosGenerales {
            NivelLog = 1,
            SNCliente = 2,
            SNVendedor = 3,
            CPagoProv = 4,
            SerieProv = 5,
            WHCentral = 6,
            MaxIntentos = 7,
            CtaSalidaMerc = 8
        }

        public static string getTipoOperacion(string metodo) {

            switch (metodo) {

                case "GET":
                    return tipoOperacion.Listar.ToString();
                case "DELETE":
                    return tipoOperacion.Eliminar.ToString();
                case "POST":
                    return tipoOperacion.Crear.ToString();
                case "PATCH":
                    return tipoOperacion.Actualizar.ToString();
            }

            return tipoOperacion.Otro.ToString();
        }

        public static string getStrIdTipoObjetoSap(int idObjeto) {

            switch (idObjeto) {

                case 4:
                    return "articulos";

                case 2:
                    return "proveedores";

                case 13:
                    return "factura deudores/cli";

                case 18:
                    return "factura compras";

                case 14:
                    return "nota credito/cli";

                case 24:
                    return "pagos recibidos";

                case 1250000001:
                    return "solicitud traslado de materia";

                case 59:
                    return "ingreso mercancia";

                case 60:
                    return "salida mercancia";

                case 64:
                    return " bodega";

                case 67:
                    return " traslado de materia";

                default:
                    return "documento no agregado a nivel de desarrollo";
            }
        }

        //getStrTipoObjeto usa el id de el enum idTipoObjetoSap

        public enum idTipoObjetoSap {

            ARTICULOS=4,
            BODEGAS=64,
            FACTURADEUDORES = 13,
            PROVEEDORES = 2,
            FACTURACOMPRAS = 18,
            NOTACREDITOCLIENTE = 14,
            PAGORECIBIDOS = 24,
            SOLTRASLADOMATERIA = 1250000001,
            TRASLADOMATERIA = 67,
            INGRESOMERCANCIA = 59,
            SALIDAMERCANCIA = 60
        }

        public enum idObjetoSapIndexes
        {

            ARTICULOS = 7,
            BODEGAS=2,
            PROVEEDORES = 11,
            FACTURADEUDORES = 12,
            FACTURACOMPRAS = 10,
            NOTACREDITOCLIENTE = 11,
            PAGORECIBIDOS = 6,
            SOLTRASLADOMATERIA = 9,
            TRASLADOMATERIA = 11,
            INGRESOMERCANCIA = 59,
            SALIDAMERCANCIA = 10
        }



    }
}
