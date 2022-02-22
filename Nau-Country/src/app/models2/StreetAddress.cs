using System;

public class StreetAddress
{
    public string street_address_id { get; set; }
    public string kind { get; set; }
    public string street { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string postal_code { get; set; }
    public string country { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}