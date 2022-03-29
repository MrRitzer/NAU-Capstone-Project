using Nau_Api.Models;
using System.Collections.Generic;

namespace Nau_Api.Models
{
    public class ContactPostRequest
    {
        public ContactPostRequest (Contact contact)
        {
            email_address = contact.email_address;
            first_name = contact.first_name;
            last_name = contact.last_name;
            job_title = contact.job_title;
            company_name = contact.company_name;
            create_source = "Account";
            birthday_month = contact.birthday_month;
            birthday_day = contact.birthday_day;
            anniversary = contact.anniversary;
            custom_fields = contact.custom_fields;
            phone_numbers = PhoneNumberPut.CreateList(contact.phone_numbers);
            street_addresses = StreetAddressPut.CreateList(contact.street_addresses);
            list_memberships = contact.list_memberships;
            taggings = contact.taggings;
            notes = contact.notes;
        }
        public EmailAddress email_address { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string job_title { get; set; }
        public string company_name { get; set; }
        public string create_source { get; set; }
        public int birthday_month { get; set; }
        public int birthday_day { get; set; }
        public string anniversary { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public List<PhoneNumberPut> phone_numbers { get; set; }
        public List<StreetAddressPut> street_addresses { get; set; }
        public List<string> list_memberships { get; set; }
        public List<string> taggings { get; set; }
        public List<Note> notes { get; set; }
    }
}
