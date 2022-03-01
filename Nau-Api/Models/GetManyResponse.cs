using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    //The response we get from the GetContacts method - '/contacts' api enpoint
    public class GetManyResponse
    {
        public List<Contact> contacts { get; set; }
        public int contacts_count { get; set; }
        public Links _links { get; set; }
        public string status { get; set; }
    }
}