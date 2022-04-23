using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nau_Api.Models
{
    public class Contact
    {
        public string contact_id { get; set; } = null;
        public EmailAddress email_address { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string company_name { get; set; }
        public int birthday_month { get; set; }
        public int birthday_day { get; set; }
        public string anniversary { get; set; }
        public string update_source { get; set; }
        public string create_source { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string deleted_at { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public List<PhoneNumber> phone_numbers { get; set; }
        public List<StreetAddress> street_addresses { get; set; }
        public List<string> list_memberships { get; set; }
        public List<string> taggings { get; set; }
        public List<Note> notes { get; set; }
    }
}
