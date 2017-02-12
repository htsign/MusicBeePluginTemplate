using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Sgml;

namespace MusicBeePlugin.Net
{
    public abstract class LyricsFetcher
    {
        #region private fields
        private static Dictionary<string, LyricsFetcher> registeredProviders = new Dictionary<string, LyricsFetcher>();
        #endregion

        #region public properties
        /// <summary>
        /// <see cref="LyricsFetcher"/> に登録済の歌詞提供事業者名リスト
        /// </summary>
        public static string[] RegisteredProviders => registeredProviders.Keys.ToArray();

        public abstract string Provider { get; }
        #endregion

        static LyricsFetcher()
        {
            RegisterProvider("DarkLyrics.com", new DarkLyricsLyricsFetcher());
        }

        #region public methods
        public static bool RegisterProvider(string provider, LyricsFetcher fetcher)
        {
            if (registeredProviders.ContainsKey(provider)) return false;
            registeredProviders[provider] = fetcher;
            return true;
        }

        public static LyricsFetcher GetFetcher(string providerName) => registeredProviders[providerName];

        public abstract string Fetch(string title, string artist, string album = null);
        #endregion

        #region private methods
        /// <summary>
        /// リクエスト先のHTMLをXMLドキュメントとして取得します。
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <param name="encoding">取得対象ドキュメントのエンコーディング</param>
        /// <param name="headers">追加HTTPヘッダ</param>
        /// <returns></returns>
        protected XDocument DownloadPageAsXml(
            string url,
            Encoding encoding,
            IDictionary<HttpRequestHeader, string> headers)
        {
            var wc = new WebClientEx { Encoding = encoding };
            if (headers != null)
            {
                foreach (KeyValuePair<HttpRequestHeader, string> header in headers)
                {
                    wc.Headers[header.Key] = header.Value;
                }
            }
            string html = wc.DownloadString(url);
            
            using (var reader = new SgmlReader { DocType = "HTML", IgnoreDtd = true })
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(html)))
            using (var sr = new StreamReader(ms))
            {
                reader.InputStream = sr;
                return XDocument.Load(reader);
            }
        }

        /// <summary>
        /// リクエスト先のHTMLをXMLドキュメントとして取得します。
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <param name="encoding">取得対象ドキュメントのエンコーディング</param>
        /// <returns></returns>
        protected XDocument DownloadPageAsXml(string url, Encoding encoding)
            => DownloadPageAsXml(url, encoding, null);

        /// <summary>
        /// リクエスト先のHTMLをXMLドキュメントとして取得します。
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <param name="headers">追加HTTPヘッダ</param>
        /// <returns></returns>
        protected XDocument DownloadPageAsXml(string url, IDictionary<HttpRequestHeader, string> headers)
            => DownloadPageAsXml(url, Encoding.UTF8, headers);

        /// <summary>
        /// リクエスト先のHTMLをXMLドキュメントとして取得します。
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <returns></returns>
        protected XDocument DownloadPageAsXml(string url) => DownloadPageAsXml(url, Encoding.UTF8);
        #endregion
    }
}
