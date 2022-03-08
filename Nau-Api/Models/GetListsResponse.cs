using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class GetListsResponse
    {
        public List<ContactList> lists { get; set; }
        public int lists_count { get; set; }
        public Links _links { get; set; }
    }
}
