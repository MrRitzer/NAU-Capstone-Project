using System;
using System.Collections.Generic;

//The response we get from the GetContacts method - '/contacts' api enpoint
public class GetContactsResponse
{
    public List<Contact> contacts { get; set; }
    public int contacts_count { get; set; }
    public Links _links { get; set; }
    public string status { get; set; }
}