using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Web;
using System.Xml;

namespace MusicBeePlugin.Net
{
    public class JLyricLyricsFetcher : LyricsFetcher
    {
        public override string Provider => "j-Lyric.net";

        public override string Fetch(string title, string artist, string album = null)
        {
            title  = HttpUtility.UrlEncode(title);
            artist = HttpUtility.UrlEncode(artist);
            string url = $"http://search.j-lyric.net/index.php?kt={title}&ct=1&ka={artist}&ca=1&kl=&cl=0";
            var headers = new Dictionary<HttpRequestHeader, string>
            {
                { HttpRequestHeader.UserAgent, "Mozilla/5.0(Windows NT 6.3; Win64; x64; Trident/7.0; rv: 11.0) like Gecko" },
            };
            var nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("x", "http://www.w3.org/1999/xhtml");

            XDocument searchPage = DownloadPageAsXml(url, headers);

            var results = searchPage.XPathSelectElements("//x:div[@id='lyricList']//x:div[@class='title']/x:a[@href]", nsMgr);
            string pageUrl = results.FirstOrDefault()?.Attribute("href").Value;

            if (pageUrl != null)
            {
                XDocument endPage = DownloadPageAsXml(pageUrl, headers);
                return endPage.XPathSelectElement("//x:p[@id='lyricBody']")?.Value.Replace("<br />", "");
            }
            else return null;
        }
    }
}
