using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class LogWSModel
    {
        private int tipoObjeto;
        private string bd;
        private string metodoHttp;
        private string url;
        private string request;
        private string response;

        public LogWSModel(int tipoObjeto, string bd, string metodoHttp, string url, string request, string response)
        {
            this.tipoObjeto = tipoObjeto;
            this.bd = bd;
            this.metodoHttp = metodoHttp;
            this.url = url;
            this.request = request;
            this.response = response;
        }

        public int TipoObjeto { get => tipoObjeto; set => tipoObjeto = value; }
        public string Bd { get => bd; set => bd = value; }
        public string MetodoHttp { get => metodoHttp; set => metodoHttp = value; }
        public string Url { get => url; set => url = value; }
        public string Request { get => request; set => request = value; }
        public string Response { get => response; set => response = value; }
    }
}
