using System.Collections.Generic;

namespace Nau_Api.Models
{
    public class StreetAddressPut
    {
        public string kind { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }

        public static List<StreetAddressPut> CreateList(List<StreetAddress> addresses)
        {
            List<StreetAddressPut> response = new List<StreetAddressPut>();
            foreach (var address in addresses)
            {
                StreetAddressPut temp = new StreetAddressPut();
                temp.kind = address.kind;
                temp.street = address.street;
                temp.city = address.city;
                temp.state = address.state;
                temp.postal_code = address.postal_code;
                temp.country = address.country;

                response.Add(temp);
            }

            return response;
        }
    }
}
