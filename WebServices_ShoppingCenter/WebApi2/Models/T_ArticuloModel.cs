using System;

namespace ShoppingCenter.Models
{
    public class T_ArticuloModel
    {
        public string IdArticulo { get; set; }
        public string Descripcion { get; set; }
        public string IdGrupo { get; set; }
        public bool EsCompra { get; set; }
        public bool EsVenta { get; set; }
        public decimal? UniMedida { get; set; }  
        public decimal? Bodega { get; set; }     
        public string CodigoBarras { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool? Enviar { get; set; }        
        public bool? Enviado { get; set; } 
        public string ItemCodeSap { get; set; }
        public int NoIntento { get; set; }
    }

}
