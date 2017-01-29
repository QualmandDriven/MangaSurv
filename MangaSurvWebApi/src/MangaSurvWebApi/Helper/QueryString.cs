using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Helper
{
    public class QueryString
    {
        private readonly Dictionary<string, string> _querystring;

        public QueryString(HttpRequest httpRequest)
        {
            this._querystring = new Dictionary<string, string>();

            // Load informations out of HttpRequest
            if (httpRequest.QueryString.HasValue)
            {
                foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> pair in httpRequest.Query)
                {
                    this._querystring.Add(pair.Key.ToUpper(), pair.Value.ToString());
                }
            }
        }

        public bool ContainsKeys()
        {
            return this._querystring.Count != 0;
        }

        public bool ContainsKey(string sKey)
        {
            return this._querystring.ContainsKey(this.FormatKey(sKey));
        }

        public string GetValue(string sKey)
        {
            return this._querystring[this.FormatKey(sKey)];
        }

        public bool TryGetValue(string sKey, out string sValue)
        {
            return this._querystring.TryGetValue(this.FormatKey(sKey), out sValue);
        }

        private string FormatKey(string sKey)
        {
            return sKey.ToUpper();
        }
    }
}
