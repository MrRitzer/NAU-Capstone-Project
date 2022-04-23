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
using Nau_Api.Models; //Where we store all of our JSON Objects
using Newtonsoft.Json; //We are using Newtonsoft.Json for all of our JSON de/serializing. 
using System.Text;

namespace Nau_Api.Controllers
{
    [Route("api/[controller]")] //the route on actual requests will look like https://example.com/api/ConstantContact/{method route}
    [ApiController]
    public class ConstantContactController : Controller
    {
        private readonly ILogger<ConstantContactController> _logger;
        private readonly ApiConfiguration _config;
        private string baseUrl = "";

        public ConstantContactController(ILogger<ConstantContactController> logger, ApiConfiguration config)
        {
            _logger = logger;
            _config = config;
            baseUrl = _config.ConstantContactUrl; //I set this up just so I didn't have to type "_config.ConstantContactUrl" as much when coding
        }

        /// <summary>
        ///     Gets all lists related to the CC account and returns a GetListsResponse object
        ///     Relates to the "Contact Lists > GET Lists Collection" call in CC API reference
        /// </summary>
        /// <returns>GetListsResponse object</returns>
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
                //include_count is one of a few options: limit, include_count, include_membership_count
                WebRequest request = WebRequest.Create(baseUrl + "contact_lists?include_count=true");
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = ReadResponse(webResponse);
                        var listResponse = JsonConvert.DeserializeObject<GetListsResponse>(json);

                        return Ok(listResponse);
                    }
                    else
                    {
                        _logger.LogError("GetContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("GetContact:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("GetLists::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("GetLists::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("GetLists::" + e.Message);
            }

            return BadRequest(new GetListsResponse()); //something along the way went wrong. Return empty object
        }

        /// <summary>
        ///     Takes in a new list object from the client side and posts it to the
        ///     CC API to have it created.
        ///     Relates to the "Contact Lists > POST (create) a List" call in CC API reference
        /// </summary>
        /// <param name="list">ContactList object sent by client side</param>
        /// <returns>ContactList object created and returned by CC API</returns>
        [HttpPost]
        [Route("createlist")]
        public async Task<IActionResult> CreateList([FromBody] ContactList list)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            string url = baseUrl + "contact_lists"; //POST endpoint of contact_lists
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                string listJson = JsonConvert.SerializeObject(list);
                byte[] bytes = encoder.GetBytes(listJson);

                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                //Encode new list
                if (bytes.Length > 0)
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.Created) //POSTS (creates) return Created status code not OK
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

        /// <summary>
        ///     Takes a ContactList object which will replace, based on ContactList.list_id, the stored data in 
        ///     the CC API. If a list is "deleted" this will reactivate the list.
        ///     Relates to the "Contact Lists > PUT (update) a List" call in CC API reference
        /// </summary>
        /// <param name="list">ContactList object as update input</param>
        /// <returns>ContactList object of the updated list</returns>
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
                string url = baseUrl + "contact_lists/" + list.list_id;
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
                        ContactList response = JsonConvert.DeserializeObject<ContactList>(json);

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

        /// <summary>
        ///     Takes in a ContactList.list_id and deletes said list. However, deletion really means something more like 
        ///     deactivate. Deletions can be undone use the UpdateList method. 
        ///     Relates to the "Contact Lists > DELETE a List" call in CC API reference
        /// </summary>
        /// <param name="listId">String list_id of the contact list to delete</param>
        /// <returns>
        ///     Technically CC API returns a ActivityDeleteListResponse object but 
        ///     I didn't implement that since it seemed useless. So I return a string "success", "dne", "failed"
        /// </returns>
        [HttpDelete]
        [Route("deletelist")]
        public async Task<IActionResult> DeleteList([FromQuery] string listId)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                string url = baseUrl + "contact_lists/" + listId;
                WebRequest request = WebRequest.Create(url);
                request.Method = "DELETE";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token);

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.Accepted) //they use Accepted not OK here
                    {
                        return Ok("success");
                    }
                    else
                    {
                        _logger.LogError("DeleteList:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("DeleteList:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    //Contact didn't exist. Unfortunately, 404 can mean a lot of things but they use it in this case to say not found. 
                    //Calebx - I'm not sure how NAU wants to handle this, it would be nice to tell the user that the list they tried to delete didn't exist I guess. 
                    return Ok("dne"); //dne = "Does not exist"
                }
                string json = ReadResponse(response);
                _logger.LogError("DeleteList::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogError("DeleteList::Response: " + json);
            }

            return BadRequest("failed");
        }

        /// <summary>
        ///     This method takes in 2 of 9 possible search filters (lists and limit) to search through all the contacts associated with your account. 
        ///     This method filters by the inputed list ids and limits to a max number of contacts which it returns.
        /// </summary>
        /// <param name="lists">String of comma separated list ids</param>
        /// <param name="limit">Integer to set the max number of contacts to return</param>
        /// <returns>GetManyResponse object. Could be an empty object in the case of bad request</returns>
        [HttpGet]
        [Route("getmany")]
        public async Task<IActionResult> GetMany([FromQuery] string lists, [FromQuery] int limit)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*"); //Don't forget to add your own CORs implementation. 

            if (String.IsNullOrWhiteSpace(_config.Token)) //If this is null then the app hasn't been authorized yet.
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            if (String.IsNullOrWhiteSpace(lists)) //If lists is null we have nothing to search by so return a bad request and blank object
            {
                return BadRequest(new GetManyResponse());
            }

            try
            {
                //Rather than have a request body we just encode our variables in the URL for GET and delete calls
                WebRequest request = WebRequest.Create(baseUrl + "contacts"
                                                               + "?lists=" + lists
                                                               + "&limit=" + limit
                                                               + "&include=list_memberships");
                request.Method = "GET";
                request.ContentType = "application/json"; 
                request.Headers.Add("Authorization", "Bearer " + _config.Token); //Need this for any CC API call

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse()) //Get the actual response
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = ReadResponse(webResponse);
                        var response = JsonConvert.DeserializeObject<GetManyResponse>(json);

                        return Ok(response);
                    }
                    else
                    {
                        //Log the status code and description, then log the response JSON
                        _logger.LogError("GetMany:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("GetMany:: Response: " + json);

                        return BadRequest(new GetManyResponse());
                    }
                }
            }
            catch (WebException we)
            {
                //Casting exceptio response as a webresponse lets us access the response JSON. CC actually sends back some pretty nice
                //errors inside some JSON so we can log that for a better idea of what went wrong. 
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                //we also log the status code and description
                _logger.LogError("GetMany::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("GetMany::Response: " + json);
            }
            catch (Exception e)
            {
                //Something else went wrong and we don't know what so we'll just log it and move on. 
                _logger.LogError("GetMany::" + e.Message);
            }

            return BadRequest(new GetManyResponse()); //At some point something went wrong so we will send back a bad request and empty object
        }

        /// <summary>
        ///     Takes in an email as a string which we then send off to the CC API. This is actually
        ///     not the GET a Contact call in the CC API. Rather it is a riff off of the Get Contacts collection
        ///     call because the get contact (singular) call takes a Contact.contact_id not an email even though
        ///     the email is required to be unique. We thought the email made more sense to search by.
        ///     Relates to the "Contacts > GET Contacts Collection" call in CC API
        /// </summary>
        /// <param name="email">String email which is the email address of the desired contact. </param>
        /// <returns>GetManyResponse object</returns>
        [HttpGet]
        [Route("getcontact")]
        public async Task<IActionResult> GetContact([FromQuery] string email)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            try
            {
                WebRequest request = WebRequest.Create(baseUrl + "contacts?email=" + email
                                                               + "&include=list_memberships");
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
                _logger.LogError("GetContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("GetContact::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("GetContact::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        /// <summary>
        ///     Takes in a Contact object from the client side and posts it to the CC API.
        ///     The CC API creates a new contact from it and returns the created object for us. 
        ///     Relates to the "Contacts > POST (create) a Contact" call in CC API reference
        /// </summary>
        /// <param name="contact">Contact object to create</param>
        /// <returns>Contact object returned by CC API</returns>
        [HttpPost]
        [Route("createcontact")]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (String.IsNullOrWhiteSpace(_config.Token))
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }

            string url = baseUrl + "contacts"; //relates to the POST endpoint of "contacts"
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
                    if (webResponse.StatusCode == HttpStatusCode.Created) //we use the created status code here not OK status code
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
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("CreateContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("CreateContact::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("CreateContact::" + e.Message);
            }

            return BadRequest(new Contact());
        }

        /// <summary>
        ///     Takes a Contact object which will replace, based on Contact.contact_id, the stored data in 
        ///     the CC API. If a list is "deleted" this will reactivate the list.
        ///     Relates to the "Contacts > PUT (update) a Contact" call in CC API reference
        /// </summary>
        /// <param name="contact">Contact object which will overwrite old contact in CC API</param>
        /// <returns>Contact object which is updated and returned by CC API</returns>
        [HttpPut]
        [Route("updatecontact")]
        public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*"); //calebx - this will have to be replaced with your own CORs implementation
            
            if (String.IsNullOrWhiteSpace(_config.Token)) //If our _config.Token is null we have not authorized the application yet
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized); //Any <returns></returns> comment I wrote ignores this case
            }

            try
            {
                string url = baseUrl + "contacts/" + contact.contact_id; //First we determine our URL. The base url is stored in appsettings.json
                WebRequest request = WebRequest.Create(url);
                request.Method = "PUT"; //The type of call we are making. 
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + _config.Token); //this token is the one we set using the Authorize method during runtime. Every call needs it

                UTF8Encoding encoder = new UTF8Encoding();
                string contactJson = JsonConvert.SerializeObject(contact); //Convert our object to a JSON string so we can encode it into the request
                byte[] bytes = encoder.GetBytes(contactJson);

                if (bytes.Length > 0) //Encode the JSON object into the request
                {
                    request.ContentLength = bytes.Length;
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(bytes, 0, bytes.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse()) //Actually send the response. If it is a good response we
                                                                                             //we continue, otherwise we move to the first catch
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK) 
                    {
                        //Return the returned contact
                        var json = ReadResponse(webResponse); //ReadResponse is a custom method at the bottom of the controller
                        Contact response = JsonConvert.DeserializeObject<Contact>(json); //All CC API calls will return some sort of JSON object to be converted using our models

                        return Ok(response); //Return the response to the client side. 
                    }
                    else
                    {
                        //I first log the status code and description
                        _logger.LogError("UpdateContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        
                        //Constant contact actually sends back pretty nice JSON error responses for us to log. I would look here for any issues
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("UpdateContact:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                //Log the status code and description then the JSON error message just like the else statement above
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("UpdateContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("UpdateContact::Response: " + json);
            }
            catch (Exception e)
            {
                //We ran into some other random exception. Log it and move on. 
                _logger.LogError("UpdateContact::" + e.Message);
            }

            return BadRequest(new Contact()); //Something along the way went wrong so send back a bad request with a blank contact object
        }


        /// <summary>
        ///     Takes in a Contact.contact_id and deletes said contact. Contact is not technically
        ///     deleted, rather they are marked as inactive and can be reactivated with UpdateContact
        ///     Relates to the "Contacts > DELETE a Contact" call in CC API reference
        /// </summary>
        /// <param name="contactId">String list_id of the contact list to delete</param>
        /// <returns>
        ///     Technically CC API returns a ActivityDeleteListResponse object but 
        ///     I didn't implement that since it seemed useless. So I return a string "success", "dne", "failed"
        /// </returns>
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
                    if (webResponse.StatusCode == HttpStatusCode.Accepted)
                    {
                        return Ok("success");
                    }
                    else
                    {
                        _logger.LogError("DeleteContact:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("DeleteContact:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    //Contact didn't exist. Unfortunately, 404 can mean a lot of things but they use it in this case to say not found. 
                    //Calebx - I'm not sure how NAU wants to handle this, it would be nice to tell the user that the person they tried to delete didn't exist I guess. 
                    return Ok("dne"); //dne = "Does not exist"
                }
                string json = ReadResponse(response);
                _logger.LogError("DeleteContact::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogError("DeleteContact::Response: " + json);
            }

            return BadRequest("failed");
        }

        //CALEBX - this is probably not the way it was designed to be done. But I wasn't sure how else to implement it.
        //This method recieves the code that we get back from the OAuth request made in the angular side of the project.
        //It takes the code and exchanges it for a token and other things which we use for any CC-API calls. 
        //Token is stored in ApiConfiguration object (this._config)
        //You can find more information on proper implementation here: https://v3.developer.constantcontact.com/api_guide/server_flow.html
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
                WebRequest request = WebRequest.Create("https://authz.constantcontact.com/oauth2/default/v1/token" 
                                                    + "?code=" + _code
                                                    + "&redirect_uri=http%3A%2F%2Flocalhost%3A4200%2Fcallback" //make sure this matches the one on client side
                                                    + "&grant_type=authorization_code");

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string b64Secret = StringToB64(_config.ClientID + ":" + _config.ClientSecret);
                request.Headers.Add("Authorization", "Basic " + b64Secret); //Need this to get access. See link in big comment above for more info

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Read the code, set the code, return the code
                        var json = ReadResponse(webResponse);
                        var response = JsonConvert.DeserializeObject<AuthorizationResponse>(json);

                        if (!String.IsNullOrWhiteSpace(response.access_token))
                        {
                            _config.Token = response.access_token; //code is stored in a customer ApiConfiguration I created and implemented using a singleton in Startup.cs
                                                                   //If you get to this point you can be confident that authorization is set and the calls shouldn't fail for that reason
                            _config.RefreshToken = response.refresh_token; //If you want a more persisten authorization you'll have to refresh the token. 
                        }

                        return Ok("success"); //Ok(response.access_token); 
                                              //You can return whatever you want here. The server side is the one that uses the token to make calls
                    }
                    else
                    {
                        _logger.LogError("Authorize:: response not ok. Status code: " + webResponse.StatusCode + ", Description: " + webResponse.StatusDescription);
                        string json = ReadResponse(webResponse);
                        _logger.LogInformation("Authorize:: Response: " + json);
                    }
                }
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                string json = ReadResponse(response);
                _logger.LogError("Authorize::StatusCode: " + response.StatusCode + ", Description: " + response.StatusDescription);
                _logger.LogInformation("Authorize::Response: " + json);
            }
            catch (Exception e)
            {
                _logger.LogError("Authorize::" + e.Message);
            }

            return BadRequest("failed");
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
