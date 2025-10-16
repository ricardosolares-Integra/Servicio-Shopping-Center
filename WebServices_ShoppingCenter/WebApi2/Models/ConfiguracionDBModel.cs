using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class ConfiguracionDBModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Servidor Base de Datos")]
        public string Server { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Puerto")]
        public string Port { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "DNS")]
        public string DNS { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Usuario")]
        public string UserHana { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Contraseña")]
        public string PasswordHana { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Nombre Base de Datos")]
        public string DBHana { get; set; }
    }
}
