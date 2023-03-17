using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Security.Authentication;
using System.Reflection;
using System.Net;
using System.Web.Mvc;

using System.Configuration;


using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;

namespace Klijent.Controllers
{

    public class Base : Controller, IDisposable
    {

        protected static string WsBaseUri
        {
            get { return ConfigurationManager.AppSettings["WebserviceBaseUri"]; }
        }

      

        protected static NetworkCredential getWsCredentials()
        {
            string username = ConfigurationManager.AppSettings["WebserviceUsername"];
            string password = ConfigurationManager.AppSettings["WebservicePassword"];
            return new NetworkCredential(username, password);
        }

  
     
        /// <summary>
        /// Sends a GET request to the web service
        /// </summary>
        /// <typeparam name="Tmodel">Data type to return</typeparam>
        /// <param name="requestUri">Request Uri (controller name)</param>
        /// <returns>the specified data type</returns>
        protected static Tmodel wsGet<Tmodel>(string requestUri)
        {
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.Credentials = getWsCredentials(MC);

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["apiTimeoutSec"]));
                    httpClient.BaseAddress = new Uri(WsBaseUri);
                    Task<String> response = httpClient.GetStringAsync(requestUri);
                    try
                    {
                        //return JsonConvert.DeserializeObjectAsync<Tmodel>(response.Result).Result;
                        return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Tmodel>(response.Result)).Result;
                    }
                    catch
                    {
                        return HandleHttpError<Tmodel>(response.Result);
                    }
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(x => x is TaskCanceledException))
                {
                    //request timed out
                    //Error(ex, callerMemberName, null, requestUri);
                    return default(Tmodel);
                }
                throw ex;
            }
            catch (Exception ex)
            {
                //  Error(ex, callerMemberName, null);
                throw ex;
            }
        }
     
    


        protected Treturn wsPost<Tmodel, Treturn>(string requestUri, Tmodel value)
        {

            //    HttpClientHandler handler = new HttpClientHandler();
            //    handler.Credentials = getWsCredentials(MC);
            try
            {
                using (System.Net.Http.HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["apiTimeoutSec"]));
                    httpClient.BaseAddress = new Uri(WsBaseUri);
                    var response = httpClient.PostAsJsonAsync<Tmodel>(requestUri, value).Result; // PASS THE IDCARD OR PASSPORT
                    var msg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //return JsonConvert.DeserializeObjectAsync<Treturn>(msg).Result;
                        return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Treturn>(msg)).Result;
                    }
                    else
                    {
                        return HandleHttpError<Treturn>(msg);
                    }
                }
            }
            catch (AggregateException ex)
            {
                //For reference: json object string lentgh is ~269700 when posting to method DLSPTaddImagesLPApp
                if (ex.InnerExceptions.Any(x => x is TaskCanceledException))
                {
                    //request timed out
                    // Error(ex, callerMemberName, value, requestUri);
                    return default(Treturn);
                }

                throw ex;
            }
            catch (Exception ex)
            {
                // Error(ex, callerMemberName);
                throw ex;
            }
        }

     

     
        


        protected static Treturn HandleHttpError<Treturn>(string responseResult)
        {
            HttpError err;

            try
            {
                //err = JsonConvert.DeserializeObjectAsync<HttpError>(responseResult).Result;
                err = Task.Factory.StartNew(() => JsonConvert.DeserializeObject<HttpError>(responseResult)).Result;

                if (err.ExceptionType == null)
                    return default(Treturn);
            }
            catch
            {
                return default(Treturn);
            }

            switch (err.ExceptionType)
            {
                /*  case HttpErrorType.DbUpdateException:
                      throw new DbUpdateException(err.ExceptionMessage);
                  case HttpErrorType.EntityCommandExecutionException:
                      throw new System.Data.Entity.Core.EntityCommandExecutionException(err.ExceptionMessage);
                  case HttpErrorType.AuthenticationException:
                      throw new AuthenticationException(err.ExceptionMessage);
                  default:
                      throw new Exception(err.ExceptionMessage);*/
            }
            throw new Exception(err.ExceptionMessage);


        }



        protected void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            var context = filterContext.HttpContext;

            if (!filterContext.IsChildAction && context.Request.QueryString.Get("eq") != null)
            {
                string encryptedQueryString = context.Request.QueryString.Get("eq");
                string decrptedString = encryptedQueryString.ToString();//QsEncoding.Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');

                    int n;
                    bool b;
                    if (int.TryParse(paramArr[1], out n))
                        decryptedParameters.Add(paramArr[0], n);
                    else if (bool.TryParse(paramArr[1], out b))
                        decryptedParameters.Add(paramArr[0], b);
                    else
                        decryptedParameters.Add(paramArr[0], paramArr[1]);
                }
            }
            else if (!filterContext.IsChildAction && filterContext.ActionParameters.Count > 0)
            {
                foreach (var q in filterContext.ActionParameters)
                {
                    try
                    {
                        string rawVal = filterContext.Controller.ValueProvider.GetValue(q.Key).AttemptedValue;
                        var sQs = rawVal;// QsEncoding.Decrypt(rawVal);

                        if (Encoding.UTF8.GetByteCount(sQs) == sQs.Length)
                        {
                            int n;
                            bool b;
                            if (int.TryParse(sQs, out n))
                                decryptedParameters.Add(q.Key, n);
                            else if (bool.TryParse(sQs, out b))
                                decryptedParameters.Add(q.Key, b);
                            else
                                decryptedParameters.Add(q.Key, sQs);

                            ModelState[q.Key].Errors.Clear();
                        }
                    }
                    catch
                    {
                        //DO NOTHING
                    }
                }
            }
            else if (!filterContext.IsChildAction && context.Request.QueryString.Count > 0)
            {
                foreach (string qs in Request.QueryString.Keys)
                {
                    try
                    {
                        var sQs = Request.QueryString[qs];//QsEncoding.Decrypt(Request.QueryString[qs]);
                        //SD 15/Nov/2016 - Check that decrypted string contains no non-ascii characters
                        if (Encoding.UTF8.GetByteCount(sQs) == sQs.Length)
                        {
                            int n;
                            if (int.TryParse(sQs, out n))
                                decryptedParameters.Add(qs, n);
                            else
                                decryptedParameters.Add(qs, sQs);
                        }
                    }
                    catch
                    {
                        //DO NOTHING
                    }
                }
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            base.OnActionExecuting(filterContext);
        }






    }
}