using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Nau_Api.Models;
using Newtonsoft.Json;

namespace Nau_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConstantContactController : Controller
    {
        private readonly ILogger<ConstantContactController> _logger;
        private readonly ApiConfiguration _config;

        public ConstantContactController(ILogger<ConstantContactController> logger, ApiConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet] //Get all contacts that match the given search parameters
        [Route("getmany/{_lists}/{limit:int}")]
        public List<Contact> GetMany([FromRoute] string _lists, [FromRoute] int limit)
        {
            string[] lists = _lists.Split("+");
            //Construct the request string with parameters
            string url = _config.ConstantContactUrl + "contacts?";

            //Add parameters to URL
            if (lists != null && lists.Length > 0)
            {
                url += "lists=";
                foreach (string list in lists)
                {
                    url += list + "%2C";
                }
                url = url.Substring(0, url.LastIndexOf("%2C"));
                url += "&";
            }
            if (limit > 0)
            {
                url += "limit=" + limit.ToString() + "&";
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
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            try
                            {
                                var json = reader.ReadToEnd();
                                _logger.LogDebug("CALEBX - \n" + json);

                                var response = JsonConvert.DeserializeObject<GetManyResponse>(json);
                                return response.contacts;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                            }
                        }
                    }
                    else
                    {
                        //calebx - there's a lot of possible response besides just 200
                        //we have to make a decision of how much effort we want to put into error handling. 
                    }
                }
            }
            catch (WebException we)
            {
                //calebx - this isn't our problem to deal with since we don't know how they log errors
            }
            catch (Exception e)
            {
                //calebx - this isn't our problem to deal with since we don't know how they log errors
            }

            //If we're here something went wrong so return an empty list.
            return new List<Contact>();
        }

        [HttpPost]
        [Route("authorize")]
        private async Task<IActionResult> Authorize([FromBody] string code)
        {
            try
            {
                string url = "https://authz.constantcontact.com/oauth2/default/v1/token" +
                    "?code=" + code +
                    "&redirect_uri=http://localhost:4200" +
                    "&grant_type=authorization_code";
                WebRequest request = WebRequest.Create(url);
                return Ok("test");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }
    }
}
