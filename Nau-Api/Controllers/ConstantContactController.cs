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
        private const string baseUrl = "https://api.cc.email/v3/";

        public ConstantContactController(ILogger<ConstantContactController> logger, ApiConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        //Gets all lists related to the CC account and returns a GetListsResponse object
        [HttpGet]
        [Route("getlists")]
        public async Task<IActionResult> GetLists()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cc.email/v3/contact_lists?include_count=true"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _config.Token);
                        request.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");

                        var response = await httpClient.SendAsync(request);
                        string json = await response.Content.ReadAsStringAsync();

                        //This is an object containing all contact lists associated with the account
                        var listsResponse = JsonConvert.DeserializeObject<GetListsResponse>(json);

                        return Ok(listsResponse); //return the list of contacts
                    }
                }
            }
            catch (Exception e)
            {
                //calebx - I don't feel like writing error handling. I'll just log it and return sadness :)
                _logger.LogError("GetMany::" + e.Message);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [Route("createlist")]
        public async Task<IActionResult> CreateList([FromBody] ContactList list)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            string url = baseUrl + "contact_lists";
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                string listJson = JsonConvert.SerializeObject(list);
                byte[] bytes = encoder.GetBytes(listJson);

                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                if (bytes.Length > 0)
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.Created)
                    {
                        //Return the returned contact
                        var json = ReadResponse(webResponse);
                        ContactList response = JsonConvert.DeserializeObject<ContactList>(json);

                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogError("CreateList:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("CreateList:: Response: " + json);

                        return BadRequest(new ContactList());
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("CreateList::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("CreateList::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateList::" + e.Message);
            }

            return BadRequest(new ContactList());
        }

        [HttpPut]
        [Route("updatelist")]
        public async Task<IActionResult> UpdateList([FromBody] ContactList list)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                string url = baseUrl + "contacts/" + list.list_id;
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                UTF8Encoding encoder = new UTF8Encoding();
                string listJson = JsonConvert.SerializeObject(list);
                byte[] bytes = encoder.GetBytes(listJson);

                if (bytes.Length > 0)
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Return the returned contact
                        var json = ReadResponse(webResponse);
                        Contact response = JsonConvert.DeserializeObject<Contact>(json);

                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogError("UpdateList:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("UpdateList:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("UpdateList::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("UpdateList::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateList::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        //Takes in string of list 
        [HttpGet]
        [Route("getmany")]
        public async Task<IActionResult> GetMany([FromQuery] string tLists, [FromQuery] int limit)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cc.email/v3/contact_lists?include_count=true"))
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
                        using (var request = new HttpRequestMessage(new HttpMethod("GET"), baseUrl + "contacts"
                                + "?lists=" + string.Join(",", listIds)
                                + "&limit=" + limit))
                        {
                            request.Headers.TryAddWithoutValidation("Accept", "application/json");
                            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _config.Token);
                            //request.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");

                            var response = await httpClient.SendAsync(request);
                            
                            string json = await response.Content.ReadAsStringAsync();

                            
                            var contacts = JsonConvert.DeserializeObject<GetManyResponse>(json);

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

        [HttpGet]
        [Route("getcontact")]
        public async Task<IActionResult> GetContact([FromQuery] string email)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            if (email.Contains("@"))
            {
                email.Replace(@"@", "%40");
            }

            try
            {
                WebRequest request = WebRequest.Create(baseUrl + "contacts?email=" + email);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = ReadResponse(webResponse);
                        var contactResponse = JsonConvert.DeserializeObject<GetManyResponse>(json);
                        if (contactResponse.contacts.Count > 0)
                        {
                            return Ok(contactResponse.contacts[0]);
                        }
                        else { return Ok(new Contact()); }
                    }
                    else
                    {
                        _logger.LogError("GetContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("GetContact:: Response: " + json);

                        return BadRequest(new Contact());
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("CreateCustomer::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("CreateCustomer::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("GetContact::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        [HttpPost]
        [Route("createcontact")]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            string url = baseUrl + "contacts";
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                string contactJson = JsonConvert.SerializeObject(contact);
                byte[] bytes = encoder.GetBytes(contactJson);
                
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                if (bytes.Length > 0)
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.Created)
                    {
                        //Return the returned contact
                        var json = ReadResponse(webResponse);
                        Contact response = JsonConvert.DeserializeObject<Contact>(json);

                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogError("CreateContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("CreateContact:: Response: " + json);

                        return BadRequest(new Contact());
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("CreateCustomer::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("CreateCustomer::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateContact::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        [HttpPut]
        [Route("updatecontact")]
        public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                string url = baseUrl + "contacts/" + contact.contact_id;
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                UTF8Encoding encoder = new UTF8Encoding();
                string contactJson = JsonConvert.SerializeObject(contact);
                byte[] bytes = encoder.GetBytes(contactJson);

                if (bytes.Length > 0)
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Return the returned contact
                        var json = ReadResponse(webResponse);
                        Contact response = JsonConvert.DeserializeObject<Contact>(json);

                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogError("UpdateContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("UpdateContact:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("UpdateContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("UpdateContact::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateContact::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        [HttpDelete]
        [Route("deletecontact")]
        public async Task<IActionResult> DeleteContact([FromQuery] string contactId)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                string url = baseUrl + "contacts/" + contactId;
                WebRequest request = WebRequest.Create(url);
                request.Method = "DELETE";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (((int)webResponse.StatusCode) < 400)
                    {
                        return Ok("success");
                    }
                    else
                    {
                        _logger.LogError("CreateContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("CreateContact:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (((int)response.StatusCode) == 404)
                {
                    //Contact didn't exist. Unfortunately, 404 can mean a lot of things but they use it in this case to say not found. 
                    //Calebx - I'm not sure how NAU wants to handle this, it would be nice to tell the user that the person they tried to delete didn't exist I guess. 
                    return Ok();
                }
                string json = ReadResponse(response);
                _logger.LogError("DeleteContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogError("DeleteContact::Response: " + json);
            }

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
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

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

        private string ReadResponse(HttpWebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                return json;
            }
        }

        private string StringToB64(string input)
        {
            var response = Encoding.UTF8.GetBytes(input);
            string temp = Convert.ToBase64String(response);
            return temp;
        }
    }
}
