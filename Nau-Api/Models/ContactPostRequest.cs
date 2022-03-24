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
            create_source = contact.create_source;
            birthday_month = contact.birthday_month;
            birthday_day = contact.birthday_day;
            anniversary = contact.anniversary;
            custom_fields = contact.custom_fields;
            street_addresses = StreetAddressPut.CreateList(contact.street_addresses);
            list_memberships = contact.list_memberships;
            taggings = contact.taggings;
            notes = contact.notes;
        }
        EmailAddress email_address { get; set; }
        string first_name { get; set; }
        string last_name { get; set; }
        string job_title { get; set; }
        string company_name { get; set; }
        string create_source { get; set; }
        int birthday_month { get; set; }
        int birthday_day { get; set; }
        string anniversary { get; set; }
        List<CustomField> custom_fields { get; set; }
        List<StreetAddressPut> street_addresses { get; set; }
        List<string> list_memberships { get; set; }
        List<string> taggings { get; set; }
        List<Note> notes { get; set; }
    }
}
