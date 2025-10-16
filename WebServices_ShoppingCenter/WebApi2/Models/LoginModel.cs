using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCenter.WebServices.Base.Models
{
    public class LoginModel
    {
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
