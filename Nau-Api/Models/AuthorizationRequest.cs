using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class AuthorizationRequest
    {
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; }
    }
}
