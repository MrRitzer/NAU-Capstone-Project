using System;

public class EmailAddress
{
    public string address { get; set; }
    public string permission_to_send { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string opt_in_source { get; set; }
    public DateTime opt_in_date { get; set; }
    public string opt_out_source { get; set; }
    public DateTime opt_out_date { get; set; }
    public string opt_out_reason { get; set; }
    public string confirm_status { get; set; }
}