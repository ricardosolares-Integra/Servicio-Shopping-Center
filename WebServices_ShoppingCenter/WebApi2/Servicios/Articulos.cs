using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCenter.WebServices.Base.Models;
using ShoppingCenter.WebServices.Base.Utilidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ShoppingCenter.WebServices.Base.Servicios
{
    public class Articulos : Documentos
    {
        private readonly static Articulos _instance = new Articulos();

        private Articulos() { }

        public static Articulos Instance
        {
            get
            {
                return _instance;
            }
        }

        public override void addCamposUsuario(object document, DataRow row, bool enviado)
        {
            if (enviado) return;

            ((SAPB1.Item)document).U_EsInterfaz = "S";
            
        }


        public override void procesarFila(DataRow row, string bdIntermedia)
         {
            string idArticulo = row["IdArticulo"].ToString();
            string grupo = row["IdGrupo"].ToString();
            incrementarIntento("T_ARTICULO", "IdArticulo", idArticulo, bdIntermedia);


            SAPB1.Item Articulo = new SAPB1.Item();
            bool enviado = Convert.ToBoolean(row["Enviado"]);
            if (!enviado)
            {
                Articulo.ItemCode = row["IdArticulo"].ToString();
                Articulo.ItemName = row["Descripcion"].ToString();
                Articulo.ItemsGroupCode = Convert.ToInt32(grupo);
                Articulo.PurchaseItem = bool.Parse(row["EsCompra"].ToString()) ? "tYES" : "tNO";
                Articulo.SalesItem = bool.Parse(row["EsVenta"].ToString()) ? "tYES" : "tNO";
                Articulo.U_Id_Externo = row["IdArticulo"].ToString();
                Articulo.BarCode = row["CodigoBarras"].ToString();
                Articulo.U_Bodega = row["Bodega"].ToString();

                addCamposUsuario(Articulo, row, enviado);

                string data = serializarObjeto(Articulo);

                finalizarIntento(
                   enviado,
                   bdIntermedia,
                   enviado ? "PATCH" : "POST",
                   data,
                   "/Items",
                   (int)Constante.idTipoObjetoSap.ARTICULOS,
                   (int)Constante.idObjetoSapIndexes.ARTICULOS,
                   idArticulo,
                   "ItemCode"
               );

         
        }else{ }


        }

        public override void enviarDocumentosNoEnviadosSAP()
        {
            int MaxIntentos = Convert.ToInt32(getParametroGenral(Constante.ParametrosGenerales.MaxIntentos));
            string bdIntermedia = utilConexion.NombreDB("INTERMEDIA");
            string query = $"" +
                $"SELECT \"IdArticulo\",\"Descripcion\",\"IdGrupo\",\"EsCompra\",\"EsVenta\",\"UniMedida\",\"Bodega\",\"CodigoBarras\",\"Estado\",\"Enviado\",\"ItemCodeSap\" " +
                $"FROM {bdIntermedia}.T_ARTICULO " +
                $"WHERE \"Enviar\" = TRUE and \"Intento\" < {MaxIntentos}";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_ARTICULO", "INTERMEDIA");

            foreach (DataRow row in filas)
            {
                try
                {
                    procesarFila(row, bdIntermedia);
                }
                catch (Exception e) {
                    new ErrorGlobal(
                        e.Message,
                        "POST",
                        e.ToString(),
                        "/Items",
                        (int)Constante.idTipoObjetoSap.ARTICULOS,
                        (int)Constante.idObjetoSapIndexes.ARTICULOS,
                        row["IdArticulo"].ToString(),
                        true);
                }
            }
        }

        public override string updateIntermedia(string idTabla, string idSAP, string nameBD)
        {
            string query = string.Format("UPDATE {0}.\"T_ARTICULO\" SET \"ItemCodeSap\"=?,\"Enviado\"=?,\"Enviar\"=? WHERE \"IdArticulo\" = ?", nameBD);
            List<ParametroDBModel> parametros = new List<ParametroDBModel>();
            parametros.Add(new ParametroDBModel("@ItemCodeSap", SqlDbType.VarChar, idSAP));
            parametros.Add(new ParametroDBModel("@Enviado", SqlDbType.Bit, 1));
            parametros.Add(new ParametroDBModel("@Enviar", SqlDbType.Bit, 0));
            parametros.Add(new ParametroDBModel("@IdArticulo", SqlDbType.VarChar, idTabla));
            return utilConexion.EjecutarComandoGenerico(query, "INTERMEDIA", parametros);
        }


        public string getItemCode(string idMenuLink, string documentoQueLlama) {
            try
            {
                string respuesta = utilConexion.RandomRest(
                "GET",
                "",
                $"/Items?$select=ItemCode&$filter=U_IdMenuLink eq '{idMenuLink}'",
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                idMenuLink);

                JObject res = JObject.Parse(respuesta);

                JToken value = res["value"];

                if (res.ContainsKey("value") && value.Count<object>() > 0)
                {

                    foreach (JProperty element in res["value"][0])
                    {
                        if (!element.Name.ToLower().Equals("itemcode")) continue;
                        return element.Value.ToString();
                    }
                }
            }
            catch (Exception ex) {
                new ErrorGlobal(
                    $"Excepcion '{ex.Message}'",
                    "GET",
                    "",
                    $"/Items?$select=ItemCode&$filter=U_IdMenuLink eq '{idMenuLink}'",
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    documentoQueLlama,
                    true);
                new ErrorGlobal(
                    $"No se encontro el el itemCode para el U_IdMenuLink '{idMenuLink}'",
                    "GET",
                    "",
                    $"/Items?$select=ItemCode&$filter=U_IdMenuLink eq '{idMenuLink}'",
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    documentoQueLlama,
                    true);
            }

            throw new ErrorGlobal(
                $"No se encontro el el itemCode para el U_IdMenuLink '{idMenuLink}'",
                "GET",
                "",
                $"/Items?$select=ItemCode&$filter=U_IdMenuLink eq '{idMenuLink}'",
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                documentoQueLlama,
                true);

        }

        public int getGroupCode(string idMenuLink, string GroupName, string documentoQueLlama)
        {
            string query = $"/ItemGroups" +
                $"?$top=1" +
                $"&$select=Number" +
                $"&$filter=GroupName eq '{GroupName}'";

            try
            {
                string respuesta = utilConexion.RandomRest(
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                idMenuLink);

                JObject res = JObject.Parse(respuesta);

                JToken value = res["value"];

                if (res.ContainsKey("value") && value.Count<object>() > 0)
                {

                    foreach (JProperty element in res["value"][0])
                    {
                        if (!element.Name.ToLower().Equals("number")) continue;
                        return Convert.ToInt32(element.Value.ToString());
                    }
                }
            }
            catch (Exception ex) {
                new ErrorGlobal(
                $"Excepcion '{ex.Message}'",
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                documentoQueLlama,
                true);

                new ErrorGlobal(
                    $"No se encontro grupo de artículo con nombre '{GroupName}'",
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    documentoQueLlama,
                    true);
            }

            throw new ErrorGlobal(
                $"No se encontro grupo de artículo con nombre '{GroupName}'",
                "GET",
                "",
                query,
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                documentoQueLlama,
                true);

        }

        public int getSeries(string idMenuLink, string GroupName, string documentoQueLlama)
        {
            string seriesName = null;
            string query = $"/ItemGroups" +
                $"?$top=1" +
                $"&$select=U_Series" +
                $"&$filter=GroupName eq '{GroupName}'";
            try
            {


                string respuesta = utilConexion.RandomRest(
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    idMenuLink);

                JObject res = JObject.Parse(respuesta);

                JToken value = res["value"];

                if (res.ContainsKey("value") && value.Count<object>() > 0)
                {
                    foreach (JProperty element in res["value"][0])
                    {
                        if (!element.Name.ToLower().Equals("u_series")) continue;
                        seriesName = element.Value.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(seriesName))
                {
                    query = $"/SeriesService_GetDocumentSeries";
                    string data = "{\"DocumentTypeParams\": {\"Document\": \"4\"}}";
                    respuesta = utilConexion.RandomRest(
                        "POST",
                        data,
                        query,
                        (int)Constante.idTipoObjetoSap.ARTICULOS,
                        (int)Constante.idObjetoSapIndexes.ARTICULOS,
                        idMenuLink);

                    res = JObject.Parse(respuesta);
                    if (res.ContainsKey("value"))
                    {
                        JArray jArray = (JArray)res["value"];
                        foreach (JObject serie in jArray)
                        {
                            if (seriesName.Equals(serie["Name"].ToString()))
                            {
                                return Convert.ToInt32(serie.ContainsKey("Series") ? serie["Series"].ToString() : serie["SeriesProperty"].ToString());
                            }

                        }
                    }
                }
            }
            catch (Exception ex) {
                new ErrorGlobal(
                                $"Excepcion: '{ex.Message}'",
                                "GET",
                                "",
                                query,
                                (int)Constante.idTipoObjetoSap.ARTICULOS,
                                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                                documentoQueLlama,
                                true);
            }
            throw new ErrorGlobal(
                                $"No se encontró serie '{seriesName}' para grupo de artículos '{GroupName}'",
                                "GET",
                                "",
                                query,
                                (int)Constante.idTipoObjetoSap.ARTICULOS,
                                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                                documentoQueLlama,
                                true);

        }


        public double getInventarioPorBodega(string ItemCode, string bodega, string idMenuLink)
        {
            double inventario = 0;
            string data = "" +
                    "{\"QueryPath\": \"$crossjoin(Items, Items/ItemWarehouseInfoCollection)\"," +
                    "\"QueryOption\": \"" +
                        "$expand = Items($select = ItemCode),Items/ItemWarehouseInfoCollection($select = InStock) " +
                        "&$filter = Items/ItemCode eq Items/ItemWarehouseInfoCollection/ItemCode " +
                        "and Items/ItemWarehouseInfoCollection/WarehouseCode eq " +
                        $"'{bodega}' " +
                        $"and Items/ItemCode eq '{ItemCode}'\"" +
                        "}";

            try
            {
                string respuesta = utilConexion.RandomRest(
                "POST",
                data,
                $"/QueryService_PostQuery",
                (int)Constante.idTipoObjetoSap.ARTICULOS,
                (int)Constante.idObjetoSapIndexes.ARTICULOS,
                idMenuLink);

                JObject res = JObject.Parse(respuesta);
                JToken value = res["value"];
                if (res.ContainsKey("value") && value.Count<object>() > 0)
                {
                    res = JObject.Parse(res["value"][0].ToString());
                    if (res.ContainsKey("Items/ItemWarehouseInfoCollection")) {
                        foreach (JProperty element in res["Items/ItemWarehouseInfoCollection"])
                        {
                            if (!element.Name.ToLower().Equals("InStock".ToLower())) continue;
                            inventario = Convert.ToDouble(element.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorGlobal(
                  $"Excepcion '{ex.Message}'",
                  "GET",
                  data,
                  "/QueryService_PostQuery",
                  (int)Constante.idTipoObjetoSap.ARTICULOS,
                  (int)Constante.idObjetoSapIndexes.ARTICULOS,
                  idMenuLink,
                  true);
                new ErrorGlobal(
                  $"No se encontró inventario para el artículo '{ItemCode}' en almacén '{bodega}'",
                  "GET",
                  data,
                  "/QueryService_PostQuery",
                  (int)Constante.idTipoObjetoSap.ARTICULOS,
                  (int)Constante.idObjetoSapIndexes.ARTICULOS,
                  idMenuLink,
                  true);
            }  
            return inventario;
        }

        public double getCostoArticuloSinTransformar(string itemCodeTransformado, string bodega, string idMenuLink)
        {
            double costo;
            string query = $"/Items('{itemCodeTransformado}')?$select=U_IdSinTransformar";
            string itemCodeSinTransformar = string.Empty;
            try
            {
                string respuesta = utilConexion.RandomRest(
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    idMenuLink
                );

                JObject res = JObject.Parse(respuesta);
                itemCodeSinTransformar = res["U_IdSinTransformar"].ToString();
                query = $"/Items('{itemCodeSinTransformar}')?$select=MovingAveragePrice";
                respuesta = utilConexion.RandomRest(
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    idMenuLink
                );

                res = JObject.Parse(respuesta);
                costo = Convert.ToDouble(res["MovingAveragePrice"].ToString());
            }
            catch (Exception ex)
            {

                string mensaje = string.IsNullOrEmpty(itemCodeSinTransformar)
                    ? $"No se encontró artículo sin transformar para el artículo transformado '{itemCodeTransformado}'"
                    : $"No se encontró costo de artículo sin transformar '{itemCodeTransformado}' para el artículo transformado '{itemCodeTransformado}'";
                new ErrorGlobal(
                  $"Excepcion '{ex.Message}'",
                  "GET",
                  "",
                  query,
                  (int)Constante.idTipoObjetoSap.ARTICULOS,
                  (int)Constante.idObjetoSapIndexes.ARTICULOS,
                  idMenuLink,
                  true);
                throw new ErrorGlobal(
                    mensaje,
                    "GET",
                    "",
                    query,
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    idMenuLink,
                    true);
            }
            return costo;
        }
    }
}
