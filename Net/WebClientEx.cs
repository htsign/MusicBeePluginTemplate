using System;
using System.Diagnostics;
using System.Net;

namespace MusicBeePlugin.Net
{
    [DebuggerStepThrough]
    public class WebClientEx : WebClient
    {
        public string Referer = null;
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);

            if (req is HttpWebRequest)
            {
                ((HttpWebRequest)req).CookieContainer = CookieContainer;
            }
            return req;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var res = base.GetWebResponse(request);

            if (res is HttpWebResponse)
            {
                Referer = ((HttpWebResponse)res).ResponseUri.AbsoluteUri;
            }
            return res;
        }
    }
}
