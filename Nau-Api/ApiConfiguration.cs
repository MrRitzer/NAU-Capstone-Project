using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api
{
    public class ApiConfiguration
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string ConstantContactUrl { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
