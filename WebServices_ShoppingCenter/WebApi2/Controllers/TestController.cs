using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCenter.WebServices.Base.Servicios;
using ShoppingCenter.WebServices.Base.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShoppingCenter.WebServices.Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/Test
        //La finalidad de este metodo es provar documentos sin depender de la consola
        [HttpGet]
        public ActionResult<string> Get()
        {
            //ULogger.instance.log(1, "INTERMEDIA", "POST", "http://localhost:50245/api/Test","Data response", "Resultado Exitoso", false, Constante.TipoTransaccion.SAP);
            //int i = (int) Constante.idTipoObjetoSap.ARTICULOS; 
            //return new string[] { "value1", "value2" };
            try
            {
                string respuesta = "ok";
                //respuesta = Servicios.Articulos.Instance.asociarABodegas();
                
                //Servicios.Articulos.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.Proveedor.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.FacturaCompras.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.FacturaDeudores.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.NotaCreditoClientes.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.PagosRecibidos.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.SolicitudTrasladoMateria.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.TrasladoMateriaConfirmacion.Instance.enviarDocumentosNoEnviadosSAPSync();
                //Servicios.SalidaMercancia.Instance.enviarDocumentosNoEnviadosSAPSync();
                return respuesta;
            }
             catch (Exception ex)
             {
                 return ex.Message;
             }

        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
