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
        private static HashSet<LyricsFetcher> registeredProviders = new HashSet<LyricsFetcher>();
        #endregion

        #region public properties
        public static string[] RegisteredProviders
        {
            get
            {
                var providers = registeredProviders.Select(f => f.Provider);
                return providers.Count() != 0 ? providers.ToArray() : null;
            }
        }

        public abstract string Provider { get; }
        #endregion

        #region public methods
        public static bool RegisterProvider(LyricsFetcher fetcher) => registeredProviders.Add(fetcher);

        public static LyricsFetcher GetFetcher(string providerName)
        {
            if (!RegisteredProviders.Contains(providerName)) return null;

            switch (providerName)
            {
                default:
                    return null;
            }
        }

        public abstract string Fetch(string title, string artist, string album = null);
        #endregion

        #region private methods
        protected Task<XDocument> DownloadPageAsXmlAsync(string url) =>
            Task.Factory.StartNew(() => new SgmlReader { Href = url, IgnoreDtd = true })
            .ContinueWith(t => XDocument.Load(t.Result));
        #endregion
    }
}
