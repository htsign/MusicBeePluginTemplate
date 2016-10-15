using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MusicBeePlugin.Net
{
    public class DarkLyricsLyricsFetcher : LyricsFetcher
    {
        private const string Host = "http://www.darklyrics.com/";

        public override string Provider => "DarkLyrics.com";

        public override string Fetch(string title, string artist, string album = null)
        {
            var queries = new StringBuilder($"\"{title}\" \"{artist}\"");
            if (!string.IsNullOrEmpty(album)) queries.Append($" \"{album}\"");
            
            var nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("x", "http://www.w3.org/1999/xhtml");

            return DownloadPageAsXmlAsync(Host + "search?q=" + HttpUtility.UrlEncode(queries.ToString()))
            .ContinueWith(t =>
            {
                XElement a = t.Result.XPathSelectElement(@"//x:h3[@class='seah' and .='Songs:']/following-sibling::x:div//x:a", nsMgr);
                return a?.Attribute("href").Value;
            })
            .ContinueWith(t => DownloadPageAsXmlAsync(Host + t.Result)
                .ContinueWith(t2 =>
                {
                    int n = int.Parse(t.Result.Split('#')[1]);
                    string lyricsRoot = @"//x:div[@class='lyrics']";
                    XDocument doc = t2.Result;

                    // ref: https://os0x.g.hatena.ne.jp/os0x/20080307/1204903268
                    // <h3><a name="{n}">n. [title]</a></h3> から <h3><a name="{n+1}">n+1. [title]</a></h3> まで取得
                    var targetNodes = (IEnumerable<object>)doc.XPathEvaluate($@"{lyricsRoot}/x:h3[x:a[@name='{n}']]/following-sibling::node()[following::x:h3[x:a[@name='{n + 1}']]]", nsMgr);
                    if (targetNodes.Count() == 0)
                    {
                        targetNodes = ((IEnumerable<object>)doc.XPathEvaluate($@"{lyricsRoot}/x:h3[x:a[@name='{n}']]/following-sibling::node()", nsMgr))
                            .TakeWhile(node =>
                            {
                                if (((XNode)node).NodeType != XmlNodeType.Element) return true;
                                var elem = (XElement)node;
                                if (elem.Name.LocalName == "div")
                                {
                                    XName attr = elem.Attribute("class")?.Name;
                                    return attr != "thanks" && attr != "note";
                                }
                                return elem.Name.LocalName != "a";
                            });
                    }
                    var items = targetNodes.Select(node =>
                    {
                        if (((XNode)node).NodeType == XmlNodeType.Element)
                        {
                            var elem = (XElement)node;
                            if (elem.Name.LocalName == "br") return "\r\n";
                            else if (elem.Name.LocalName == "i") return elem.Value.Trim();
                            else return null;
                        }
                        else if (((XNode)node).NodeType == XmlNodeType.Text) return ((XText)node).Value.Trim();
                        else return null;
                    });
                    return items.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                })
                .ContinueWith(t2 => string.Join("", t2.Result)).Result
            ).Result.Trim();
        }
    }
}
