using System.Collections.Generic;

namespace Nau_Api.Models
{
    public class PhoneNumberPut
    {
        public string phone_number { get; set; }
        public string kind { get; set; }

        public static List<PhoneNumberPut> CreateList(List<PhoneNumber> phones)
        {
            List<PhoneNumberPut> response = new List<PhoneNumberPut>();
            foreach (PhoneNumber phone in phones)
            {
                PhoneNumberPut temp = new PhoneNumberPut();
                temp.phone_number = phone.phone_number;
                temp.kind = phone.kind;

                response.Add(temp);
            }

            return response;
        }
    }
}
