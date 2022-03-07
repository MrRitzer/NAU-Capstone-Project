using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Nau_Api.Models;
using Newtonsoft.Json;
using System.Text;

namespace Nau_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetMany([FromRoute] string _lists, [FromRoute] int limit)
        {
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
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
                                return Ok(response.contacts);
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
            return Ok(new List<Contact>());
        }

        //CALEBX - this is probably not the way it was designed to be done. But I wasn't sure how else to implement it.
        //This method recieves the code that we get back from the OAuth request made in the angular side of the project.
        //It takes the code and exchanges it for a token and other things which we use for any CC-API calls. 
        [HttpPost]
        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromForm] string _code)
        {
            if (String.IsNullOrWhiteSpace(_code))
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://authz.constantcontact.com/oauth2/default/v1/token"))
                    {
                        string b64Secret = StringToB64(_config.ClientID + ":" + _config.ClientSecret);
                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + b64Secret);

                        var contentList = new List<string>();
                        contentList.Add("code=" + _code);
                        contentList.Add("redirect_uri=http%3A%2F%2Flocalhost%3A4200%2Fcallback");
                        contentList.Add("grant_type=authorization_code");

                        request.Content = new StringContent(string.Join("&", contentList));
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                        var response = await httpClient.SendAsync(request);
                        HttpStatusCode temp = response.StatusCode;
                        string json = await response.Content.ReadAsStringAsync();

                        var authorization = JsonConvert.DeserializeObject<AuthorizationResponse>(json);
                        if (!String.IsNullOrWhiteSpace(authorization.access_token))
                        {
                            _config.Token = authorization.access_token;
                            _config.RefreshToken = authorization.refresh_token;
                        }

                        return Ok(authorization.access_token);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        private string StringToB64(string input)
        {
            var response = Encoding.UTF8.GetBytes(input);
            string temp = Convert.ToBase64String(response);
            return temp;
        }
    }
}
