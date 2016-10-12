using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Net
{
    public abstract class LyricsFetcher
    {
        #region private fields
        private static HashSet<LyricsFetcher> registeredProviders = new HashSet<LyricsFetcher>();
        #endregion

        #region public properties
        public static string[] RegisteredProviders => registeredProviders.Select(f => f.Provider).ToArray();

        public abstract string Provider { get; }
        #endregion

        #region public methods
        public static bool RegisterProvider(LyricsFetcher fetcher) => registeredProviders.Add(fetcher);

        public abstract string Fetch(string title, string artist, string album = null);
        #endregion
    }
}
