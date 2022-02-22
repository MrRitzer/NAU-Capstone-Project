using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text.Json; //CALEBX - until we figure out how to include these idk what to do. I'm at a halt. 

public class ConstantContactController 
{
    private readonly string _baseUrl;

    public ConstantContactController(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    [HttpGet] //Get all contacts that match the given search parameters
    public List<ContactResource> GetContacts([FromBody] GetParameters parameters)
    {
        //Construct the request string with parameters
        string url = _baseUrl + "contacts?";
        if (!String.IsNullOrWhiteSpace(parameters.email))
        {
            url += "email=" + parameters.email + "&";
        }
        if (!String.IsNullOrWhiteSpace(parameters.status))
        {
            url += "status=" + parameters.status + "&";
        }
        if (parameters.lists != null && parameters.lists.Count > 0)
        {
            url += "lists=";
            foreach(string list in parameters.lists)
            {
                url += list + "%2C";
            }
            url = url.Substring(0, url.LastIndexOf("%2C"));
            url += "&";
        }
        if (parameters.limit > 0)
        {
            url += "limit=" + parameters.limit.ToString() + "&";
        }

        if (url.LastIndexOf("&") > 0) //the case where no parameters are passed
        {
            url = url.Substring(0, url.LastIndexOf('&'));
        }

        //Actually send the request
        try
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                //Check the response
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    //CALEBX - IDK what to to here because I don't have System.Text.Json
                    //properly referenced so I can't deserialize it. I guess we could send it 
                    //back to the typescript side and try to process it there. 
                    //The response we get back should be a json string of the form GetContactResponse.cs (models2).
                }
                else
                {
                    //calebx - there's a lot of possible response besides just 200
                    //we have to make a decision of how much effort we want to put into error handling. 
                }
            }
        }
        catch(WebException we)
        {
            //calebx - this isn't our problem to deal with since we don't know how they log errors
        }
        catch (Exception e)
        {
            //calebx - this isn't our problem to deal with since we don't know how they log errors
        }
        
    }
}