using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class ParametroDBModel
    {
        public ParametroDBModel(string Nombre, object Tipo, object Valor) {
            this.Nombre = Nombre;
            this.Tipo = Tipo;
            this.Valor = Valor;
        }

        public string Nombre { get; set; }
        public object Tipo { get; set; }
        public object Valor { get; set; }
    }
}
