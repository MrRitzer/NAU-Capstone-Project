using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class ContactList
    {
        public string list_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool favorite { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int membership_count { get; set; }
    }
}
