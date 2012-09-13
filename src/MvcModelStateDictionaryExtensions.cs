using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MvcModelStateDictionaryExtensions
{
    public static object ToMyArray(this System.Web.Mvc.ModelStateDictionary modelState)
    {
        string keys = string.Empty;
        string values = string.Empty;

        int counter = 0;
        foreach (KeyValuePair<string, System.Web.Mvc.ModelState> m in modelState)
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

    public static void AddModelErrors(this System.Web.Mvc.ModelStateDictionary modelState, Dictionary<string, string> ms)
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
}

