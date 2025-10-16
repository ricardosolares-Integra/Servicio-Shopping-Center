using ShoppingCenter.WebServices.Base.Models;
using ShoppingCenter.WebServices.Base.Utilidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAPB1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Servicios
{
    public class FacturaDeudores : Documentos
    {
        private readonly static FacturaDeudores _instance = new FacturaDeudores();

        private FacturaDeudores() { }
        public static FacturaDeudores Instance
        {
            get
            {
                return _instance;
            }
        }

        private void detalleFacturasNoEnviadas(string idFactura, SAPB1.Document invoice)
        {
            string query = $"select \"IdArticulo\",\"CodigoImpuesto\" ,\"Cantidad\", \"CodigoBarras\", \"DescripcionCB\", \"PreciodLista\", \"Moneda\", \"Monto\" From {utilConexion.NombreDB("INTERMEDIA")}.T_FACTURACLI_FILAS WHERE \"IdFactura\" = '{idFactura}'" +
            $"ORDER BY \"Fila\";";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_FACTURACLI_FILAS", "INTERMEDIA");
            if (filas.Count == 0)
            {
                throw new RowNotInTableException($"No hay datos en tabla de detalle del documento {idFactura}.");
            }
            else
            {
                foreach (DataRow row in filas)
                {
                    SAPB1.DocumentLine line = new SAPB1.DocumentLine();

                    line.ItemCode = row["IdArticulo"].ToString();
                    line.Quantity = double.Parse(row["Cantidad"].ToString());
                    line.UoMCode = row["CodigoBarras"].ToString(); // SE UTILIZA PARA GUARDAR EL CÓDITGO DE BARRAS
                    line.U_DesCodBarra = row["DescripcionCB"].ToString();
                    line.U_PreciodLista = Convert.ToInt32(row["PreciodLista"]);
                    line.Currency = row["Moneda"].ToString();
                    line.PriceAfterVAT = double.Parse(row["Monto"].ToString());
                    line.TaxCode = row["CodigoImpuesto"].ToString();
                    invoice.DocumentLines.Add(line);
                }
            }
        }

        public override void procesarFila(DataRow row, string bdIntermedia)
        {
            string idFactura = row["IdFactura"].ToString();
            string DocEntry = "NON HAY CONS";
            incrementarIntento("T_FACTURACLI", "IdFactura", idFactura, bdIntermedia);


            bool enviado = Convert.ToBoolean(row["Enviado"]);
            SAPB1.Document invoice = new SAPB1.Document();
            if (!enviado)
            {
                try
                {
                    invoice.U_IdExterno = row["IdFactura"].ToString();
                    invoice.CardCode = row["CodigoCliente"].ToString();
                    invoice.TaxDate = Convert.ToDateTime(row["FechaDocumento"].ToString());
                    invoice.U_Bodega = row["Bodega"].ToString();
                    invoice.U_FE_TipoDoc = row["TipoDTE"].ToString();
                    invoice.U_FE_NoControl = row["NumeroControl"].ToString();
                    invoice.U_FE_CodGeneracion = row["CodigoGeneracion"].ToString();
                    invoice.U_FE_SelloRecepcion = row["SelloMH"].ToString();
                    invoice.U_NumeroControl_Del = row["NumeroControl_Del"].ToString();
                    invoice.U_NumeroControl_Al = row["NumeroControl_Al"].ToString();
                    invoice.U_Correlativo_Del = row["Correlativo_Del"].ToString();
                    invoice.U_Correlativo_Al = row["Correlativo_Al"].ToString();
                    invoice.U_Serie_Del = row["Serie_Del"].ToString();
                    invoice.U_Serie_Al = row["Serie_Al"].ToString();
                    invoice.U_TipoDocIdentificacion = row["TipoDocIdentificacion"].ToString();
                    invoice.U_FE_NIT = row["NIT"].ToString();
                    invoice.U_FE_NRC = row["NRC"].ToString();
                    invoice.U_FE_Nombre = row["Nombre"].ToString();
                    invoice.U_Direccion_Complemento = row["Direccion_Complemento"].ToString();
                    invoice.U_Direccion_Departamento = row["Direccion_Departamento"].ToString();
                    invoice.U_Direccion_Municipio = row["Direccion_Municipio"].ToString();
                    invoice.U_CodigoActividadEconomica = row["CodigoActividadEconomica"].ToString();
                    invoice.U_giro = row["DescActividadEconomica"].ToString();
                    invoice.U_Correo = row["Correo"].ToString();
                    invoice.U_PuntoVenta = row["PuntoVenta"].ToString();
                    invoice.U_Establecimiento = row["Establecimiento"].ToString();


                    detalleFacturasNoEnviadas(idFactura, invoice);
                    addCamposUsuario(invoice, row, enviado);

                    string data = serializarObjeto(invoice);

                    finalizarIntento(
                        enviado,
                        bdIntermedia,
                        "POST",
                        data,
                        "/Invoices",
                        (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                        (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                        idFactura,
                        "DocEntry"
                    );
                    ActualizarIntermedia(idFactura, utilConexion.NombreDB("INTERMEDIA"));
                }
                catch (Exception ex)
                {
                    throw;

                }
            }
        }


        public override void enviarDocumentosNoEnviadosSAP()
        {
            int MaxIntentos = Convert.ToInt32(getParametroGenral(Constante.ParametrosGenerales.MaxIntentos));
            string bdIntermedia = utilConexion.NombreDB("INTERMEDIA");
            string query = $"SELECT \"IdFactura\", \"CodigoCliente\", \"FechaDocumento\", \"Bodega\", \"TipoDTE\", \"NumeroControl\", \"CodigoGeneracion\", \"SelloMH\", \"NumeroControl_Del\"" +
                $", \"NumeroControl_Al\" , \"Correlativo_Del\", \"Correlativo_Al\", \"Serie_Del\", \"Serie_Al\", \"TipoDocIdentificacion\", \"NIT\", \"NRC\", \"Nombre\", \"Direccion_Complemento\", \"Direccion_Departamento\"" +
                $", \"Direccion_Municipio\", \"CodigoActividadEconomica\", \"DescActividadEconomica\", \"Correo\", \"PuntoVenta\", \"Establecimiento\", \"FechaCreacion\", \"Enviado\", \"SapDocEntry\"" +
                $"FROM {utilConexion.NombreDB("INTERMEDIA")}.T_FACTURACLI " +
                $"WHERE \"Enviar\" = TRUE AND \"Intento\" < {MaxIntentos}";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_FACTURACLI", "INTERMEDIA");


            foreach (DataRow row in filas)
            {
                string jsonError = "";
                try
                {
                    procesarFila(row, bdIntermedia);
                }
                catch (Exception e)
                {
                    jsonError = e.Message;

                    new ErrorGlobal(
                        e.Message,
                        "POST",
                        jsonError,
                        "/Invoices",
                        (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                        (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                        row["idFactura"].ToString(),
                        true);

                    throw;
                }
            }


        }

        public override string updateIntermedia(string idTabla, string idSAP, string nameBD)
        {
            string query = string.Format("UPDATE {0}.\"T_FACTURACLI\" SET \"SapDocEntry\"=?,\"Enviado\"=?,\"Enviar\"=? WHERE \"IdFactura\" = ?", nameBD);
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@SapDocEntry", SqlDbType.Int, int.Parse(idSAP)));
            parametros.Add(new ParametroDBModel("@Enviado", SqlDbType.Bit, 1));
            parametros.Add(new ParametroDBModel("@Enviar", SqlDbType.Bit, 0));
            parametros.Add(new ParametroDBModel("@IdFactura", SqlDbType.VarChar, idTabla));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }

        public string ActualizarIntermedia(string idTabla, string nameBD)
        {

            string query = string.Format("UPDATE {0}.\"T_FACTURACLI_FILAS\" SET \"Enviado\"=?,\"Enviar\"=? WHERE \"IdFactura\" = ?", nameBD);
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@Enviado", SqlDbType.Bit, 1));
            parametros.Add(new ParametroDBModel("@Enviar", SqlDbType.Bit, 0));
            parametros.Add(new ParametroDBModel("@IdFactura", SqlDbType.VarChar, idTabla));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);

        }

        public override void addCamposUsuario(object document, DataRow row, bool enviado)
        {
            if (enviado) return;

            ((SAPB1.Document)document).U_EsInterfaz = "S";
        }


        public int getDocEntry(string idMenuLink, string documentoQueLlama)
        {

            string query = $"/Invoices?$select=DocEntry&$filter=U_IdMenuLink eq '{idMenuLink}'";
            string respuesta = utilConexion.RandomRest(
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                idMenuLink);

            JObject res = JObject.Parse(respuesta);
            JToken value = res["value"];
            if (res.ContainsKey("value") && value.Count<object>() > 0)
            {
                foreach (JProperty element in res["value"][0])
                {
                    if (!element.Name.ToLower().Equals("docentry")) continue;
                    return int.Parse(element.Value.ToString());
                }
            }

            throw new ErrorGlobal(
                $"No se encontró docEntry para el U_IdMenuLink '{idMenuLink}'",
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                documentoQueLlama,
                true);

        }

        public Collection<DocumentLine> getDocumentLines(int docEntry, string documentoQueLlama)
        {
            Collection<DocumentLine> LineasFactura = new Collection<DocumentLine>();
            string query = $"/Invoices({docEntry})/DocumentLines";
            string respuesta = utilConexion.RandomRest(
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                documentoQueLlama);

            JObject res = JObject.Parse(respuesta);
            if (res.ContainsKey("DocumentLines"))
            {
                foreach (JObject linea in res["DocumentLines"].ToArray())
                {
                    DocumentLine line = new DocumentLine();
                    line.LineNum = Convert.ToInt32(linea["LineNum"].ToString());
                    line.ItemCode = linea["ItemCode"].ToString();
                    line.PriceAfterVAT = Convert.ToDouble(linea["PriceAfterVAT"].ToString());
                    LineasFactura.Add(line);
                }
            }
            else
            {
                throw new ErrorGlobal(
                    $"No se encontró Detalle de líneas para la factura SAP '{docEntry}'",
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                    (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                    documentoQueLlama,
                    true);
            }
            return LineasFactura;
        }
    }

}
