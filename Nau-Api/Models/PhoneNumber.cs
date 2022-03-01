using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class PhoneNumber
    {
        public string phone_number_id { get; set; }
        public string phone_number { get; set; }
        public string kind { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string update_source { get; set; }
        public string create_source { get; set; }
    }
}