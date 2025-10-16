using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class ParametrosEnvioModel
    {
        public int IdServicio { get; set; }
        public string Ejecucion { get; set; }
        public string DiasEjecucion { get; set; }
        public string TipoEnvio { get; set; }
        public string Horario { get; set; }
        public int TiempoEspera {get;set;} 
    }
}
