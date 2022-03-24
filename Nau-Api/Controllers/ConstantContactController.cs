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

        //Takes in string of list 
        [HttpGet]
        [Route("getmany")]//{tLists}/{limit}")]
        public async Task<IActionResult> GetMany([FromQuery] string tLists, [FromQuery] int limit)
        {
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            string[] listNames = tLists.Split("+");

            //First get all lists associated with the account.
            GetListsResponse listsResponse = null;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cc.email/v3/contact_lists?include_count=false"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _config.Token);
                        request.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");

                        var response = await httpClient.SendAsync(request);
                        string json = await response.Content.ReadAsStringAsync();

                        //This is an object containing all contact lists associated with the account
                        listsResponse = JsonConvert.DeserializeObject<GetListsResponse>(json);
                    }
                }
            }
            catch (Exception e)
            {
                //calebx - I don't feel like writing error handling. I'll just log it and return sadness :)
                _logger.LogError("GetMany::" + e.Message);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            //Parse the lists we get back to choose the ones requested
            List<string> listIds = new List<string>();
            if (listsResponse != null)
            {
                foreach (ContactList cl in listsResponse.lists)
                {
                    //if the client request contains this list's name, add the list id to listIds for a query
                    if (listNames.Contains(cl.name))
                    {
                        listIds.Add(cl.list_id);
                    }
                }

                //Now get all contacts associated with those lists
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cc.email/v3/contacts"
                                + "?lists=" + string.Join(",", listIds)
                                + "&limit=" + limit))
                        {
                            request.Headers.TryAddWithoutValidation("Accept", "application/json");
                            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _config.Token);
                            request.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");

                            var response = await httpClient.SendAsync(request);
                            
                            string json = await response.Content.ReadAsStringAsync();

                            
                            var contacts = JsonConvert.DeserializeObject<GetManyResponse>(json);

                            Response.Headers.Add("Access-Control-Allow-Origin", "*");
                            return Ok(contacts); //return the list of contacts
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("GetMany::" + e.Message);
                }
            }

            //If we're here something went wrong so return an empty list.
            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        //CALEBX - this is probably not the way it was designed to be done. But I wasn't sure how else to implement it.
        //This method recieves the code that we get back from the OAuth request made in the angular side of the project.
        //It takes the code and exchanges it for a token and other things which we use for any CC-API calls. 
        //Token is stored in ApiConfiguration object (this._config)
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
                        request.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");

                        var contentList = new List<string>();
                        contentList.Add("code=" + _code);
                        contentList.Add("redirect_uri=http%3A%2F%2Flocalhost%3A4200%2Fcallback");
                        contentList.Add("grant_type=authorization_code");

                        request.Content = new StringContent(string.Join("&", contentList));
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                        var response = await httpClient.SendAsync(request);
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
