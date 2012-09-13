using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

public static class HttpModelStateDictionaryExtensions
{
    public static object ToMyArray(this System.Web.Http.ModelBinding.ModelStateDictionary modelState)
    {
        string keys = string.Empty;
        string values = string.Empty;

        int counter = 0;
        foreach (KeyValuePair<string, System.Web.Http.ModelBinding.ModelState> m in modelState)
        {
            if (m.Value.Errors.Count > 0)
            {
                counter += 1;
                if (keys.Length > 0)
                {
                    keys += "&";
                    values += "&";
                }

                keys += m.Key;
                values += m.Value.Errors[0].ErrorMessage;
            }
        }

        return new { IsModelStateError = true, length = counter, Keys = keys.Split('&'), Values = values.Split('&') };
    }

    public static void AddModelErrors(this System.Web.Http.ModelBinding.ModelStateDictionary modelState, Dictionary<string, string> ms)
    {
        if (ms != null)
        {
            if (ms.Count > 0)
            {
                foreach (KeyValuePair<string, string> k in ms)
                    modelState.AddModelError(k.Key, k.Value);
            }
        }
    }

    public static HttpResponseMessage GetInvalidResponse(this System.Web.Http.ModelBinding.ModelStateDictionary modelState, HttpRequestMessage request)
    {
        //var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        string errors = string.Empty;

        foreach (var key in modelState.Keys)
        {
            var state = modelState[key];
            if (state.Errors.Count > 0)
            {
                try
                {
                    if (state.Errors[0].ErrorMessage.Trim().Length > 0)
                    {
                        if (errors.Length > 0) errors += ",";
                        errors += string.Format("{0}:{1}", key, state.Errors[0].ErrorMessage);
                    }
                }
                catch { }
                //response.Content.Headers.Add(key, state.Errors[0].ErrorMessage);
            }
        }

        string _errors = "{" + errors + "}";
        var response = request.CreateResponse<string>(HttpStatusCode.BadRequest, _errors);
        
        return response;
    }
}

