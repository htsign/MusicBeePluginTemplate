using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
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
        protected Task<XDocument> DownloadPageAsXmlAsync(string url) =>
            Task.Factory.StartNew(() => new SgmlReader { Href = url, IgnoreDtd = true })
            .ContinueWith(t => XDocument.Load(t.Result));
        #endregion
    }
}
