using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCenter.WebServices.Base.Servicios;
using ShoppingCenter.WebServices.Base.Utilidades;

namespace ShoppingCenter.WebServices.Base.Controllers
{
    [Route("IGT/[controller]/[action]")]
    [ApiController]
    public class ServicioController : Controller
    {
        private readonly ILogger<ServicioController> _logger;

        public ServicioController(ILogger<ServicioController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ActionName("ServiceTimer")]
        public ActionResult serviceTimer()
        {
            try
            {
                Servicio.Instance.timmer();
                return Ok("Registro realizado de manera éxitosa");
            }
            catch (Exception ex)
            {
                // Obtener el mensaje de la excepción más interna
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                string mensajeError = inner.Message;

                // Registrar el error si es necesario
                new ErrorGlobal(
                    $"Excepción: '{mensajeError}'",
                    "",
                    ex.ToString(),
                    "",
                    (int)Constante.idTipoObjetoSap.ARTICULOS,
                    (int)Constante.idObjetoSapIndexes.ARTICULOS,
                    "",
                    true);


                return BadRequest($"Ocurrió un error: {ex.Message}");
            }
        }

    }
}