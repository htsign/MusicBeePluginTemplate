using System;
using System.Diagnostics;
using System.Net;

namespace MusicBeePlugin.Net
{
    [DebuggerStepThrough]
    public class WebClientEx : WebClient
    {
        public string Referer { get; set; } = null;
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);

            if (req is HttpWebRequest hreq)
            {
                hreq.CookieContainer = CookieContainer;
            }
            return req;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var res = base.GetWebResponse(request);

            if (res is HttpWebResponse hres)
            {
                Referer = hres.ResponseUri.AbsoluteUri;
            }
            return res;
        }
    }
}
