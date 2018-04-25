using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            XDocument searchPage = DownloadPageAsXml(Host + "search?q=" + HttpUtility.UrlEncode(queries.ToString()));

            // 'Songs:' ってテキストを持った h3.seah の次の要素の中にある a[href] の値が歌詞のあるページURL
            XElement a = searchPage.XPathSelectElement(@"//x:h3[@class='seah' and .='Songs:']/following-sibling::x:div//x:a", nsMgr);
            string href = a?.Attribute("href")?.Value ??
                throw new Exception("なんか知らんけど歌詞URL取れんかった。\r\n" +
                                    $"Title: \"{title}\", Artist: \"{artist}\"");
            string endPageUrl = href;
            
            XDocument doc = DownloadPageAsXml(Host + endPageUrl);
            // URLの # 以降にトラック番号があるんだけど、万が一の # が無かった場合を考慮してないのでコケたらすまんな
            int n = int.Parse(endPageUrl.Split('#')[1]);
            
            string lyricsRoot = @"//x:div[@class='lyrics']";

            // ref: https://os0x.g.hatena.ne.jp/os0x/20080307/1204903268
            // h3 > a[name="{n}"]   から
            // h3 > a[name="{n+1}"] まで取得
            var targetNodes = (IEnumerable<object>)doc.XPathEvaluate($@"{lyricsRoot}/x:h3[x:a[@name='{n}']]/following-sibling::node()[following::x:h3[x:a[@name='{n + 1}']]]", nsMgr);
            if (targetNodes.Count() == 0)
            {
                // 上の条件で見つからなかった場合は
                // h3 > a[name="{n}"]      から
                // div.thanks, div.note, a まで取得
                targetNodes = ((IEnumerable<object>)doc.XPathEvaluate($@"{lyricsRoot}/x:h3[x:a[@name='{n}']]/following-sibling::node()", nsMgr))
                    .TakeWhile(node =>
                    {
                        switch (node)
                        {
                            case XElement elem:
                                if (elem.Name.LocalName == "div")
                                {
                                    XName attr = elem.Attribute("class")?.Name;
                                    return attr != "thanks" && attr != "note";
                                }
                                return elem.Name.LocalName != "a";
                            default:
                                return true;
                        }
                    });
            }
            // それでも見つからなかったら知らん

            var items = targetNodes.Select(obj =>
            {
                switch (obj)
                {
                    case XElement elem:
                        // br なら改行にして、i ならその中のテキストを取り出す
                        switch (elem.Name.LocalName)
                        {
                            case "br": return "\r\n";
                            case "i":  return elem.Value.Trim();
                            default:   return null;
                        }
                    case XText text:
                        // TextNode ならテキストを取り出す
                        return text.Value.Trim();
                }
                return null;
            });
            // シーケンスから空のテキストを除外
            var parts = items.Where(s => !string.IsNullOrEmpty(s));
            return string.Join("", parts).Trim();
        }
    }
}
