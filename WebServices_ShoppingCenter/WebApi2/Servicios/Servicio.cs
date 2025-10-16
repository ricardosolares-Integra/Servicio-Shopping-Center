using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCenter.WebServices.Base.Models;
using ShoppingCenter.WebServices.Base.Utilidades;
using ShoppingCenter.WebServices.Base.Servicios;

namespace ShoppingCenter.WebServices.Base.Servicios
{
    public class Servicio
    {
        private readonly static Servicio _instance = new Servicio();
        private Conexion utilConexion;

        private Servicio()
        {
            utilConexion = new Conexion();
        }

        public static Servicio Instance
        {
            get
            {
                return _instance;
            }
        }

        public void timmer()
        {
            int TipoEnvio = 0;//1=Automatico,0=Manual
            int Ejecucion = 0;//1=Diario,2=Semanal,3=Inmediato

            string Dias = string.Empty;
            string Horarios = string.Empty;
            string Error = "";

            string query = $"SELECT * FROM {utilConexion.NombreDB("INTERMEDIA")}.T_PARAMETROS_ENVIO ";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_PARAMETROS_ENVIO", "INTERMEDIA");
            foreach (DataRow item in filas)
            {
                TipoEnvio = Convert.ToInt32(item["TipoEnvio"]);
                Ejecucion = Convert.ToInt32(item["Ejecucion"]);
                Dias = item["DiasEjecucion"].ToString();//Posiciones del arreglo: Domingo,Lunes,Martes,Miercoles,Jueves,Viernes,Sabado
                Horarios = item["Horario"].ToString();
            }

            if (TipoEnvio.Equals(0))
            {
                return;
            }

            string HoraActual = DateTime.Now.ToString("HH:mm");
            List<string> horarioList = Horarios.Split(',').ToList();

            switch (Ejecucion)
            {
                case 3: // Inmediato
                    try
                    {
                        EnviarTodos();
                    }
                    catch (Exception ex)
                    {
                        string mensajeError = Documentos.ObtenerMensajeError(ex);

                        // Puedes registrar el error o simplemente dejarlo en el throw
                       
                        // Puedes loguear si quieres
                        new ErrorGlobal(
                            $"Error en ejecución inmediata: {ex.Message}",
                            "GET",
                            ex.ToString(),
                            "/ServiceTimer",
                            (int)Constante.idTipoObjetoSap.FACTURADEUDORES,
                            (int)Constante.idObjetoSapIndexes.FACTURADEUDORES,
                            "",
                            true
                        );
                        //throw new Exception(ex);// ⚠️ IMPORTANTE: relanza para que el controller lo atrape
                        throw new Exception(mensajeError);
                    }
                    break;


                case 1://Diario
                    foreach(string h in horarioList)
                    {
                        if (h.Equals(HoraActual))
                        {
                            EnviarTodos();
                        }
                    }
                    break;

                case 2://Semanal
                    int DiaActual = (int)DateTime.Now.DayOfWeek; //obtiene el numero de dia
                    // 0=Domingo,1=Lunes,2=Martes,3=Miercoles,4=Jueves,5=Viernes,6=Sabado
                    string[] DiasSplit = Dias.Split(',');
                    // 1=Enviar,0=NoEnviar
                    if (DiasSplit[DiaActual].Equals("0"))
                    {
                        return;
                    }

                    foreach (string h in horarioList)
                    {
                        if (h.Equals(HoraActual))
                        {
                            EnviarTodos();
                        }
                    }
                    break;

                default:
                    return;

            }


        }

        //public void EnviarTodos()
        //{
        //    //string query = $"SELECT \"IdTipoObjetoSap\" FROM {utilConexion.NombreDB("INTERMEDIA")}.T_TIPO_OBJETO WHERE \"Estado\" = 'A' AND \"EnEjecucion\" = FALSE ORDER BY \"Orden\" ";
        //    string query = $"SELECT \"IdTipoObjetoSap\" FROM {utilConexion.NombreDB("INTERMEDIA")}.T_TIPO_OBJETO WHERE  \"EnEjecucion\" = FALSE ORDER BY \"Orden\" ";
        //    DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_TIPO_OBJETO", "INTERMEDIA");
        //    foreach (DataRow item in filas)
        //    {
        //        Enviar(Convert.ToInt32(item["IdTipoObjetoSap"]));
        //    }
        //}

        public void EnviarTodos()
        {
            string query = $"SELECT \"IdTipoObjetoSap\" FROM {utilConexion.NombreDB("INTERMEDIA")}.T_TIPO_OBJETO WHERE \"EnEjecucion\" = FALSE ORDER BY \"Orden\" ";
            DataRowCollection filas = utilConexion.GetDataRowCollection(query, "T_TIPO_OBJETO", "INTERMEDIA");

            foreach (DataRow item in filas)
            {
                Enviar(Convert.ToInt32(item["IdTipoObjetoSap"])); // Si lanza, se propaga
            }
        }


        public void Enviar(int idObjetoSAP)
        {
            switch (idObjetoSAP)
            {
                case (int)Constante.idTipoObjetoSap.ARTICULOS:
                    Articulos.Instance.enviarDocumentosNoEnviadosSAPSync();
                    break;
  

                case (int)Constante.idTipoObjetoSap.FACTURADEUDORES:
                    FacturaDeudores.Instance.enviarDocumentosNoEnviadosSAPSync();
                    break;

            }
        } 
    }
}
